using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Game.Input;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AvatarSample
{
  // Common base class of all samples.
  public abstract class Sample : DrawableGameComponent
  {
    // Services and resources that can be accessed by the derived classes.
    protected IInputService InputService { get; private set; }
    protected IAnimationService AnimationService { get; private set; }
    protected Simulation Simulation { get; private set; }
    protected SpriteBatch SpriteBatch { get; private set; }
    protected SpriteFont SpriteFont { get; private set; }
    protected Camera Camera { get; private set; }
    protected Model GroundModel { get; private set; }

    // A message that is printed on top of the screen.
    protected string DisplayMessage { get; set; }


    protected Sample(Game game)
      : base(game)
    {
      InputService = (IInputService)game.Services.GetService(typeof(IInputService));
      AnimationService = (IAnimationService)game.Services.GetService(typeof(IAnimationService));
      Simulation = (Simulation)game.Services.GetService(typeof(Simulation));

      Camera = Game.Components.OfType<Camera>().First();
    }


    protected override void LoadContent()
    {
      SpriteBatch = new SpriteBatch(GraphicsDevice);
      SpriteFont = Game.Content.Load<SpriteFont>("SpriteFont1");
      GroundModel = Game.Content.Load<Model>("Ground");

      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      if (Enabled)
      {
        SpriteBatch.Begin();

        // Draw display message.
        float left = GraphicsDevice.Viewport.TitleSafeArea.Left;
        float top = GraphicsDevice.Viewport.TitleSafeArea.Top;
        Vector2 position = new Vector2(left, top);
        SpriteBatch.DrawStringOutlined(SpriteFont, DisplayMessage, position, Color.Black, Color.White);

        SpriteBatch.End();
      }

      base.Draw(gameTime);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean-up physics simulation.
        Simulation.RigidBodies.Clear();
      }

      base.Dispose(disposing);
    }
  }
}
