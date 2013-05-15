using System;
using DigitalRune.Animation;
using DigitalRune.Animation.Easing;
using Microsoft.Xna.Framework;


namespace AnimationSample
{
  // This sample animates a double value.
  // DigitalRune Animation does not directly support double animations. To add double animations
  // you need to create a DoubleTraits class (see DoubleTraits.cs) that tells the animation 
  // service how double values are created/recycled/added/interpolated/blended/etc.
  // This DoubleTraits class can be used in a custom animation (see DoubleFromToByAnimation.cs).
  public class DoubleAnimationSample : Sample
  {
    private AnimatableProperty<double> _animatableDouble = new AnimatableProperty<double>();


    public DoubleAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "DoubleAnimationSample\n"
                     + "This sample animates a double value.\n"
                     + "DigitalRune Animation does not have built-in support for double animations.\n"
                     + "This sample shows how to add a from/to/by animation for the type double.";
    }
      
      
     protected override void LoadContent()
     {
       Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;
       
       // A 2 second from/to animation.
       DoubleFromToByAnimation doubleAnimation = new DoubleFromToByAnimation
       {
         From = bounds.Left + 200,
         To = bounds.Right - 200,
         Duration = TimeSpan.FromSeconds(2),
         EasingFunction = new QuadraticEase { Mode = EasingMode.EaseInOut },
       };

       // Make the from/to animation oscillate forever.
       AnimationClip<double> oscillatingDoubleAnimation = new AnimationClip<double>(doubleAnimation)
       {
         LoopBehavior = LoopBehavior.Oscillate,
         Duration = TimeSpan.MaxValue,
       };

       AnimationService.StartAnimation(oscillatingDoubleAnimation, _animatableDouble);

       base.LoadContent();
     }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.White);

      // Draw the sprite using the animated value for the x position.
      Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;
      Vector2 position = new Vector2((float)_animatableDouble.Value, bounds.Center.Y) - new Vector2(Logo.Width, Logo.Height) / 2.0f;

      SpriteBatch.Begin();
      SpriteBatch.Draw(Logo, position, Color.Red);
      SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
