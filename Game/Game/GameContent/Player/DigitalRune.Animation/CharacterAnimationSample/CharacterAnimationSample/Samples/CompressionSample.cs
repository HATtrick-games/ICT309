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
  // This sample shows how to compress a SkeletonKeyFrameAnimation.
  // The uncompressed and the compressed animations are played on two models side-by-side
  // to visually compare the compression results.
  public class CompressionSample : Sample
  {
    private Model _model;
    private SkeletonPose _skeletonPoseUncompressed;
    private SkeletonPose _skeletonPoseCompressed;

    private float _rotationCompressionThreshold = 0.01f;
    private SkeletonKeyFrameAnimation _animation;


    public CompressionSample(Game game)
      : base(game)
    {
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;

      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPoseUncompressed = SkeletonPose.Create(skeleton);
      _skeletonPoseCompressed = SkeletonPose.Create(skeleton);

      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
      _animation = animations.Values.First();

      RestartAnimations();
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      // <Up>/<Down> --> Increase/Decrease rotation threshold and recreate animations.
      if (InputService.IsPressed(Keys.Up, true))
      {
        _rotationCompressionThreshold += 0.01f;
        RestartAnimations();
      }
      if (InputService.IsPressed(Keys.Down, true))
      {
        _rotationCompressionThreshold -= 0.01f;
        if (_rotationCompressionThreshold < 0)
          _rotationCompressionThreshold = 0;

        RestartAnimations();
      }
      
      base.Update(gameTime);
    }


    private void RestartAnimations()
    {
      // Start original animation on one model.
      var loopingAnimation = new AnimationClip<SkeletonPose>(_animation)
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };
      AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_skeletonPoseUncompressed);      

      // Clone the original animation.
      var animationCompressed = _animation.Clone();
      
      // Compress animation. This removes key frames that can be computed from neighboring frames.
      // This animation is lossy and the parameters define the allowed error.
      float removedKeyFrames = animationCompressed.Compress(0.01f, _rotationCompressionThreshold, 0.001f);

      // Finalize the SkeletonKeyFrameAnimation. 
      // (This must be called to optimize the internal data structures.)
      animationCompressed.Freeze();

      // Start compressed animation on the other model.
      var loopingAnimationCompressed = new AnimationClip<SkeletonPose>(animationCompressed)
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };
      AnimationService.StartAnimation(loopingAnimationCompressed, (IAnimatableProperty)_skeletonPoseCompressed);

      DisplayMessage = "CompressionSample\n"
                       + "The sample shows how to compress a SkeletonKeyFrameAnimation to safe memory.\n"
                       + "The left dude plays the original animation. The right dude plays the compressed animation.\n"
                       + "Press <Up> or <Down> to increase or decrease the allowed compression error.\n"
                       + "Rotation Compression Threshold [°]: " + _rotationCompressionThreshold + "\n"
                       + "Compression: " + removedKeyFrames * 100 + "%";
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      DrawSkinnedModel(_model, new Pose(new Vector3F(-0.5f, 0, 0), Matrix33F.CreateRotationY(ConstantsF.Pi)), _skeletonPoseUncompressed);
      DrawSkinnedModel(_model, new Pose(new Vector3F(0.5f, 0, 0), Matrix33F.CreateRotationY(ConstantsF.Pi)), _skeletonPoseCompressed);

      base.Draw(gameTime);
    }
  }
}
