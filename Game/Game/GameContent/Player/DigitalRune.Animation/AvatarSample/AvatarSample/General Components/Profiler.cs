using DigitalRune.Diagnostics;
using DigitalRune.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AvatarSample
{
  // Displays profiling data if the conditional compilation symbol DIGITALRUNE_PROFILE is defined.
  // The HierarchicalProfiler instance is stored in the static field 'MainProfiler'.
  public class Profiler : DrawableGameComponent
  {
    public static HierarchicalProfiler MainProfiler = new HierarchicalProfiler("Main");

    private readonly IInputService _inputService;
    private SpriteBatch _spriteBatch;
    private SpriteFont _spriteFont;
    private string _text = string.Empty;


    public Profiler(Game game)
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


    public override void Update(GameTime gameTime)
    {
#if DIGITALRUNE_PROFILE
      if (_inputService.IsPressed(Keys.H, false) || _inputService.IsPressed(Buttons.LeftShoulder, false, PlayerIndex.One))
      {
        // Get a profiler dump.
        _text = MainProfiler.Dump(null, 10);
      }
#endif

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
#if DIGITALRUNE_PROFILE
      if (_inputService.IsDown(Keys.H) || _inputService.IsDown(Buttons.LeftShoulder, PlayerIndex.One))
      {
        // Print statistics.

        GraphicsDevice.Clear(Color.White);
        
        float right = GraphicsDevice.Viewport.TitleSafeArea.Left + 20;
        float top = GraphicsDevice.Viewport.TitleSafeArea.Top + 20;
        Vector2 position = new Vector2(right, top);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(_spriteFont, _text, position, Color.Black);
        _spriteBatch.End();
      }
#endif

      base.Draw(gameTime);
    }
  }
}