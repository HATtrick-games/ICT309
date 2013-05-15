using System.Linq;
using DigitalRune.Game.Input;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // Renders the rigid bodies of the physics simulation for debugging.
  public class RigidBodyRenderer : DrawableGameComponent
  {
    private readonly IInputService _inputService;
    private readonly Simulation _simulation;

    private BasicEffect _basicEffect;
    private bool _showSleeping = true;      // Determines whether sleeping objects should be visualized.
    
    private readonly RasterizerState _wireFrameState = new RasterizerState
    {
      FillMode = FillMode.WireFrame,
    };
    
    private readonly RasterizerState _wireFrameNoCullState = new RasterizerState
    {
      FillMode = FillMode.WireFrame,
      CullMode = CullMode.None,
    };


    // Determines whether wire frame or solid fill mode should be used.
    public bool DrawWireFrame { get; private set; }


    public RigidBodyRenderer(Game game)
      : base(game)
    {
      _inputService = (IInputService)game.Services.GetService(typeof(IInputService));
      _simulation = (Simulation)game.Services.GetService(typeof(Simulation));

      DrawWireFrame = true;
    }


    protected override void LoadContent()
    {
      // Create a basic effect.
      _basicEffect = new BasicEffect(GraphicsDevice);
      _basicEffect.EnableDefaultLighting();
      _basicEffect.PreferPerPixelLighting = true;
      _basicEffect.SpecularColor = new Vector3(0.2f, 0.1f, 0.1f);
      _basicEffect.SpecularPower = 20f;

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      foreach (RigidBody body in _simulation.RigidBodies)
      {
        // We use a DrawableShape to render each rigid body.
        // The DrawableShape is stored in body.UserData.
        DrawableShape drawable = body.UserData as DrawableShape;
        if (drawable == null
            && body.UserData == null)  // If UserData is not available, then the RigidBodyRenderer does not render this body.
        {
          // The DrawableShape does not yet exist.
          // --> Search for another body with the same shape and scale.
          foreach (RigidBody other in _simulation.RigidBodies)
          {
            if (other.Shape == body.Shape && other.Scale == body.Scale && other.UserData is DrawableShape)
            {
              // There is another rigid body with the same shape and size.
              // We can share the DrawableShape between the rigid bodies.
              drawable = (DrawableShape)other.UserData;
              break;
            }
          }

          // Create a new DrawableShape if we haven't found any.
          if (drawable == null)
          {
            drawable = new DrawableShape();
            drawable.LoadContent(GraphicsDevice, body);
          }

          body.UserData = drawable;
        }
      }

      // Toggle between wireframe and normal mode if <M> is pressed.
      if (_inputService.IsPressed(Keys.M, false))
        DrawWireFrame = !DrawWireFrame;

      // If <L> is pressed, render the sleeping (inactive) bodies in a different color.
      if (_inputService.IsPressed(Keys.L, false))
        _showSleeping = !_showSleeping;

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      // 'Camera' is the game component that controls the view.
      Camera camera = Game.Components.OfType<Camera>().First();

      _basicEffect.View = camera.View;
      _basicEffect.Projection = camera.Projection;

      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.BlendState = BlendState.Opaque;

      // Draw the rigid bodies of all registered simulations.
      foreach (RigidBody body in _simulation.RigidBodies)
      {
        DrawableShape drawable = body.UserData as DrawableShape;
        if (drawable != null)
        {
          Color color = Color.Gray;

          // Switch wireframe rendering and backside culling.
          if (DrawWireFrame)
          {
            if (drawable.IsTwoSided)
              GraphicsDevice.RasterizerState = _wireFrameNoCullState;
            else
              GraphicsDevice.RasterizerState = _wireFrameState;
          }
          else
          {
            if (drawable.IsTwoSided)
              GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            else
              GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
          }

          // Static bodies are rendered in light gray.
          // Optional: Sleeping (inactive) bodies are rendered in light gray.
          if (body.MotionType == MotionType.Static || _showSleeping && body.IsSleeping)
            color = Color.LightGray;

          drawable.Draw(GraphicsDevice, _basicEffect, body.Pose, color);
        }
      }

      // Restore original settings.
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

      base.Draw(gameTime);
    }
  }
}