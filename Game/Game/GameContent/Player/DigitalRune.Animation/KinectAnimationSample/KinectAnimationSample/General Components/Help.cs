using DigitalRune.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace KinectAnimationSample
{
  // Displays help text.
  // The component draws a short text when F1 is not pressed. The full help text 
  // is drawn when F1 is pressed.
  public class Help : DrawableGameComponent
  {
    // The help text that is drawn when F1 is pressed.
    private const string Text = "Help Text\n"
      + "----------------------------------------------------\n"
      + "Gamepad:\n"
      + "  Press <Start> to display this help text.\n"
      + "  Press <Back> to exit.\n"
      + "  Use <Left Stick> and <Right Stick> to display this help text.\n"
      + "  Press <DPad Up> or <DPad Down> to to move up/down.\n"
      + "  Press <DPad Left> or <DPad Right> to select sample.\n"
      + "  Press <Right Stick> to reset camera position.\n"
      + "\n"
      + "Keyboard/Mouse:\n"
      + "  Press <F1> to display this help text.\n"
      + "  Press <Esc> to exit.\n"
      + "  Use <W>, <A>, <S>, <D> and mouse to move camera.\n"
      + "  Press <R>, <F> to move up/down.\n"
      + "  Press <Left Arrow> or <Right Arrow> to select sample.\n"
      + "  Press <Home> to reset camera position.\n"
      + "\n";

    private readonly IInputService _inputService;
    private SpriteBatch _spriteBatch;
    private SpriteFont _spriteFont;


    public Help(Game game)
      : base(game)
    {
      _inputService = (IInputService)game.Services.GetService(typeof(IInputService));
    }


    protected override void LoadContent()
    {
      _spriteBatch = new SpriteBatch(GraphicsDevice);
      _spriteFont = Game.Content.Load<SpriteFont>("SpriteFont1");

      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      if (_inputService.IsDown(Keys.F1) || _inputService.IsDown(Buttons.Start, PlayerIndex.One))
      {
        // F1 is pressed:

        // Clear screen.
        GraphicsDevice.Clear(Color.White);

        // Draw help text.
        float left = MathHelper.Max(GraphicsDevice.Viewport.TitleSafeArea.Left, 20);
        float top = MathHelper.Max(GraphicsDevice.Viewport.TitleSafeArea.Top, 20);
        Vector2 position = new Vector2(left, top);
        _spriteBatch.Begin();      
        _spriteBatch.DrawString(_spriteFont, Text, position, Color.Black);
        _spriteBatch.End();
      }
      else
      {
        // F1 is not pressed:

        // Draw a help hint at the bottom of the screen.
        float left = MathHelper.Max(GraphicsDevice.Viewport.TitleSafeArea.Left, 20);
        float bottom = MathHelper.Min(GraphicsDevice.Viewport.TitleSafeArea.Bottom - 20, GraphicsDevice.Viewport.Height - 30);
        Vector2 position = new Vector2(left, bottom);
        const string text = "Press <F1> or <Start> to display Help";
        _spriteBatch.Begin();
        _spriteBatch.DrawString(_spriteFont, text, position, Color.White);
        _spriteBatch.End();
      }

      base.Draw(gameTime);
    }
  }
}