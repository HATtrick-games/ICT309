using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.Constraints;
using DigitalRune.Physics.Specialized;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

// Type Skeleton exists in DigitalRune Animation and in the Kinect SDK.
using DRSkeleton = DigitalRune.Animation.Character.Skeleton;
using KinectSkeleton = Microsoft.Kinect.Skeleton;


namespace KinectAnimationSample
{
  // This sample animates the Dude character model using Kinect.
  // To animate the Dude, a "Marionette" approach is used: A ragdoll is created for the model
  // that is used to animated the model. Then we use selected joint positions (hands, elbows, 
  // neck, knees, ankles) of the Kinect player skeleton as target positions. With BallJoint
  // constraints we pull ragdoll bodies to the target positions. These BallJoints act as
  // the "strings" of our marionette. 
  // The pelvis body is kinematic and is positioned directly at the Kinect HipCenter position.
  //
  // Only the first player in front of the Kinect sensor is used. (No model for the second
  // player.)
  //
  // Since the skeleton of the Dude model is very different from a usual Kinect player
  // skeleton, it is hard to make this approach stable. - This sample is only quick proof-of-
  // concept. To get better results, we need a model that has a similar bone hierarchy and the
  // same proportions as the Kinect player skeleton. 
  // The constraint parameters (MaxForce, ErrorReduction, Softness) of the marionette 
  // constraints and the ragdoll constraints could also deserve more tweaking.
  // We could also add more constraints to stabilize the ragdoll.
  public class RagdollMarionetteSample : SampleBase
  {
    // The Dude character model.
    private Model _model;
    // The ragdoll of the Dude model.
    private Ragdoll _ragdoll; 
    // A skeleton that is used to animate the Dude. This skeleton is updated by the ragdoll bodies.
    private SkeletonPose _skeletonPose;
    
    // "Marionette constraints":
    // BallJoints constraints that pull important joints of the ragdoll to the target positions
    // All these joints will use a small MaxForce because these constraints should be weaker than 
    // the normal ragdoll joints and limits.
    private BallJoint _headSpring;
    private BallJoint _elbowLeftSpring;
    private BallJoint _handLeftSpring;
    private BallJoint _elbowRightSpring;
    private BallJoint _handRightSpring;
    private BallJoint _kneeLeftSpring;
    private BallJoint _ankleLeftSpring;
    private BallJoint _kneeRightSpring;
    private BallJoint _ankleRightSpring;

    private bool _drawModel = true;
    private bool _drawModelSkeleton;
    private bool _drawConstraints;


    public RagdollMarionetteSample(Game game)
      : base(game)
    {
      UpdateDisplayMessage();      
    }


    protected override void LoadContent()
    {
      InitializeModel();      
      InitializeMarionetteConstraints();

      base.LoadContent();
    }


    private void InitializeModel()
    {
      // Load dude model including the skeleton.
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var ragdollSkeleton = (DRSkeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(ragdollSkeleton);

      // Create a ragdoll for the Dude model.
      _ragdoll = new Ragdoll();
      DudeRagdollCreator.Create(_skeletonPose, _ragdoll, Simulation, 0.57f);

      // Set the world space pose of the whole ragdoll. And copy the bone poses of the
      // current skeleton pose.
      _ragdoll.UpdateBodiesFromSkeleton(_skeletonPose);

      // Disable sleeping.
      foreach (var body in _ragdoll.Bodies)
      {
        if (body != null)
          body.CanSleep = false;
      }

      // The pelvis bone (index 1) is updated directly from the Kinect hip center.
      _ragdoll.Bodies[1].MotionType = MotionType.Kinematic;

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
          motor.ConstraintDamping = 100;
          motor.ConstraintSpring = 0;
        }
      }
      _ragdoll.EnableMotors();

      // Add rigid bodies and the constraints of the ragdoll to the simulation.
      _ragdoll.AddToSimulation(Simulation);      
    }


    private void InitializeMarionetteConstraints()
    {
      // Create constraints that pull important body parts to Kinect joint positions.
      // The Update() method below will update the BallJoint.AnchorPositionALocal vectors.

      // Limit the maximal forces that these joints can apply. We do not want these joints to be
      // so strong that they can violate the ragdoll joint and limit constraints. Increasing
      // this force makes the ragdoll more responsive but can also violate ragdoll constraints
      // (e.g. by stretching the limbs).
      float maxForce = 1000;

      var ragdollSkeleton = _skeletonPose.Skeleton;

      _headSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("Head")],
        MaxForce = maxForce,
      };
      Simulation.Constraints.Add(_headSpring);
      _elbowLeftSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("L_Forearm")],
        MaxForce = maxForce / 2,    // Elbow springs are weaker because the correct 
                                    // hand position is more important and the hand 
                                    // constraint should therefore be stronger.
      };
      // This constraint should be attached at the elbow position and not at the center of the forearm:
      var elbowLeftJointPosition = _skeletonPose.GetBonePoseAbsolute(ragdollSkeleton.GetIndex("L_Forearm")).Translation;
      _elbowLeftSpring.AnchorPositionBLocal = _elbowLeftSpring.BodyB.Pose.ToLocalPosition(elbowLeftJointPosition);
      Simulation.Constraints.Add(_elbowLeftSpring);
      
      _handLeftSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("L_Hand")],
        MaxForce = maxForce,
      };
      Simulation.Constraints.Add(_handLeftSpring);
      
      _elbowRightSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("R_Forearm")],
        MaxForce = maxForce / 2,
      };
      // This constraint should be attached at the elbow position and not at the center of the forearm:
      var elbowRightJointPosition = _skeletonPose.GetBonePoseAbsolute(ragdollSkeleton.GetIndex("R_Forearm")).Translation;
      _elbowRightSpring.AnchorPositionBLocal = _elbowRightSpring.BodyB.Pose.ToLocalPosition(elbowRightJointPosition);
      Simulation.Constraints.Add(_elbowRightSpring);
      
      _handRightSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("R_Hand")],
        MaxForce = maxForce,
      };
      Simulation.Constraints.Add(_handRightSpring);

      _kneeLeftSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("L_Knee2")],
        MaxForce = maxForce,
      };
      // This constraint should be attached at the knee position and not at the center of the lower leg:
      var kneeLeftJointPosition = _skeletonPose.GetBonePoseAbsolute(ragdollSkeleton.GetIndex("L_Knee2")).Translation;
      _kneeLeftSpring.AnchorPositionBLocal = _kneeLeftSpring.BodyB.Pose.ToLocalPosition(kneeLeftJointPosition);
      Simulation.Constraints.Add(_kneeLeftSpring);
      
      _ankleLeftSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("L_Ankle1")],
        MaxForce = maxForce,
      };
      Simulation.Constraints.Add(_ankleLeftSpring);

      _kneeRightSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("R_Knee")],
        MaxForce = maxForce,
      };
      // This constraint should be attached at the knee position and not at the center of the lower leg:
      var kneeRightJointPosition = _skeletonPose.GetBonePoseAbsolute(ragdollSkeleton.GetIndex("R_Knee")).Translation;
      _kneeRightSpring.AnchorPositionBLocal = _kneeRightSpring.BodyB.Pose.ToLocalPosition(kneeRightJointPosition);
      Simulation.Constraints.Add(_kneeRightSpring);
      
      _ankleRightSpring = new BallJoint
      {
        BodyA = Simulation.World,
        BodyB = _ragdoll.Bodies[ragdollSkeleton.GetIndex("R_Ankle")],
        MaxForce = maxForce,
      };
      Simulation.Constraints.Add(_ankleRightSpring);
    }


    public override void Update(GameTime gameTime)
    {
      CorrectWorldSpacePose();

      // Update the skeleton bone transforms from the current rigid body positions.
      _ragdoll.UpdateSkeletonFromBodies(_skeletonPose);

      // Since not all joints are constrained by the Kinect skeleton, we might want 
      // to show some bones always in a specific pose if they move too much.
      _skeletonPose.SetBoneTransform(_skeletonPose.Skeleton.GetIndex("Head"), SrtTransform.Identity);
      
      if (KinectWrapper.IsTrackedA)
      {
        // The Kinect position (0, 0, 0) is at the Kinect sensor and not on the floor. In this
        // sample the floor is at height 0. Therefore, we add a vertical offset to the Kinect 
        // positions.
        var offset = new Vector3F(0, KinectSensorHeight, 0);

        // The new pelvis position. (We keep the original rotation and use the Kinect position.)
        var kinectSkeletonPose = KinectWrapper.SkeletonPoseA;
        var newPose = new Pose(kinectSkeletonPose.GetBonePoseAbsolute(0).Translation + offset)
                      * new Pose(_skeletonPose.Skeleton.GetBindPoseRelative(1).Rotation.Inverse);

        // If the new position is too far away from the last position, then we limit the 
        // position change. If the ragdoll makes very large jumps in a single step, then 
        // it could get tangled up.
        var oldPose = _ragdoll.Bodies[1].Pose; // Pelvis has bone index 1.
        var translation = newPose.Position - oldPose.Position;
        var maxTranslation = 0.1f;
        if (translation.Length > maxTranslation)
        {
          translation.Length = maxTranslation;
          newPose.Position = oldPose.Position + translation;
        }        
        _ragdoll.Bodies[1].Pose = newPose;

        // ----- Update the target positions for the animation joints.        

        var kinectHeadPosition = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.Head).Translation + offset;
        // The ragdoll torso cannot bend sideways. Correct target position to avoid sideway pull.
        kinectHeadPosition.X = newPose.Position.X;
        _headSpring.AnchorPositionALocal = kinectHeadPosition;
        
        // In this sample, the model on the screen should act like a mirror for the players'
        // movements. Therefore, we mirror the skeletons, e.g. the right Kinect arm controls left 
        // model arm.
        _elbowLeftSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.ElbowRight).Translation + offset;
        _handLeftSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.HandRight).Translation + offset;
        _elbowRightSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.ElbowLeft).Translation + offset;
        _handRightSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.HandLeft).Translation + offset;
        _kneeLeftSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.KneeRight).Translation + offset;
        _ankleLeftSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.AnkleRight).Translation + offset;
        _kneeRightSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.KneeLeft).Translation + offset;
        _ankleRightSpring.AnchorPositionALocal = kinectSkeletonPose.GetBonePoseAbsolute((int)JointType.AnkleLeft).Translation + offset;
      }

      // <1> --> Toggle drawing of model.
      if (InputService.IsPressed(Keys.D1, false))
      {
        _drawModel = !_drawModel;
        UpdateDisplayMessage();
      }

      // <2> --> Toggle drawing of model skeleton.
      if (InputService.IsPressed(Keys.D2, false))
      {
        _drawModelSkeleton = !_drawModelSkeleton;
        UpdateDisplayMessage();
      }

      // <3> --> Toggle drawing of ragdoll bodies.
      if (InputService.IsPressed(Keys.D3, false))
      {
        RigidBodyRenderer.Enabled = !RigidBodyRenderer.Enabled;
        UpdateDisplayMessage();
      }

      // <4> --> Toggle drawing of ragdoll constraints.
      if (InputService.IsPressed(Keys.D4, false))
      {
        _drawConstraints = !_drawConstraints;
        UpdateDisplayMessage();
      }
      
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
    }


    private void UpdateDisplayMessage()
    {
      DisplayMessage = "Marionette Ragdoll Sample"
        + "\nPress <1> to toggle drawing of the model: " + _drawModel
        + "\nPress <2> to toggle drawing of the model skeleton: " + _drawModelSkeleton
        + "\nPress <3> to toggle drawing of the ragdoll: " + RigidBodyRenderer.Enabled
        + "\nPress <4> to toggle drawing of the joint limits: " + _drawConstraints;
    }


    protected override void OnDrawSample(GameTime gameTime)
    {
      // Draw animated model.
      if (_drawModel)
        DrawSkinnedModel(_model, _ragdoll.Pose, _skeletonPose);

      // Draw model skeletons of tracked players for debugging.
      if (_drawModelSkeleton)
      {
        BasicEffect.World = _ragdoll.Pose;
        _skeletonPose.DrawBones(GraphicsDevice, BasicEffect, 0.1f, SpriteBatch, SpriteFont, Color.GreenYellow);
      }

      // Draw joint limits for debugging.
      if (_drawConstraints)
      {
        GraphicsDevice.DepthStencilState = DepthStencilState.None;
        RagdollHelper.DrawConstraints(_ragdoll, GraphicsDevice, BasicEffect, 0.1f);
      }

      base.OnDrawSample(gameTime);
    }


    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean-up physics simulation.
        Simulation.RigidBodies.Clear();

        RigidBodyRenderer.Enabled = true;
      }

      base.Dispose(disposing);
    }
  }
}
