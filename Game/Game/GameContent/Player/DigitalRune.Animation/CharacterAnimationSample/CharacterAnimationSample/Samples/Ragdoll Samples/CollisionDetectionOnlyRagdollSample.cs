using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample shows how to use the Ragdoll class to manage collision objects for 
  // animated models.
  // In this sample we create a ragdoll. Only the bodies are used. Collision response 
  // is disabled. Joints, limits and motors are not needed. 
  // The method Update() moves the bodies using the Ragdoll.UpdateBodiesFromSkeleton() 
  // method and it checks if a ball shot by the user collides with the head.
  public class CollisionDetectionOnlyRagdollSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0), QuaternionF.CreateRotationY(ConstantsF.Pi));
    private SkeletonPose _skeletonPose;
    private Ragdoll _ragdoll;
    private bool _hitDetected;


    public CollisionDetectionOnlyRagdollSample(Game game)
      : base(game)
    {
      DisplayMessage = "CollisionDetectionOnlyRagdollSample\n"
                       + "Rigid bodies are moved with the animated bodies and can be used for collision detection.\n"
                       + "Press <Right Mouse Button> to shoot balls. Aim for the head!\n"
                       + "Press <Space> to display model in bind pose.";
    }


    protected override void LoadContent()
    {
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
      AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_skeletonPose);

      // Create a ragdoll for the Dude model.
      _ragdoll = new Ragdoll();
      DudeRagdollCreator.Create(_skeletonPose, _ragdoll, Simulation);

      // Set the world space pose of the whole ragdoll. 
      _ragdoll.Pose = _pose;
      // And copy the bone poses of the current skeleton pose.
      _ragdoll.UpdateBodiesFromSkeleton(_skeletonPose);

      foreach(var body in _ragdoll.Bodies)
      {
        if (body != null)
        {
          // Set all bodies to kinematic - they should not be affected by forces.
          body.MotionType = MotionType.Kinematic;

          // Disable collision response.
          body.CollisionResponseEnabled = false;
        }
      }
      
      // In this sample, we do not need joints, limits or motors.
      _ragdoll.DisableJoints();
      _ragdoll.DisableLimits();
      _ragdoll.DisableMotors();

      // Add ragdoll rigid bodies to the simulation.
      _ragdoll.AddToSimulation(Simulation);
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      // <Space> --> Reset skeleton pose to bind pose.
      if (InputService.IsDown(Keys.Space))
        _skeletonPose.ResetBoneTransforms();

      // ----- Detect collisions
      _hitDetected = false;

      // Get all contact sets of the head.
      RigidBody headBody = _ragdoll.Bodies[7];
      var headContacts = Simulation.CollisionDomain.GetContacts(headBody.CollisionObject);
      foreach (var contactSet in headContacts)
      {
        // Get the rigid body that collided with the head.
        RigidBody otherBody;
        if (contactSet.ObjectA.GeometricObject == headBody)
          otherBody = contactSet.ObjectB.GeometricObject as RigidBody;
        else
          otherBody = contactSet.ObjectA.GeometricObject as RigidBody;

        // If the head collided with a ball (from the BallShooter.cs), then set the hit flag.
        if (otherBody != null && otherBody.Name.StartsWith("Ball"))
        {
          _hitDetected = true;
          break;
        }
      }

      // Update the bodies to match the current skeleton pose.
      // This method changes the body poses directly. 
      _ragdoll.UpdateBodiesFromSkeleton(_skeletonPose);
      
      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      DrawSkinnedModel(_model, _pose, _skeletonPose);

      // Draw a giant message when a hit was detected.
      if (_hitDetected)
      {
        SpriteBatch.Begin();
        SpriteBatch.DrawString(SpriteFont, "Ouch!!!", new Vector2(200, 200), Color.Black, 0, new Vector2(), 10, SpriteEffects.None, 0);
        SpriteBatch.End();
      }

      base.Draw(gameTime);
    }
  }
}
