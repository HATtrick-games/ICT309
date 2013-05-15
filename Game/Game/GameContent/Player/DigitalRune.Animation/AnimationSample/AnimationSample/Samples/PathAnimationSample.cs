using DigitalRune.Animation;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Mathematics.Interpolation;
using Microsoft.Xna.Framework;
using CurveLoopType = DigitalRune.Mathematics.Interpolation.CurveLoopType;


namespace AnimationSample
{
  // This sample uses a curve-based animation to move a sprite on a 2D path.
  public class PathAnimationSample : Sample
  {
    private AnimatableProperty<Vector2F> _animatablePosition = new AnimatableProperty<Vector2F>();
      
      
    public PathAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "PathAnimationSample\n"
                     + "A sprite is moving on a path using a curve-based animation.";
    }
      
      
    protected override void LoadContent()
    {
      Rectangle bounds = GraphicsDevice.Viewport.Bounds;

      // Create a 2D path.
      Path2F path = new Path2F
      {
        // Path is cyclic.
        PreLoop = CurveLoopType.Cycle,   
        PostLoop = CurveLoopType.Cycle,

        //  End of path should smoothly interpolate with start of path.
        SmoothEnds = true,
      };
      
      // The spline type.
      SplineInterpolation splineInterpolation = SplineInterpolation.BSpline;

      // Add path keys. The parameter of a path key is the time in seconds.
      path.Add(new PathKey2F
      {
        Parameter = 0,
        Point = new Vector2F(bounds.Center.X, bounds.Center.Y),
        Interpolation = splineInterpolation,        
      });
      path.Add(new PathKey2F
      {
        Parameter = 0.5f,
        Point = new Vector2F(bounds.Center.X / 2.0f, 2.0f * bounds.Center.Y / 3.0f),
        Interpolation = splineInterpolation,
      });
      path.Add(new PathKey2F
      {
        Parameter = 1.0f,
        Point = new Vector2F(bounds.Center.X, 1.0f * bounds.Center.Y / 3.0f),
        Interpolation = splineInterpolation,
      });
      path.Add(new PathKey2F
      {
        Parameter = 1.5f,
        Point = new Vector2F(3.0f * bounds.Center.X / 2.0f, 2.0f * bounds.Center.Y / 3.0f),
        Interpolation = splineInterpolation,
      });
      path.Add(new PathKey2F
      {
        Parameter = 2.0f,
        Point = new Vector2F(bounds.Center.X, bounds.Center.Y),
        Interpolation = splineInterpolation,
      });
      path.Add(new PathKey2F
      {
        Parameter = 2.5f,
        Point = new Vector2F(bounds.Center.X / 2.0f, 4.0f * bounds.Center.Y / 3.0f),
        Interpolation = splineInterpolation,
      });

      path.Add(new PathKey2F
      {
        Parameter = 3.0f,
        Point = new Vector2F(bounds.Center.X, 5.0f * bounds.Center.Y / 3.0f),
        Interpolation = splineInterpolation,
      });
      path.Add(new PathKey2F
      {
        Parameter = 3.5f,
        Point = new Vector2F(3.0f * bounds.Center.X / 2.0f, 4.0f * bounds.Center.Y / 3.0f),
        Interpolation = splineInterpolation,
      });
      path.Add(new PathKey2F
      {
        Parameter = 4.0f,
        Point = new Vector2F(bounds.Center.X, bounds.Center.Y),
        Interpolation = splineInterpolation,
      });

      // Create a path animation using the path.
      // (Start at parameter value 0 and loop forever.)
      Path2FAnimation pathAnimation = new Path2FAnimation(path)
      {
        StartParameter = 0,
        EndParameter = float.PositiveInfinity,
      };

      AnimationService.StartAnimation(pathAnimation, _animatablePosition);

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
