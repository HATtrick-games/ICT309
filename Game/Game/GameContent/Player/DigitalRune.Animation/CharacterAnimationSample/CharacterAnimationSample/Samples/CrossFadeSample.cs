using System;
using System.Collections.Generic;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample shows how to cross-fade between animations.
  public class CrossFadeSample : Sample
  {
    private Model _model;
    private Pose _pose = Pose.Identity;
    private SkeletonPose _skeletonPose;

    private ITimeline _idleAnimation;
    private AnimationController _idleAnimationController;
    private ITimeline _runAnimation;
    private AnimationController _runAnimationController;
    private TimelineGroup _aimAndShootAnimation;
    private AnimationController _aimAndShootAnimationController;


    public CrossFadeSample(Game game)
      : base(game)
    {
      DisplayMessage = "CrossFadeSample\n"
                     + "This sample shows how to cross-fade between animations.\n"
                     + "Press <Space> to play 'Shoot' animation.\n"
                     + "Press <Up> to play 'Run' animation.";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("PlayerMarine");
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);
      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];

      // Create a looping 'Idle' animation.
      _idleAnimation = new AnimationClip<SkeletonPose>(animations["Idle"])
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };

      // Create a looping 'Run' animation.
      _runAnimation = new AnimationClip<SkeletonPose>(animations["Run"])
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };

      // Combine the 'Aim' and 'Shoot' animation. The 'Aim' animation should start immediately. 
      // The 'Shoot' animation should start after 0.3 seconds.
      // (Animations can be combined by creating timeline groups. All timelines/animations 
      // in a timeline group are played simultaneously. AnimationClips can be used to 
      // arrange animations on a timeline. The property Delay, for example, can be used to
      // set the begin time.)
      _aimAndShootAnimation = new TimelineGroup();
      _aimAndShootAnimation.Add(animations["Aim"]);
      _aimAndShootAnimation.Add(new AnimationClip<SkeletonPose>(animations["Shoot"]) { Delay = TimeSpan.FromSeconds(0.3) });

      // Start 'Idle' animation. We use a Replace transition with a fade-in.
      _idleAnimationController = AnimationService.StartAnimation(
        _idleAnimation,
        (IAnimatableProperty)_skeletonPose,
        AnimationTransitions.Replace(TimeSpan.FromSeconds(0.5)));

      _idleAnimationController.AutoRecycle();

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      // <Space> --> Cross-fade to 'Aim-and-Shoot' animation.
      if (InputService.IsPressed(Keys.Space, false))
      {
        // Start the new animation using a Replace transition with a fade-in time.
        _aimAndShootAnimationController = AnimationService.StartAnimation(
          _aimAndShootAnimation,
          (IAnimatableProperty)_skeletonPose,
          AnimationTransitions.Replace(TimeSpan.FromSeconds(0.1)));

        _aimAndShootAnimationController.AutoRecycle();
      }

      // <Up> --> Cross-fade to 'Run' animation - unless the 'Aim-and-Shoot' animation is playing
      // or the 'Run' animation is already playing.
      if (_aimAndShootAnimationController.State != AnimationState.Playing
          && _runAnimationController.State != AnimationState.Playing
          && InputService.IsDown(Keys.Up))
      {
        _runAnimationController = AnimationService.StartAnimation(
          _runAnimation,
          (IAnimatableProperty)_skeletonPose,
          AnimationTransitions.Replace(TimeSpan.FromSeconds(0.2)));

        _runAnimationController.AutoRecycle();
      }

      if (_aimAndShootAnimationController.State != AnimationState.Playing)
      {
        // If none of the animations are playing, or if the user releases the <Up> key,
        // then restart the 'Idle' animation.
        if (_runAnimationController.State != AnimationState.Playing && _idleAnimationController.State != AnimationState.Playing
            || _runAnimationController.State == AnimationState.Playing && InputService.IsUp(Keys.Up))
        {
          _idleAnimationController = AnimationService.StartAnimation(
            _idleAnimation,
            (IAnimatableProperty)_skeletonPose,
            AnimationTransitions.Replace(TimeSpan.FromSeconds(0.2)));

          _idleAnimationController.AutoRecycle();
        }
      }

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
