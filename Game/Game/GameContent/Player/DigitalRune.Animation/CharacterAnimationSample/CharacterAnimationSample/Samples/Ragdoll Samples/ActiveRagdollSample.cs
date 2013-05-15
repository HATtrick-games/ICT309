using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRune.Animation;
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
  // This sample creates an active (=animated) ragdoll.
  // Constraint ragdoll motors are used to move the ragdoll limbs.
  public class ActiveRagdollSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0), QuaternionF.CreateRotationY(ConstantsF.Pi));
    
    // This skeleton pose is animated. It defines the desired pose for the dude.
    private SkeletonPose _targetSkeletonPose;

    // This skeleton pose displays the result of the physics simulation. 
    private SkeletonPose _actualSkeletonPose;

    private Ragdoll _ragdoll;


    public ActiveRagdollSample(Game game)
      : base(game)
    {
      DisplayMessage = "ActiveRagdollSample\n"
                     + "The ragdoll is animated but it also reacts to impacts.\n"
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

      // Load model.
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];

      // Create the two skeleton poses. 
      _targetSkeletonPose = SkeletonPose.Create(skeleton);
      _actualSkeletonPose = SkeletonPose.Create(skeleton);

      // Animate the _targetSkeletonPose.
      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
      var loopingAnimation = new AnimationClip<SkeletonPose>(animations.Values.First())
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };
      AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_targetSkeletonPose);

      // Create a ragdoll for the Dude model.
      _ragdoll = new Ragdoll();
      DudeRagdollCreator.Create(_targetSkeletonPose, _ragdoll, Simulation);

      // Set the world space pose of the whole ragdoll. And copy the bone poses of the
      // current skeleton pose.
      _ragdoll.Pose = _pose;
      _ragdoll.UpdateBodiesFromSkeleton(_targetSkeletonPose);

      // In this sample we use an active ragdoll. We need joints because constraint ragdoll
      // motors only affect the body rotations.
      _ragdoll.EnableJoints();
      
      // We disable limits. If limits are enabled, the ragdoll could get unstable if 
      // the animation tries to move a limb beyond an allowed limit. (This happens if
      // a pose in the animation violates one of our limits.)
      _ragdoll.DisableLimits();

      // Set all motors to constraint motors. Constraint motors are like springs that
      // rotate the limbs to a target position.
      foreach (RagdollMotor motor in _ragdoll.Motors)
      {
        if (motor != null)
        {
          motor.Mode = RagdollMotorMode.Constraint;
          motor.ConstraintDamping = 10000;
          motor.ConstraintSpring = 100000;
        }
      }
      _ragdoll.EnableMotors();

      // Add rigid bodies and the constraints of the ragdoll to the simulation.
      _ragdoll.AddToSimulation(Simulation);
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      CorrectWorldSpacePose();

      // Update the actual skeleton pose which is used during rendering.
      _ragdoll.UpdateSkeletonFromBodies(_actualSkeletonPose);
      
      // Set the new motor targets using the animated pose.
      _ragdoll.DriveToPose(_targetSkeletonPose, deltaTime);

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

      var pelvis = _targetSkeletonPose.Skeleton.GetIndex("Pelvis");
      
      // This is different from the PassiveRagdollSample:
      // We use the _targetSkeletonPose to define the distance between pelvis and the Pose.
      var pelvisBonePoseAbsoluteInverse = _targetSkeletonPose.GetBonePoseAbsolute(pelvis).Inverse;
      
      _ragdoll.Pose = _ragdoll.Bodies[pelvis].Pose * _ragdoll.BodyOffsets[pelvis].Inverse * (Pose)pelvisBonePoseAbsoluteInverse;
      _pose = _ragdoll.Pose;
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      DrawSkinnedModel(_model, _pose, _actualSkeletonPose);

      base.Draw(gameTime);
    }
  }
}
