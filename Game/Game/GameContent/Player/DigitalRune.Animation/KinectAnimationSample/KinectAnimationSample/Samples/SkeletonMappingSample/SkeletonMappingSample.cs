using System;
using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace KinectAnimationSample
{
  // This sample uses the two players that Kinect can track to animate two different 3D models
  // (Dude and Marine). 
  // The difficulty is that Kinect skeleton data consists only of joint positions - no bone
  // rotations. Additionally, the bone hierarchy of a Kinect skeleton, the Dude skeleton and the
  // Marine skeleton are different. (The bone names and even the number of bones are different!)  
  // The SkeletonMapper class (from DigitalRune Animation) is used to transfer a Kinect
  // SkeletonPose to model's SkeletonPose. Translations of the root bones will be transferred.
  // For all other bones, only bone rotations are transferred. 
  // A model is only visible when a player is detected. 
  public class SkeletonMappingSample : SampleBase
  {
    // The 3D models.
    private Model _modelA;
    private Model _modelB;

    // The SkeletonPoses that animate the models.
    private SkeletonPose _skeletonPoseA;
    private SkeletonPose _skeletonPoseB;

    // The SkeletonMappers that map Kinect SkeletonPoses to the model's SkeletonPoses.
    private SkeletonMapper _skeletonMapperA;
    private SkeletonMapper _skeletonMapperB;

    // Low-pass filters that remove jitter from bone rotations in the SkeletonPoses.
    private SkeletonPoseFilter _filterA;
    private SkeletonPoseFilter _filterB;

    // True if skeleton of models should be visualized for debugging.
    private bool _drawModelSkeletons;


    public SkeletonMappingSample(Game game)
      : base(game)
    {      
    }


    protected override void LoadContent()
    {
      InitializeModels();

      _filterA = new SkeletonPoseFilter(_skeletonPoseA);
      _filterB = new SkeletonPoseFilter(_skeletonPoseB);

      InitializeSkeletonMappers();

      UpdateDisplayMessage();

      base.LoadContent();
    }


    private void InitializeModels()
    {
      // Load the two different 3D human models.
      // The models use our custom SkinnedModelProcessor as the content processor! 
      // This content processor stores the model's skeleton in the additional data of the model.

      _modelA = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_modelA.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPoseA = SkeletonPose.Create(skeleton);
      
      _modelB = Game.Content.Load<Model>("PlayerMarine");
      additionalData = (Dictionary<string, object>)_modelB.Tag;
      skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPoseB = SkeletonPose.Create(skeleton);
    }


    private void InitializeSkeletonMappers()
    {
      // Create a SkeletonMapper for each model. 
      // In this sample, the models on the screen should act like a mirror for the players'
      // movements. Therefore, we mirror the skeletons, e.g. the right Kinect arm controls left 
      // model arm.

      //
      // ----- SkeletonMapper for the Dude model.
      //
      _skeletonMapperA = new SkeletonMapper(KinectWrapper.SkeletonPoseA, _skeletonPoseA);
      var ks = KinectWrapper.SkeletonPoseA.Skeleton;
      var ms = _skeletonPoseA.Skeleton;

      // So far _skeletonMapperA does nothing. We have to configure how bones or bone chains
      // from the Kinect skeleton should map to the Dude skeleton. This is done using 
      // BoneMappers:      
      // A DirectBoneMapper transfers the rotation and scale of a single bone.
      // Note: The Kinect does not produce bone rotations - only translations. Therefore, this
      // bone mapper only transfers the translation of the Kinect "HipCenter" bone to the 
      // Dude's "Root" bone. 
      _skeletonMapperA.BoneMappers.Add(new DirectBoneMapper(ks.GetIndex("HipCenter"), ms.GetIndex("Root"))
      {
        MapTranslations = true,
        ScaleAToB = 1f,           // TODO: Make this scale factor configurable.
      });

      // An UpperBackBoneMapper is a special bone mapper that is specifically designed for
      // spine bones. It uses the spine, neck and shoulders to compute the rotation of the spine
      // bone. This rotations is transferred to the Dude's "Spine" bone. 
      // (An UpperBackBoneMapper does not transfer bone translations.)
      _skeletonMapperA.BoneMappers.Add(new UpperBackBoneMapper(
        ks.GetIndex("Spine"), ks.GetIndex("ShoulderCenter"), ks.GetIndex("ShoulderLeft"), ks.GetIndex("ShoulderRight"),
        ms.GetIndex("Spine"), ms.GetIndex("Neck"), ms.GetIndex("R_UpperArm"), ms.GetIndex("L_UpperArm")));

      // A ChainBoneMapper transfers the rotation of a bone chain. In this case, it rotates
      // the Dude's "R_UpperArm" bone. It makes sure that the direction from the Dude's
      // "R_Forearm" bone origin to the "R_UpperArm" origin is parallel, to the direction
      // "ElbowLeft" to "ShoulderLeft" of the Kinect skeleton.
      // (An ChainBoneMapper does not transfer bone translations.)
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ShoulderLeft"), ks.GetIndex("ElbowLeft"), ms.GetIndex("R_UpperArm"), ms.GetIndex("R_Forearm")));

      // And so on...
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ShoulderRight"), ks.GetIndex("ElbowRight"), ms.GetIndex("L_UpperArm"), ms.GetIndex("L_Forearm")));
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ElbowLeft"), ks.GetIndex("WristLeft"), ms.GetIndex("R_Forearm"), ms.GetIndex("R_Hand")));
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ElbowRight"), ks.GetIndex("WristRight"), ms.GetIndex("L_Forearm"), ms.GetIndex("L_Hand")));
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("HipLeft"), ks.GetIndex("KneeLeft"), ms.GetIndex("R_Thigh"), ms.GetIndex("R_Knee")));
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("HipRight"), ks.GetIndex("KneeRight"), ms.GetIndex("L_Thigh1"), ms.GetIndex("L_Knee2")));
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("KneeLeft"), ks.GetIndex("AnkleLeft"), ms.GetIndex("R_Knee"), ms.GetIndex("R_Ankle")));
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("KneeRight"), ks.GetIndex("AnkleRight"), ms.GetIndex("L_Knee2"), ms.GetIndex("L_Ankle1")));
      _skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ShoulderCenter"), ks.GetIndex("Head"), ms.GetIndex("Neck"), ms.GetIndex("Head")));

      // We could also try to map the hand bones - but the Kinect input for the hands jitters a lot. 
      // It looks better if we do not animate the hands.
      //_skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("WristLeft"), ks.GetIndex("HandLeft"), ms.GetIndex("R_Hand"), ms.GetIndex("R_Middle1")));
      //_skeletonMapperA.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("WristRight"), ks.GetIndex("HandRight"), ms.GetIndex("L_Hand"), ms.GetIndex("L_Middle1")));

      //
      // ----- SkeletonMapper for the Marine model.
      //
      // (Same as for the Dude - only different bone names.)
      _skeletonMapperB = new SkeletonMapper(KinectWrapper.SkeletonPoseB, _skeletonPoseB);
      ks = KinectWrapper.SkeletonPoseB.Skeleton;
      ms = _skeletonPoseB.Skeleton;
      _skeletonMapperB.BoneMappers.Add(new DirectBoneMapper(ks.GetIndex("HipCenter"), ms.GetIndex("Spine_0"))
      {
        MapTranslations = true,
        ScaleAToB = 1f,             // TODO: Make this scale factor configurable.
      });
      _skeletonMapperB.BoneMappers.Add(new UpperBackBoneMapper(
        ks.GetIndex("Spine"), ks.GetIndex("ShoulderCenter"), ks.GetIndex("ShoulderLeft"), ks.GetIndex("ShoulderRight"),
        ms.GetIndex("Spine1"), ms.GetIndex("Neck"), ms.GetIndex("R_Arm"), ms.GetIndex("L_Arm")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ShoulderLeft"), ks.GetIndex("ElbowLeft"), ms.GetIndex("R_Arm"), ms.GetIndex("R_Elbow")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ShoulderRight"), ks.GetIndex("ElbowRight"), ms.GetIndex("L_Arm"), ms.GetIndex("L_Elbow")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ElbowLeft"), ks.GetIndex("WristLeft"), ms.GetIndex("R_Elbow"), ms.GetIndex("R_Hand")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ElbowRight"), ks.GetIndex("WristRight"), ms.GetIndex("L_Elbow"), ms.GetIndex("L_Hand")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("HipLeft"), ks.GetIndex("KneeLeft"), ms.GetIndex("R_Hip"), ms.GetIndex("R_Knee")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("HipRight"), ks.GetIndex("KneeRight"), ms.GetIndex("L_Hip"), ms.GetIndex("L_Knee")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("KneeLeft"), ks.GetIndex("AnkleLeft"), ms.GetIndex("R_Knee"), ms.GetIndex("R_Ankle")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("KneeRight"), ks.GetIndex("AnkleRight"), ms.GetIndex("L_Knee"), ms.GetIndex("L_Ankle")));
      _skeletonMapperB.BoneMappers.Add(new ChainBoneMapper(ks.GetIndex("ShoulderCenter"), ks.GetIndex("Head"), ms.GetIndex("Neck"), ms.GetIndex("Head")));
    }



    public override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // Map the _kinectSkeletonPoses of tracked players to the _modelSkeletonPoses.
      if (KinectWrapper.IsTrackedA)
      {
        _skeletonMapperA.MapAToB();
        _filterA.Update(deltaTime);
      }
      if (KinectWrapper.IsTrackedA)
      {
        _skeletonMapperB.MapAToB();
        _filterB.Update(deltaTime);
      }

      // <1> --> Toggle drawing of model skeletons.
      if (InputService.IsPressed(Keys.D1, false))
      {
        _drawModelSkeletons = !_drawModelSkeletons;
        UpdateDisplayMessage();
      }

      // <2> --> Increase filter strength.
      if (InputService.IsDown(Keys.D2))
      {
        _filterA.TimeConstant += 0.05f * deltaTime;
        _filterB.TimeConstant = _filterA.TimeConstant;
        UpdateDisplayMessage();
      }

      // <3> --> Decrease filter strength.
      if (InputService.IsDown(Keys.D3))
      {
        _filterA.TimeConstant = Math.Max(0, _filterA.TimeConstant - 0.05f * deltaTime);
        _filterB.TimeConstant = _filterA.TimeConstant;
        UpdateDisplayMessage();
      }

      base.Update(gameTime);
    }    


    private void UpdateDisplayMessage()
    {
      DisplayMessage = "Skeleton Mapping Sample"
                       + "\nPress <1> to toggle drawing of the model skeletons (green): " + _drawModelSkeletons
                       + "\nPress <2>/<3> to increase/decrease the filter strength: " + _filterA.TimeConstant;
    }


    protected override void OnDrawSample(GameTime gameTime)
    {
      // Draw models of tracked players.
      if (KinectWrapper.IsTrackedA)
        DrawSkinnedModel(_modelA, Pose.Identity, _skeletonPoseA);
      if (KinectWrapper.IsTrackedB)
        DrawSkinnedModel(_modelB, Pose.Identity, _skeletonPoseB);

      // Draw model skeletons of tracked players for debugging.
      if (_drawModelSkeletons)
      {
        if (KinectWrapper.IsTrackedA)
          _skeletonPoseA.DrawBones(GraphicsDevice, BasicEffect, 0.1f, SpriteBatch, SpriteFont, Color.GreenYellow);
        if (KinectWrapper.IsTrackedB)
          _skeletonPoseB.DrawBones(GraphicsDevice, BasicEffect, 0.1f, SpriteBatch, SpriteFont, Color.DarkGreen);
      }

      base.OnDrawSample(gameTime);
    }
  }
}
