using DigitalRune.Animation;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;


namespace AnimationSample
{
  // This sample shows how to use custom animations. See also MyCircleAnimation.cs
  public class CustomAnimationSample : Sample
  {
    private AnimatableProperty<Vector2F> _animatablePosition = new AnimatableProperty<Vector2F>();
      
      
    public CustomAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "CustomAnimationSample\n"
                     + "This sample uses a custom animation class to create a circular movement.";
    }
      
      

    protected override void LoadContent()
    {
      // Start the custom circle animation.
      AnimationService.StartAnimation(new MyCircleAnimation(), _animatablePosition);

      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.White);

      // Draw sprite centered at the animated position.
      Vector2 position = (Vector2)_animatablePosition.Value - new Vector2(Logo.Width, Logo.Height) / 2.0f;

      SpriteBatch.Begin();
      SpriteBatch.Draw(Logo, position, Color.Red);
      SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
