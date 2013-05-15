using System;
using DigitalRune.Animation;
using DigitalRune.Animation.Easing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AnimationSample
{
  // This sample shows how to use additive animations. 
  // A sprite is animated on a horizontal path. A vertical animation is "added" to the horizontal
  // base animation.
  // Additive animations are created by setting the IsAdditive flag (which most animation
  // types have). To add the animation to another running animation, the additive animation must
  // be started using a "Compose" transition. See code below.
  // This sample also shows how AnimationController can be used to pause/resume animations.
  public class AdditiveAnimationSample : Sample
  {
    private AnimatableProperty<Vector2> _animatablePosition = new AnimatableProperty<Vector2>();

    private AnimationController _baseAnimationController;
    private AnimationController _additiveAnimationController;


    public AdditiveAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "AdditiveAnimationSample\n"
                     + "A base animation moves the sprite horizontally. An additive animation moves the sprite vertically.\n"
                     + "Press <1> to pause/resume the base animation (horizontal movement).\n"
                     + "Press <2> to pause/resume the secondary, additive animation (vertical movement).";
    }


    protected override void LoadContent()
    {
      Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;

      // ----- Create and start the base animation.
      Vector2FromToByAnimation leftRightAnimation = new Vector2FromToByAnimation()
      {
        TargetProperty = "Position",
        From = new Vector2(bounds.Left + 100, bounds.Center.Y),
        To = new Vector2(bounds.Right - 100, bounds.Center.Y),
        Duration = TimeSpan.FromSeconds(2),
        EasingFunction = new HermiteEase { Mode = EasingMode.EaseInOut },
      };
      AnimationClip<Vector2> baseAnimation = new AnimationClip<Vector2>(leftRightAnimation)
      {
        LoopBehavior = LoopBehavior.Oscillate,
        Duration = TimeSpan.MaxValue,
      };
      _baseAnimationController = AnimationService.StartAnimation(baseAnimation, _animatablePosition);

      // ----- Create and start the additive animation.
      Vector2FromToByAnimation upDownAnimation = new Vector2FromToByAnimation()
      {
        TargetProperty = "Position",
        From = new Vector2(0, 50),
        To = new Vector2(0, -50),
        Duration = TimeSpan.FromSeconds(0.5),
        EasingFunction = new SineEase { Mode = EasingMode.EaseInOut },

        // Set IsAdditive flag.
        IsAdditive = true,
      };
      AnimationClip<Vector2> additiveAnimation = new AnimationClip<Vector2>(upDownAnimation)
      {
        LoopBehavior = LoopBehavior.Oscillate,
        Duration = TimeSpan.MaxValue,
      };

      // Start animation using "Compose".
      _additiveAnimationController = AnimationService.StartAnimation(
        additiveAnimation, 
        _animatablePosition, 
        AnimationTransitions.Compose());

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (InputService.IsPressed(Keys.D1, false))
      {
        // Pause/resume base animation.

        if (_baseAnimationController.IsPaused)
          _baseAnimationController.Resume();
        else
          _baseAnimationController.Pause();
      }

      if (InputService.IsPressed(Keys.D2, false))
      {
        // Pause/resume additive animation.

        if (_additiveAnimationController.IsPaused)
          _additiveAnimationController.Resume();
        else
          _additiveAnimationController.Pause();
      }
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.White);

      // Draw the sprite centered at the animated position.
      Vector2 position = _animatablePosition.Value - new Vector2(Logo.Width, Logo.Height) / 2.0f;

      SpriteBatch.Begin();
      SpriteBatch.Draw(Logo, position, Color.Blue);
      SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
