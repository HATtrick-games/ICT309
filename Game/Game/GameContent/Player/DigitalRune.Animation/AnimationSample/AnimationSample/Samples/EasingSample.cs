using System;
using DigitalRune.Animation;
using DigitalRune.Animation.Easing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AnimationSample
{
  // This sample demonstrates the easing functions. An easing function is a function y=f(x),
  // where x and y are in the range [0, 1]. The easing functions are used to create a transition 
  // from a start value to a target value in a from/to/by animations.
  // The sample lets the user select the easing function and easing mode. This easing function
  // is used in a from/to animation that moves a sprite horizontally.
  public class EasingSample : Sample
  {
    private AnimatableProperty<float> _animatableFloat = new AnimatableProperty<float>();

    private EasingFunction[] _easingFunctions;
    private int _selectedEasingFunctionIndex;
    private EasingMode _selectedEasingMode;
    private SingleFromToByAnimation _fromToAnimation;
    private AnimationController _animationController;


    public EasingSample(Game game)
      : base(game)
    {
      DisplayMessage = "EasingSample\n"
                     + "This sample demonstrates various easing functions.\n"
                     + "Press <Up>/<Down> to select easing function.\n"
                     + "Press <1> to use EaseIn.\n"
                     + "Press <2> to use EaseOut.\n"
                     + "Press <3> to use EaseInOut.\n";
    }


    protected override void LoadContent()
    {
      base.LoadContent();

      // Initialize array of easing functions.
      _easingFunctions = new EasingFunction[]
       {
         null,
         new BackEase { Amplitude = 0.5f },
         new BounceEase { Bounces = 3, Bounciness = 3 },
         new CircleEase(), 
         new CubicEase(), 
         new ElasticEase { Oscillations = 3, Springiness = 10 }, 
         new ExponentialEase(), 
         new LogarithmicEase(),
         new HermiteEase(), 
         new PowerEase { Power = 2 }, 
         new QuadraticEase(), 
         new QuinticEase(), 
         new SineEase(), 
       };


      // Create and start a horizontal from/to animation.
      Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;
      _fromToAnimation = new SingleFromToByAnimation
      {
        From = bounds.Left + 200,
        To = bounds.Right - 200,
        Duration = TimeSpan.FromSeconds(1.5),
        EasingFunction = _easingFunctions[_selectedEasingFunctionIndex],
      };
      _animationController = AnimationService.StartAnimation(_fromToAnimation, _animatableFloat);
    }


    public override void Update(GameTime gameTime)
    {
      // <UP> key --> Select previous easing function.
      if (InputService.IsPressed(Keys.Up, true))
      {
        _selectedEasingFunctionIndex--;
        
        if (_selectedEasingFunctionIndex < 0)
          _selectedEasingFunctionIndex = _easingFunctions.Length - 1;
      }

      // <DOWN> key --> Select next easing function.
      if (InputService.IsPressed(Keys.Down, true))
      {
        _selectedEasingFunctionIndex++;
        if (_selectedEasingFunctionIndex >= _easingFunctions.Length)
          _selectedEasingFunctionIndex = 0;
      }

      // <1>, <2>, <3> --> Select easing mode.
      if (InputService.IsPressed(Keys.D1, false))
      {
        _selectedEasingMode = EasingMode.EaseIn;
      }
      else if (InputService.IsPressed(Keys.D2, false))
      {
        _selectedEasingMode = EasingMode.EaseOut;
      }
      else if (InputService.IsPressed(Keys.D3, false))
      {
        _selectedEasingMode = EasingMode.EaseInOut;
      }

      if (_animationController.State == AnimationState.Filling)
      {
        // The current animation has finished - it is now holding the last animation 
        // value because the fill behavior is set to 'Hold' by default.
        // (_fromToAnimation.FillBehavior == FillBehavior.Hold).

        // Stop the animation.
        _animationController.Stop();

        // Switch the From and To values of the horizontal animation to move the sprite back
        // to the other screen side.
        Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;
        if (_animatableFloat.Value < bounds.Center.X)
        {
          _fromToAnimation.From = bounds.Left + 200;
          _fromToAnimation.To = bounds.Right - 200;
        }
        else
        {
          _fromToAnimation.From = bounds.Right - 200;
          _fromToAnimation.To = bounds.Left + 200;
        }

        // Set easing funciton.
        _fromToAnimation.EasingFunction = _easingFunctions[_selectedEasingFunctionIndex];

        // Set easing mode.
        EasingFunction currentEasingFunction = _fromToAnimation.EasingFunction as EasingFunction;
        if (currentEasingFunction != null)
          currentEasingFunction.Mode = _selectedEasingMode;

        // Start the new animation.
        _animationController.Start();
      }

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.White);

      Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;
      EasingFunction easingFunction = _easingFunctions[_selectedEasingFunctionIndex];

      SpriteBatch.Begin();

      // Draw name of easing function.
      SpriteBatch.DrawString(
        SpriteFont,
        easingFunction != null ? easingFunction.GetType().Name : "None",
        new Vector2(bounds.Center.X - 100, bounds.Center.Y - 100),
        Color.Black);

      // Draw name of easing mode.
      SpriteBatch.DrawString(
        SpriteFont,
        _selectedEasingMode.ToString(),
        new Vector2(bounds.Center.X - 100, bounds.Center.Y - 80),
        Color.Black);

      // Draw sprite.
      SpriteBatch.Draw(
        Logo,
        new Vector2(_animatableFloat.Value, bounds.Center.Y) - new Vector2(Logo.Width, Logo.Height) / 2.0f,
        Color.Red);
      SpriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
