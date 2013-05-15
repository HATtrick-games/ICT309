using System.Linq;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // Shoots a ball when a button is pressed.
  public class BallShooter : GameComponent
  {
    private readonly IInputService _inputService;
    private readonly Simulation _simulation;

    // Prepare a number of balls which can be reused as ammunition.
    private RigidBody[] _balls;
    private int _nextBallIndex;


    public BallShooter(Game game)
      : base(game)
    {
      _inputService = (IInputService)game.Services.GetService(typeof(IInputService));
      _simulation = (Simulation)game.Services.GetService(typeof(Simulation));
    }


    public override void Initialize()
    {
      // Prepare 10 balls.
      _balls = new RigidBody[10];
      Shape sphereShape = new SphereShape(0.2f);
      for (int i = 0; i < _balls.Length; i++)
      {
        _balls[i] = new RigidBody(sphereShape)      // Note: All rigid bodies share the same shape.
        {
          // Assign a name. (Just for debugging.)
          Name = "Ball" + i,

          // The balls are shot with a high velocity. We need to enable "Continuous Collision 
          // Detection" - otherwise, we could miss some collision.
          CcdEnabled = true,
        };
      }

      base.Initialize();
    }


    public override void Update(GameTime gameTime)
    {
      // Fire ball if right mouse button or right trigger is pressed.
      if (_inputService.IsPressed(MouseButtons.Right, false) || _inputService.IsPressed(Buttons.RightTrigger, false, PlayerIndex.One))
      {
        RigidBody ball = _balls[_nextBallIndex];

        // Remove ball from physics simulation in case it has already been used.
        if (ball.Simulation != null)
          ball.Simulation.RigidBodies.Remove(ball);

        // Get the forward direction of the camera. ("Forward" is in -z direction.)
        Camera camera = Game.Components.OfType<Camera>().First();
        Pose cameraPose = camera.Pose;
        Vector3F forward = cameraPose.ToWorldDirection(-Vector3F.UnitZ);

        // Place the ball at the position of the camera and shoot forward by directly setting
        // the velocity.
        ball.Pose = cameraPose;
        ball.LinearVelocity = forward * 20;

        // Add the ball to the physics simulation.
        _simulation.RigidBodies.Add(ball);

        // Select next ball in array.
        _nextBallIndex = (_nextBallIndex + 1) % _balls.Length;
      }

      base.Update(gameTime);
    }
  }
}