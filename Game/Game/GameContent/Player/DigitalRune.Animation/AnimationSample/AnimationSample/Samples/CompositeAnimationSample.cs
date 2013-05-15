using System;
using DigitalRune.Animation;
using DigitalRune.Animation.Easing;
using Microsoft.Xna.Framework;


namespace AnimationSample
{
  // This sample shows how to animate a Vector2 using two separate float animations for x and y.
  // Composite animations (e.g. Vector2Animation) combine other animations to create an animation
  // of a complex animation value type.
  public class CompositeAnimationSample : Sample
  {
    private AnimatableProperty<Vector2> _animatablePosition = new AnimatableProperty<Vector2>();


    public CompositeAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "CompositeAnimationSample\n"
                     + "The 2D position of the sprite is animated using two separate float animations for x and y.";
    }


    protected override void LoadContent()
    {
      Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;

      // A single from/to animation.
      SingleFromToByAnimation fromToAnimation = new SingleFromToByAnimation
      {
        From = bounds.Top + 200,
        To = bounds.Bottom - 200,
        Duration = TimeSpan.FromSeconds(2),
        EasingFunction = new SineEase { Mode = EasingMode.EaseInOut },
      };

      // Create an animation that oscillates forever.
      AnimationClip<float> loopedSingleAnimationX = new AnimationClip<float>(fromToAnimation)
      {
        LoopBehavior = LoopBehavior.Oscillate,
        Duration = TimeSpan.MaxValue,
      };

      // Create an animation that oscillates forever. The animations starts 1 second into
      // the fromToAnimation - that means, loopedSingleAnimationX is 1 second "behind" this
      // animation.
      AnimationClip<float> loopedSingleAnimationY = new AnimationClip<float>(fromToAnimation)
      {
        LoopBehavior = LoopBehavior.Oscillate,
        Duration = TimeSpan.MaxValue,
        Delay = TimeSpan.FromSeconds(-1),
      };

      // Create a composite animation that combines the two float animations to animate
      // a Vector2 value.
      Vector2Animation compositeAnimation = new Vector2Animation(loopedSingleAnimationX, loopedSingleAnimationY);

      // Start animation.
      AnimationService.StartAnimation(compositeAnimation, _animatablePosition);

      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.White);

      // Draw the sprite centered at the animated position.
      Vector2 position = _animatablePosition.Value - new Vector2(Logo.Width, Logo.Height) / 2.0f;

      SpriteBatch.Begin();
      SpriteBatch.Draw(Logo, position, Color.Gold);
      SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
