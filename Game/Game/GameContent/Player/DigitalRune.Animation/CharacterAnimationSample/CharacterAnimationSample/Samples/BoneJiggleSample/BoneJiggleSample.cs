using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample shows how to make a bone-based jiggle effect. 
  // Here, the jiggle effect is applied to the head bone - but it should also be useful for
  // other jiggle effects. ;-)
  public class BoneJiggleSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(Matrix33F.CreateRotationY(ConstantsF.Pi));
    private SkeletonPose _skeletonPose;

    private AnimationController _walkAnimationController;

    private BoneJiggler _boneJiggler;


    public BoneJiggleSample(Game game)
      : base(game)
    {
      DisplayMessage = "BoneJiggleSample\n"
                     + "This sample shows how to apply a bone-based jiggle effect.\n"
                     + "Press <Space> to pause/resume the animation and watch the head.\n"
                     + "Press <Back> to reset the jiggle effect.";
    }


    protected override void LoadContent()
    {
      // Load model and start a looping animation.
      _model = Game.Content.Load<Model>("Dude");

      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
      var walkAnimation = new AnimationClip<SkeletonPose>(animations.Values.First())
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };
      _walkAnimationController = AnimationService.StartAnimation(walkAnimation, (IAnimatableProperty)_skeletonPose);
      _walkAnimationController.AutoRecycle();

      // Create a BoneJiggler instance for the head bone (bone index 7).
      _boneJiggler = new BoneJiggler(_skeletonPose, 7, new Vector3F(1.1f, 0, 0))
      {
        Spring = 100,
        Damping = 3,
      };

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // <Space> --> Pause/Resume animation.
      if (InputService.IsPressed(Keys.Space, false))
      {
        if (_walkAnimationController.IsPaused)
          _walkAnimationController.Resume();
        else
          _walkAnimationController.Pause();
      }

      // <Back> --> Reset BoneJiggler.
      if (InputService.IsDown(Keys.Back))
        _boneJiggler.Reset();

      // Update BoneJiggler. This will change the bone transform of the affected bone.
      _boneJiggler.Update(deltaTime, _pose);
      
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
