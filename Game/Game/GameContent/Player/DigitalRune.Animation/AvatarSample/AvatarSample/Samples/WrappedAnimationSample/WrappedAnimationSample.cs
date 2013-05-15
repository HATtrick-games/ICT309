using System;
using DigitalRune;
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
  // This sample shows how to wrap the XNA AvatarAnimation class to use the animations
  // with the DigitalRune Animation system. (See files WrappedExpressionAnimation.cs 
  // and WrappedSkeletonAnimation.cs.)
  //
  // When the user presses a button the animation fades to the next AvatarAnimationPreset.
  public class WrappedAnimationSample : Sample
  {
    private AvatarDescription _avatarDescription;
    private AvatarRenderer _avatarRenderer;

    private AvatarAnimationPreset _currentAvatarAnimationPreset;
    private AvatarPose _avatarPose;
    private Pose _pose = new Pose(new Vector3F(-0.5f, 0, 0));
    private string _currentAnimationText = string.Empty;


    public WrappedAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "WrappedAnimationSample\n"
                     + "This sample shows how to wrap the XNA AvatarAnimation class to use the AvatarAnimation\n"
                     + "with the AnimationService.\n"
                     + "Press <A> to cross-fade to next preset.\n"
                     + "Press <B> to cross-fade to previous preset.";
    }


    protected override void LoadContent()
    {
      _avatarDescription = AvatarDescription.CreateRandom();
      _avatarRenderer = new AvatarRenderer(_avatarDescription);
      
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

          // Start the first animation.
          var wrappedAnimation = WrapAnimation(_currentAvatarAnimationPreset);
          AnimationService.StartAnimation(wrappedAnimation, _avatarPose).AutoRecycle();

          _currentAnimationText = "Current Animation: " + _currentAvatarAnimationPreset;
        }
      }
      else
      {
        if (InputService.IsPressed(Buttons.A, false, PlayerIndex.One))
        {
          // Switch to next preset.
          _currentAvatarAnimationPreset++;
          if (!Enum.IsDefined(typeof(AvatarAnimationPreset), _currentAvatarAnimationPreset))
            _currentAvatarAnimationPreset = 0;

          // Cross-fade to new animation.
          var wrappedAnimation = WrapAnimation(_currentAvatarAnimationPreset);
          AnimationService.StartAnimation(wrappedAnimation, 
                                          _avatarPose, 
                                          AnimationTransitions.Replace(TimeSpan.FromSeconds(0.5))
                                         ).AutoRecycle();

          _currentAnimationText = "Current Animation: " + _currentAvatarAnimationPreset;
        }

        if (InputService.IsPressed(Buttons.B, false, PlayerIndex.One))
        {
          // Switch to previous preset.
          _currentAvatarAnimationPreset--;
          if (!Enum.IsDefined(typeof(AvatarAnimationPreset), _currentAvatarAnimationPreset))
            _currentAvatarAnimationPreset = (AvatarAnimationPreset)EnumHelper.GetValues(typeof(AvatarAnimationPreset)).Length - 1;

          // Cross-fade to new animation.
          var wrappedAnimation = WrapAnimation(_currentAvatarAnimationPreset);
          AnimationService.StartAnimation(wrappedAnimation, 
                                          _avatarPose, 
                                          AnimationTransitions.Replace(TimeSpan.FromSeconds(0.5))
                                         ).AutoRecycle();

          _currentAnimationText = "Current Animation: " + _currentAvatarAnimationPreset;
        }
      }
    }


    public ITimeline WrapAnimation(AvatarAnimationPreset preset)
    {
      // Return a timeline group that contains one animation for the Expression and one
      // animation for the SkeletonPose.
      var avatarAnimation = new AvatarAnimation(_currentAvatarAnimationPreset);
      var expressionAnimation = new WrappedExpressionAnimation(avatarAnimation);
      var skeletonAnimation = new WrappedSkeletonAnimation(avatarAnimation);
      return new TimelineGroup
      {
        expressionAnimation,
        skeletonAnimation,
      };
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);

      if (_avatarPose != null)
      {
        // Draw avatar.
        _avatarRenderer.World = _pose;
        _avatarRenderer.View = Camera.View;
        _avatarRenderer.Projection = Camera.Projection;
        _avatarRenderer.Draw(_avatarPose);
      }

      // Draw string with currently selected animation preset.
      Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;
      SpriteBatch.Begin();
      SpriteBatch.DrawStringOutlined(SpriteFont, _currentAnimationText, new Vector2(bounds.Left, bounds.Top + 100), Color.Black, Color.White);
      SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
