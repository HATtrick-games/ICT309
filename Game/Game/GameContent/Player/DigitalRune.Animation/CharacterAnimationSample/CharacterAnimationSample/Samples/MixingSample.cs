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
  // This sample shows how to mix animations on one skeleton. 
  // The lower body parts play an 'Idle' or a 'Run' animation. The upper body parts play a 
  // 'Shoot' animation.
  public class MixingSample : Sample
  {
    private Model _model;
    private Pose _pose = Pose.Identity;
    private SkeletonPose _skeletonPose;

    private AnimationClip<SkeletonPose> _idleAnimation;
    private AnimationClip<SkeletonPose> _runAnimation;
    private AnimationController _runAnimationController;
    private AnimationController _idleAnimationController;

    private bool _isRunning;

    

    public MixingSample(Game game)
      : base(game)
    {
      DisplayMessage = "MixingSample\n"
                       + "This sample shows how to mix animations on one skeleton.\n"
                       + "The lower body parts play the 'Idle' animation or 'Run' animation.\n"
                       + "The upper body plays the 'Shoot' animation.\n"
                       + "Press <Up> to play 'Run' animation.\n";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("PlayerMarine");
      var additionalData = (Dictionary<string, object>)_model.Tag;

      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];

      _runAnimation = new AnimationClip<SkeletonPose>(animations["Run"])
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };

      _idleAnimation = new AnimationClip<SkeletonPose>(animations["Idle"])
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };

      // Create a 'Shoot' animation that only affects the upper body.
      var shootAnimation = animations["Shoot"];

      // The SkeletonKeyFrameAnimations allows to set a weight for each bone channel. 
      // For the 'Shoot' animation, we set the weight to 0 for all bones that are 
      // not descendants of the second spine bone (bone index 2). That means, the 
      // animation affects only the upper body bones and is disabled on the lower 
      // body bones.
      for (int i = 0; i < skeleton.NumberOfBones; i++)
      {
        if (!SkeletonHelper.IsAncestorOrSelf(_skeletonPose, 2, i))
          shootAnimation.SetWeight(i, 0);
      }

      var loopedShootingAnimation = new AnimationClip<SkeletonPose>(shootAnimation)
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };

      // Start 'Idle' animation.
      _idleAnimationController = AnimationService.StartAnimation(_idleAnimation, (IAnimatableProperty)_skeletonPose);
      _idleAnimationController.AutoRecycle();

      // Start looping the 'Shoot' animation. We use a Compose transition. This will add the 
      // 'Shoot' animation to the animation composition chain and keeping all other playing 
      // animations.
      // The 'Idle' animation animates the whole skeleton. The 'Shoot' animation replaces 
      // the 'Idle' animation on the bones of the upper body.
      AnimationService.StartAnimation(
        loopedShootingAnimation, 
        (IAnimatableProperty)_skeletonPose,
        AnimationTransitions.Compose())
        .AutoRecycle();
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      if (InputService.IsDown(Keys.Up))
      {
        if (!_isRunning)
        {
          _isRunning = true;

          // Start 'Run' animation. We use a Replace transition and replace the 'Idle' 
          // animation which is the first in the animation composition chain. Since we only
          // replace one specific animation, the 'Shoot' animation will stay in the composition
          // chain and keep playing.
          _runAnimationController = AnimationService.StartAnimation(
            _runAnimation, 
            (IAnimatableProperty)_skeletonPose,
            AnimationTransitions.Replace(_idleAnimationController.AnimationInstance, TimeSpan.FromSeconds(0.3)));
          _runAnimationController.AutoRecycle();
        }
      }
      else
      {
        if (_isRunning)
        {
          _isRunning = false;

          // Start 'Idle' animation and replace the 'Run' animation.
          _idleAnimationController = AnimationService.StartAnimation(
             _idleAnimation,
             (IAnimatableProperty)_skeletonPose,
             AnimationTransitions.Replace(_runAnimationController.AnimationInstance, TimeSpan.FromSeconds(0.3)));
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
