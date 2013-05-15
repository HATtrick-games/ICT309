using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CharacterAnimationSample
{
  // This sample creates a passive, damped ragdoll.
  // Constraint ragdoll motors (damping only) are used to create a damping effect, which removes
  // a lot of unwanted jitter.
  public class PassiveRagdollSample : Sample
  {
    private BasicEffect _basicEffect;
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0), QuaternionF.CreateRotationY(ConstantsF.Pi));    
    private SkeletonPose _skeletonPose;
    private Ragdoll _ragdoll;


    public PassiveRagdollSample(Game game)
      : base(game)
    {
      DisplayMessage = "PassiveRagdollSample\n"
                       + "Press <Left Mouse Button> to grab ragdoll.\n"
                       + "Press <Right Mouse Button> to shoot balls.\n";
    }


    protected override void LoadContent()
    {
      // Add a ground plane to the simulation.
      Simulation.RigidBodies.Add(
        new RigidBody(new PlaneShape(Vector3F.UnitY, 0))
        {
          MotionType = MotionType.Static
        });

      _basicEffect = new BasicEffect(GraphicsDevice);

      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      // Create a ragdoll for the Dude model.
      _ragdoll = new Ragdoll();
      DudeRagdollCreator.Create(_skeletonPose, _ragdoll, Simulation);

      // Set the world space pose of the whole ragdoll. And copy the bone poses of the
      // current skeleton pose.
      _ragdoll.Pose = _pose;
      _ragdoll.UpdateBodiesFromSkeleton(_skeletonPose);

      // Uncomment to disable dynamic movement (for debugging during ragdoll creation):
      //foreach (var body in _ragdoll.Bodies)
      //  if (body != null)
      //    body.MotionType = MotionType.Kinematic;

      // In this sample we use a passive ragdoll where we need joints to hold the
      // limbs together and limits to restrict angular movement.
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

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      CorrectWorldSpacePose();

      // Update the skeleton bone transforms from the current rigid body positions.
      _ragdoll.UpdateSkeletonFromBodies(_skeletonPose);
      
      base.Update(gameTime);
    }


    private void CorrectWorldSpacePose()
    {
      // Notes:
      // The Ragdoll class is simply a container for rigid bodies, joints, limits, motors, etc.
      // It has a Ragdoll.Pose property that determines the world space pose of the model.
      // The Ragdoll class does not update this property. It only reads it.
      // Let's say the ragdoll and model are created at the world space origin. Then the user
      // grabs the ragdoll and throws it 100 units away. Then the Ragdoll.Pose (and the root bone)
      // is still at the origin and the first body (the pelvis) is 100 units away. 
      // You can observe this if you comment out this method and look at the debug rendering of 
      // the skeleton.
      // To avoid this we correct the Ragdoll.Pose and make sure that it is always near the 
      // pelvis bone.

      int pelvis = _skeletonPose.Skeleton.GetIndex("Pelvis");
      SrtTransform pelvisBindPoseAbsoluteInverse = _skeletonPose.Skeleton.GetBindPoseAbsoluteInverse(pelvis);
      _ragdoll.Pose = _ragdoll.Bodies[pelvis].Pose * _ragdoll.BodyOffsets[pelvis].Inverse * (Pose)pelvisBindPoseAbsoluteInverse;
      _pose = _ragdoll.Pose;
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      DrawSkinnedModel(_model, _pose, _skeletonPose);

      //// Draw avatar skeleton for debugging.
      //_basicEffect.World = _pose;
      //_basicEffect.View = Camera.View;
      //_basicEffect.Projection = Camera.Projection;
      //SkeletonHelper.DrawBones(_skeletonPose, GraphicsDevice, _basicEffect, 0.1f,
      //                         SpriteBatch, SpriteFont, Color.Orange);


      // Draw constraints for debugging.
      GraphicsDevice.DepthStencilState = DepthStencilState.None;
      _basicEffect.World = Matrix.Identity;
      _basicEffect.View = Camera.View;
      _basicEffect.Projection = Camera.Projection;      
      RagdollHelper.DrawConstraints(_ragdoll, GraphicsDevice, _basicEffect, 0.3f);
                
      base.Draw(gameTime);
    }
  }
}
