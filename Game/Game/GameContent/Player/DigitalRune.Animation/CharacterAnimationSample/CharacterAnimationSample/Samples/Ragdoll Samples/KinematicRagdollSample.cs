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
  // This sample shows how to use the Ragdoll class to interact with other rigid bodies.
  // In this sample we create a ragdoll. The rigid bodies are kinematic - therefore, 
  // they push other rigid bodies but cannot be pushed themselves.
  // Ragdoll motors (in Velocity mode) are used to set the velocities of the rigid bodies. 
  // In Update() the method Ragdoll.DriveToPose() is called in each frame to update the
  // motor target positions.
  // Ragdoll joints and limits are not used.
  public class KinematicRagdollSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0), QuaternionF.CreateRotationY(ConstantsF.Pi));
    private SkeletonPose _skeletonPose;
    private Ragdoll _ragdoll;


    public KinematicRagdollSample(Game game)
      : base(game)
    {
      DisplayMessage = "KinematicRagdollSample\n"
                       + "This ragdoll is kinematic. The bodies are moved by velocity ragdoll motors.\n"
                       + "The ragdoll pushes other objects but does not react to impacts.\n"
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

      // Load model and start animation.
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);
      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
      var loopingAnimation = new AnimationClip<SkeletonPose>(animations.Values.First())
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };
      var animationController = AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_skeletonPose);
      animationController.UpdateAndApply();

      // Create a ragdoll for the Dude model.
      _ragdoll = new Ragdoll();
      DudeRagdollCreator.Create(_skeletonPose, _ragdoll, Simulation);

      // Set the world space pose of the whole ragdoll. And copy the bone poses of the
      // current skeleton pose.
      _ragdoll.Pose = _pose;
      _ragdoll.UpdateBodiesFromSkeleton(_skeletonPose);

      // Set all bodies to kinematic -  they should not be affected by forces.
      foreach (var body in _ragdoll.Bodies)
      {
        if (body != null)
        {
          body.MotionType = MotionType.Kinematic;
        }
      }

      // Set all motors to velocity motors. Velocity motors change RigidBody.LinearVelocity
      // RigidBody.AngularVelocity to move the rigid bodies. 
      foreach (RagdollMotor motor in _ragdoll.Motors)
      {
        if (motor != null)
        {
          motor.Mode = RagdollMotorMode.Velocity;
        }
      }
      _ragdoll.EnableMotors();

      // In this sample, we do not need joints or limits.
      _ragdoll.DisableJoints();
      _ragdoll.DisableLimits();

      // Add ragdoll rigid bodies to the simulation.
      _ragdoll.AddToSimulation(Simulation);

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // Update the motors. The velocity motors will modify the rigid body velocities.
      _ragdoll.DriveToPose(_skeletonPose, deltaTime);

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      DrawSkinnedModel(_model, _pose, _skeletonPose);

      base.Draw(gameTime);
    }
  }
}
