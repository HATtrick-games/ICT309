using DigitalRune.Animation;
using DigitalRune.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AnimationSample
{
  // Common base class of all samples.
  public abstract class Sample : DrawableGameComponent
  {
    // The help text that is drawn when F1 is pressed.
    private const string Text = "Press <Esc> to exit\n"
                              + "Press <Left Arrow> or <Right Arrow> to select sample\n";


    // Services and resources that can be accessed by derived classes.
    protected IInputService InputService { get; private set; }
    protected IAnimationService AnimationService { get; private set; }
    protected SpriteBatch SpriteBatch { get; private set; }
    protected SpriteFont SpriteFont { get; private set; }
    protected Texture2D Logo { get; private set; }

    // A message that is printed on top of the screen.
    protected string DisplayMessage { get; set;  }

    
    protected Sample(Game game) : base(game)
    {
      InputService = (IInputService)game.Services.GetService(typeof(IInputService));
      AnimationService = (IAnimationService)game.Services.GetService(typeof(IAnimationService));
    }


    protected override void LoadContent()
    {
      SpriteBatch = new SpriteBatch(GraphicsDevice);
      SpriteFont = Game.Content.Load<SpriteFont>("SpriteFont1");
      Logo = Game.Content.Load<Texture2D>("Logo");

      base.LoadContent();
    }
    

    public override void Draw(GameTime gameTime)
    {
      if (Enabled)
      {
        if (DisplayMessage != null)
        {
          SpriteBatch.Begin();

          // Draw display message.
          float left = GraphicsDevice.Viewport.TitleSafeArea.Left + 20;
          float top = GraphicsDevice.Viewport.TitleSafeArea.Top + 20;
          Vector2 position = new Vector2(left, top);
          SpriteBatch.DrawString(SpriteFont, DisplayMessage, position, Color.Black);

          // Draw help text.
          left = GraphicsDevice.Viewport.TitleSafeArea.Left + 20;
          top = GraphicsDevice.Viewport.TitleSafeArea.Bottom - 80;
          position = new Vector2(left, top);
          SpriteBatch.DrawString(SpriteFont, Text, position, Color.Black);

          SpriteBatch.End();
        }
      }

      base.Draw(gameTime);
    }
  }
}
