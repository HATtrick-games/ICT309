using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CharacterAnimationSample
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
    protected Texture2D WhiteTexture { get; private set; }  // A 1x1 white texture.

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

      WhiteTexture = new Texture2D(GraphicsDevice, 1, 1);
      WhiteTexture.SetData(new [] { Color.White });

      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      if (Enabled)
      {
        SpriteBatch.Begin();

        // Draw display message.
        float left = MathHelper.Max(GraphicsDevice.Viewport.TitleSafeArea.Left, 20);
        float top = MathHelper.Max(GraphicsDevice.Viewport.TitleSafeArea.Top, 20);
        Vector2 position = new Vector2(left, top);
        SpriteBatch.DrawStringOutlined(SpriteFont, DisplayMessage, position, Color.Black, Color.White);

        SpriteBatch.End();
      }

      base.Draw(gameTime);
    }


    // Helper method that draws a model with SkinnedEffects.
    protected void DrawSkinnedModel(Model model, Pose pose, SkeletonPose skeletonPose)
    {
      foreach (ModelMesh mesh in model.Meshes)
      {
        foreach (SkinnedEffect effect in mesh.Effects)
        {
          // SkeletonPose.SkinningMatricesXna provides an array of transformations as needed
          // by the SkinnedEffect.
          effect.SetBoneTransforms(skeletonPose.SkinningMatricesXna);

          // The world space transformation.
          effect.World = pose;

          // Camera transformation.
          effect.View = Camera.View;
          effect.Projection = Camera.Projection;

          // Lighting.
          effect.EnableDefaultLighting();
          effect.SpecularColor = new Vector3(0.25f);
          effect.SpecularPower = 16;
        }

        mesh.Draw();
      }
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
