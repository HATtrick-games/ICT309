using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MathHelper = Microsoft.Xna.Framework.MathHelper;


namespace AvatarSample
{
  // Draws a cross-hair in the center of the screen.
  public class Reticle : DrawableGameComponent
  {
    private const float CrossHairSize = 0.05f;

    private BasicEffect _basicEffect;
    private readonly VertexPositionColor[] _pointList = new VertexPositionColor[2];


    public Reticle(Game game) : base(game)
    {      
    }


    protected override void LoadContent()
    {
      // Create a basic effect with standard transformation matrices.
      _basicEffect = new BasicEffect(GraphicsDevice)
      {
        LightingEnabled = false, 
        VertexColorEnabled = true,
        World = Matrix.Identity,
        View = Matrix.Identity,
        Projection = Matrix.CreatePerspectiveFieldOfView(
          MathHelper.ToRadians(45),
          GraphicsDevice.Viewport.AspectRatio,
          0.1f,
          1000f)
      };

      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.None;

      foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
      {
        pass.Apply();
        DrawLine(new Vector3F(-CrossHairSize / 2, 0, -1), new Vector3F(CrossHairSize / 2, 0, -1), Color.Black);
        DrawLine(new Vector3F(0, -CrossHairSize / 2, -1), new Vector3F(0, CrossHairSize / 2, -1), Color.Black);
      }

      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      
      base.Draw(gameTime);
    }


    private void DrawLine(Vector3F start, Vector3F end, Color color)
    {
      _pointList[0] = new VertexPositionColor((Vector3)start, color);
      _pointList[1] = new VertexPositionColor((Vector3)end, color);
      GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _pointList, 0, 1);
    }
  }
}