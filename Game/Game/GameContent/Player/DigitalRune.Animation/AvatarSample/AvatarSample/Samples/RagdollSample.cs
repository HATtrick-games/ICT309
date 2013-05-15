using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace AvatarSample
{
  // This sample shows how to create an ragdoll for an avatar.
  public class RagdollSample : Sample
  {
    private AvatarDescription _avatarDescription;
    private AvatarRenderer _avatarRenderer;
    private AvatarPose _avatarPose;
    private Pose _pose = new Pose(new Vector3F(-0.5f, 0, 0));

    private Ragdoll _ragdoll;


    public RagdollSample(Game game)
      : base(game)
    {
      DisplayMessage = "RagdollSample\n"
                     + "This sample shows how to create an avatar ragdoll.\n"
                     + "Press <Left Trigger> to grab avatar.\n"
                     + "Press <Right Trigger> to shoot a ball.";
    }


    protected override void LoadContent()
    {
      _avatarDescription = AvatarDescription.CreateRandom();
      _avatarRenderer = new AvatarRenderer(_avatarDescription);

      // Add a ground plane in the simulation.
      Simulation.RigidBodies.Add(
        new RigidBody(new PlaneShape(Vector3F.UnitY, 0))
        {
          MotionType = MotionType.Static
        });

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (_avatarPose == null)
      {
        if (_avatarRenderer.State == AvatarRendererState.Ready)
        {
          _avatarPose = new AvatarPose(_avatarRenderer);

          // Create a ragdoll for the avatar.
          _ragdoll = Ragdoll.CreateAvatarRagdoll(_avatarPose, Simulation);

          // Set the world space pose of the whole ragdoll. And copy the bone poses of the
          // current skeleton pose.
          _ragdoll.Pose = _pose;
          _ragdoll.UpdateBodiesFromSkeleton(_avatarPose.SkeletonPose);

          // In this sample we use a passive ragdoll where we need joints to hold the
          // limbs together and limits to control the angular movement.
          _ragdoll.EnableJoints();
          _ragdoll.EnableLimits();

          // Set all motors to constraint motors that only use damping. This adds a damping
          // effect to all ragdoll limbs.
          foreach (RagdollMotor motor in _ragdoll.Motors)
          {
            if (motor != null)
            {
              motor.Mode = RagdollMotorMode.Constraint;
              motor.ConstraintDamping = 5;
              motor.ConstraintSpring = 0;
            }
          }
          _ragdoll.EnableMotors();
          
          // Add rigid bodies and the constraints of the ragdoll to the simulation.
          _ragdoll.AddToSimulation(Simulation);
        }      
      }
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);

      if (_avatarPose != null)
      {
        // Copy skeleton pose from ragdoll.
        _ragdoll.UpdateSkeletonFromBodies(_avatarPose.SkeletonPose);

        // Draw avatar.
        _avatarRenderer.World = _pose;
        _avatarRenderer.View = Camera.View;
        _avatarRenderer.Projection = Camera.Projection;
        _avatarRenderer.Draw(_avatarPose);
      }

      base.Draw(gameTime);
    }
  }
}
