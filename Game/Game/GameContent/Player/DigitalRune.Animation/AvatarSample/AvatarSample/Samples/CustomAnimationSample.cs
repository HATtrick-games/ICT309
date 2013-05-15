using System;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AvatarSample
{
  // This sample shows how to use custom animations loaded from the content pipeline.
  // The animation are processed using a custom processor: See AvatarAnimationProcessor.cs
  //
  // The user can start animations. After the animation has finished, the avatar returns
  // to the stand animation.
  // In this sample, we keep the AnimationController instances that are returned by
  // AnimationService.StartAnimation(). The AnimationControllers are used to check which 
  // animation is playing.
  public class CustomAnimationSample : Sample
  {
    private AvatarDescription _avatarDescription;
    private AvatarRenderer _avatarRenderer;
    private AvatarPose _avatarPose;
    private Pose _pose = new Pose(new Vector3F(-0.5f, 0, 0));

    private TimelineClip _standAnimation;
    private AnimationController _standAnimationController;
    
    private TimelineGroup _faintAnimation;
    private TimelineGroup _jumpAnimation;
    private TimelineGroup _kickAnimation;
    private TimelineGroup _punchAnimation;
    private AnimationController _actionAnimationController;

    private TimelineClip _walkAnimation;
    private AnimationController _walkAnimationController;


    public CustomAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "CustomAnimationSample\n"
                     + "This sample shows how to use animations loaded from the content pipeline.\n"
                     + "Press <A>, <B>, <X>, <Y> to start animations.\n"
                     + "Press <Left Trigger> to start a walk animation and control the walk speed.";
    }


    protected override void LoadContent()
    {
      _avatarDescription = AvatarDescription.CreateRandom();
      _avatarRenderer = new AvatarRenderer(_avatarDescription);

      // Wrap the Stand0 AvatarAnimationPreset (see WrappedAnimationSample) to create an
      // infinitely looping stand animation.
      AvatarAnimation standAnimationPreset = new AvatarAnimation(AvatarAnimationPreset.Stand0);
      TimelineGroup standAnimation = new TimelineGroup
      {
        new WrappedExpressionAnimation(standAnimationPreset),
        new WrappedSkeletonAnimation(standAnimationPreset),
      };
      _standAnimation = new TimelineClip(standAnimation)
      {
        LoopBehavior = LoopBehavior.Cycle,  // Cycle the Stand animation...
        Duration = TimeSpan.MaxValue,       // ...forever.
      };

      // Load animations from content pipeline.
      _faintAnimation = Game.Content.Load<TimelineGroup>("Faint");
      _jumpAnimation = Game.Content.Load<TimelineGroup>("Jump");
      _kickAnimation = Game.Content.Load<TimelineGroup>("Kick");
      _punchAnimation = Game.Content.Load<TimelineGroup>("Punch");
      
      // The walk cycle should loop: Put it into a timeline clip and set a
      // loop-behavior.
      TimelineGroup walkAnimation = Game.Content.Load<TimelineGroup>("Walk");
      _walkAnimation = new TimelineClip(walkAnimation)
      {
        LoopBehavior = LoopBehavior.Cycle,  // Cycle the Walk animation...
        Duration = TimeSpan.MaxValue,       // ...forever.
      };
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (_avatarPose == null)
      {
        if (_avatarRenderer.State == AvatarRendererState.Ready)
        {
          _avatarPose = new AvatarPose(_avatarRenderer);

          // Start stand animation.
          _standAnimationController = AnimationService.StartAnimation(_standAnimation, _avatarPose);
          _standAnimationController.AutoRecycle();
        }
      }
      else
      {
        // When the user presses buttons, we cross-fade to the custom animations.
        if (InputService.IsPressed(Buttons.A, false, PlayerIndex.One))
        {
          _actionAnimationController = AnimationService.StartAnimation(
            _jumpAnimation,
            _avatarPose,
            AnimationTransitions.Replace(TimeSpan.FromSeconds(0.3)));

          _actionAnimationController.AutoRecycle();
        }
        if (InputService.IsPressed(Buttons.B, false, PlayerIndex.One))
        {
          _actionAnimationController = AnimationService.StartAnimation(
            _punchAnimation,
            _avatarPose,
            AnimationTransitions.Replace(TimeSpan.FromSeconds(0.3)));

          _actionAnimationController.AutoRecycle();
        }
        if (InputService.IsPressed(Buttons.X, false, PlayerIndex.One))
        {
          _actionAnimationController = AnimationService.StartAnimation(
            _kickAnimation,
            _avatarPose,
            AnimationTransitions.Replace(TimeSpan.FromSeconds(0.3)));

          _actionAnimationController.AutoRecycle();
        }
        if (InputService.IsPressed(Buttons.Y, false, PlayerIndex.One))
        {
          _actionAnimationController = AnimationService.StartAnimation(
            _faintAnimation,
            _avatarPose,
            AnimationTransitions.Replace(TimeSpan.FromSeconds(0.3)));

          _actionAnimationController.AutoRecycle();
        }

        // The left trigger controls the speed of the walk cycle.
        float leftTrigger = Math.Abs(InputService.GetGamePadState(PlayerIndex.One).Triggers.Left);
        _walkAnimationController.Speed = leftTrigger * 2;
        if (_walkAnimationController.State != AnimationState.Playing)
        {
          // The walk cycle is not playing. 
          // --> Start walk animation if left trigger is pressed.
          if (leftTrigger > 0)
          {
            _walkAnimationController = AnimationService.StartAnimation(
              _walkAnimation,
              _avatarPose,
              AnimationTransitions.Replace(TimeSpan.FromSeconds(0.3)));

            _walkAnimationController.AutoRecycle();
          }
        }
        else 
        {
          // The walk cycle is playing. 
          // --> Cross-fade to stand animation if left trigger is not pressed.
          if (leftTrigger == 0)
          {
            _standAnimationController = AnimationService.StartAnimation(
              _standAnimation,
              _avatarPose,
              AnimationTransitions.Replace(TimeSpan.FromSeconds(0.3)));

            _standAnimationController.AutoRecycle();
          }
        }

        // If none of the animations is playing, then restart the stand animation.
        if (_standAnimationController.State != AnimationState.Playing
           && _actionAnimationController.State != AnimationState.Playing
           && _walkAnimationController.State != AnimationState.Playing)
        {
          _standAnimationController = AnimationService.StartAnimation(
              _standAnimation,
              _avatarPose,
              AnimationTransitions.Replace(TimeSpan.FromSeconds(0.3)));

          _standAnimationController.AutoRecycle();
        }
      }
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);

      if (_avatarPose != null)
      {
        _avatarRenderer.World = _pose;
        _avatarRenderer.View = Camera.View;
        _avatarRenderer.Projection = Camera.Projection;
        _avatarRenderer.Draw(_avatarPose);
      }

      base.Draw(gameTime);
    }
  }
}
