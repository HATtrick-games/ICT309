using System;
using System.Diagnostics;
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
  // There are a lot of reasons to use the DigitalRune Animation system for animating
  // Xbox LIVE Avatars: Support for animation blending, cross-fading, inverse kinematics, 
  // ragdoll physics, and more.
  // 
  // This sample shows how to convert an XNA AvatarAnimation into an 
  // AvatarExpressionKeyFrameAnimation and a SkeletonKeyFrameAnimation.
  // (Using AvatarExpressionKeyFrameAnimations and SkeletonKeyFrameAnimations 
  // is faster than wrapping an AvatarAnimation as it is done in the 
  // WrappedAnimationSample.)
  public class BakedAnimationSample : Sample
  {
    private AvatarDescription _avatarDescription;
    private AvatarRenderer _avatarRenderer;

    private ITimeline _waveAnimation;
    private AvatarPose _avatarPose;
    private Pose _pose = new Pose(new Vector3F(-0.5f, 0, 0));


    public BakedAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "BakedAnimationSample\n"
                     + "This sample shows how to convert an XNA AvatarAnimation into a SkeletonKeyFrameAnimation.\n"
                     + "Press <A> to restart animation.";
    }


    protected override void LoadContent()
    {
      _avatarDescription = AvatarDescription.CreateRandom();
      _avatarRenderer = new AvatarRenderer(_avatarDescription);

      // Convert animation.
      _waveAnimation = BakeAnimation(new AvatarAnimation(AvatarAnimationPreset.Clap));

      base.LoadContent();
    }


    private ITimeline BakeAnimation(AvatarAnimation avatarAnimation)
    {
      // Create an AvatarExpression key frame animation that will be applied to the Expression
      // property of an AvatarPose.
      AvatarExpressionKeyFrameAnimation expressionAnimation = new AvatarExpressionKeyFrameAnimation
      {
        TargetProperty = "Expression"
      };
      
      // Create a SkeletonPose key frame animation that will be applied to the SkeletonPose
      // property of an AvatarPose.
      SkeletonKeyFrameAnimation skeletonKeyFrameAnimation = new SkeletonKeyFrameAnimation
      {
        TargetProperty = "SkeletonPose"
      };
      
      // In the next loop, we sample the original animation with 30 Hz and store the key frames.
      int numberOfKeyFrames = 0;
      AvatarExpression previousExpression = new AvatarExpression();
      TimeSpan time = TimeSpan.Zero;
      TimeSpan length = avatarAnimation.Length;
      TimeSpan step = new TimeSpan(333333); //  1/30 seconds;
      while (true)
      {
        // Advance time in AvatarAnimation.
        avatarAnimation.CurrentPosition = time;

        // Add expression key frame if this is the first key frame or if the key frame is 
        // different from the last key frame.
        AvatarExpression expression = avatarAnimation.Expression;
        if (time == TimeSpan.Zero || !expression.Equals(previousExpression))
          expressionAnimation.KeyFrames.Add(new KeyFrame<AvatarExpression>(time, expression));
        
        previousExpression = expression;

        // Convert bone transforms to SrtTransforms and add key frames to the SkeletonPose
        // animation.
        for (int i = 0; i < avatarAnimation.BoneTransforms.Count; i++)
        {
          SrtTransform boneTransform = SrtTransform.FromMatrix(avatarAnimation.BoneTransforms[i]);
          skeletonKeyFrameAnimation.AddKeyFrame(i, time, boneTransform);
          numberOfKeyFrames++;
        }

        // Abort if we have arrived at the end time.
        if (time == length)
          break;

        // Increase time. We check that we do not step over the end time. 
        if (time + step > length)
          time = length;
        else
          time += step;
      }

      // Compress animation to save memory.
      float numberOfRemovedKeyFrames = skeletonKeyFrameAnimation.Compress(0.1f, 0.1f, 0.001f);
      Debug.WriteLine("Compression removed " + numberOfRemovedKeyFrames * 100 + "% of the key frames.");

      // Finalize the skeleton key frame animation. This optimizes the internal data structures.
      skeletonKeyFrameAnimation.Freeze();

      return new TimelineGroup
      {
        expressionAnimation,
        skeletonKeyFrameAnimation,
      };
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (_avatarPose == null)
      {
        if (_avatarRenderer.State == AvatarRendererState.Ready)
        {
          _avatarPose = new AvatarPose(_avatarRenderer);
          AnimationService.StartAnimation(_waveAnimation, _avatarPose).AutoRecycle();
        }
      }
      else if (InputService.IsPressed(Buttons.A, false, PlayerIndex.One))
      {
        // Restart animation using a cross-fade of 0.5 seconds.
        AnimationService.StartAnimation(_waveAnimation, 
                                        _avatarPose, 
                                        AnimationTransitions.Replace(TimeSpan.FromSeconds(0.5))
                                       ).AutoRecycle();
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
