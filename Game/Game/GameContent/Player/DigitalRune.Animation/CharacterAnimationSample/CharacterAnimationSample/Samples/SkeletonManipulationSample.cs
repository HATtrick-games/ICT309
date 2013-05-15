using System;
using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CharacterAnimationSample
{
  // This sample show how to manipulate a skeleton pose directly in code. The arm of 
  // the model is rotated in code.
  public class SkeletonManipulationSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0));
    private SkeletonPose _skeletonPose;

    private float _upperArmAngle;
    private bool _moveArmDown;

    public SkeletonManipulationSample(Game game)
      : base(game)
    {
      DisplayMessage = "SkeletonManipulationSample\n"
                     + "The arm and hand of the model are manipulated directly in the sample code.";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;

      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // Change rotation angle.
      if (_moveArmDown)
        _upperArmAngle -= 0.3f * deltaTime;
      else 
        _upperArmAngle += 0.3f * deltaTime;

      // Change direction when a certain angle is reached.
      if (Math.Abs(_upperArmAngle) > 0.5f)
        _moveArmDown = !_moveArmDown;

      // Get the bone index of the upper arm bone.
      int upperArmIndex = _skeletonPose.Skeleton.GetIndex("L_UpperArm");
      
      // Define the desired bone transform.
      SrtTransform boneTransform = new SrtTransform(QuaternionF.CreateRotationY(_upperArmAngle));

      // Set the new bone transform.
      _skeletonPose.SetBoneTransform(upperArmIndex, boneTransform);

      // The class SkeletonHelper provides some useful extension methods.
      // One is SetBoneRotationAbsolute() which sets the orientation of a bone relative 
      // to model space. 
      int handIndex = _skeletonPose.Skeleton.GetIndex("L_Hand");
      SkeletonHelper.SetBoneRotationAbsolute(_skeletonPose, handIndex, QuaternionF.CreateRotationX(ConstantsF.Pi));

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      DrawSkinnedModel(_model, _pose, _skeletonPose);      

      base.Draw(gameTime);
    }
  }
}
