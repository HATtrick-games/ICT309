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
  // This sample shows how to play a looping walk animation.
  // The user can increase/decrease the animation speed.
  public class DudeWalkingSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0), Matrix33F.CreateRotationY(ConstantsF.Pi));
    private SkeletonPose _skeletonPose;

    private AnimationController _animationController; 


    public DudeWalkingSample(Game game)
      : base(game)
    {
      DisplayMessage = "DudeWalkingSample\n"
                       + "This sample shows how to play a looping walk animation.\n"
                       + "Press <Up>/<Down> to increase/decrease the animation speed";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("Soldier");
      
      var additionalData = (Dictionary<string, object>)_model.Tag;

      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      // Get the animations from the additional data.
      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];

      // The Dude model contains only one animation, which is a SkeletonKeyFrameAnimation with 
      // a walk cycle.
      SkeletonKeyFrameAnimation walkAnimation = animations.Values.First();

      // Wrap the walk animation in an animation clip that loops the animation forever.
      AnimationClip<SkeletonPose> loopingAnimation = new AnimationClip<SkeletonPose>(walkAnimation)
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };

      // Start the animation and keep the created AnimationController.
      // We must cast the SkeletonPose to IAnimatableProperty because SkeletonPose implements
      // IAnimatableObject and IAnimatableProperty. We must tell the AnimationService if we want
      // to animate an animatable property of the SkeletonPose (IAnimatableObject), or if we want to
      // animate the whole SkeletonPose (IAnimatableProperty).
      _animationController = AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_skeletonPose);

      // The animation will be applied the next time AnimationManager.ApplyAnimations() is called
      // in the main loop. ApplyAnimations() is called before this method is called, therefore
      // the model will be rendered in the bind pose in this frame and in the first animation key
      // frame in the next frame - this creates an annoying visual popping effect. 
      // We can avoid this if we call AnimationController.UpdateAndApply(). This will immediately
      // change the model pose to the first key frame pose.
      _animationController.UpdateAndApply();

      // (Optional) Enable Auto-Recycling: 
      // After the animation is stopped, the animation service will recycle all
      // intermediate data structures. 
      _animationController.AutoRecycle();

      

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
       
      // Increase animation speed when <Up> is pressed.
      if (InputService.IsPressed(Keys.Up, true))
        _animationController.Speed += 0.2f;

      // Decrease animation speed when <Down> is pressed.
      if (InputService.IsPressed(Keys.Down, true))
        _animationController.Speed -= 0.2f;

      // Clamp the speed to 0 if negative.
      if (_animationController.Speed < 0)
        _animationController.Speed = 0;

      //_pose.Position = new Vector3F(0.5f, 1000, 0);
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
