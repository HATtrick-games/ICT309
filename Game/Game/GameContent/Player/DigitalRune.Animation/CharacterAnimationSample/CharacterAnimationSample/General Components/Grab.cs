using System.Linq;
using DigitalRune.Game.Input;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Collisions;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Physics.Constraints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // Enables the user to grab physics objects.
  public class Grab : GameComponent
  {
    private readonly IInputService _inputService;
    private readonly Simulation _simulation;

    // When an object is grabbed it is attached to the mouse position using a ball-socket joint.
    private BallJoint _spring;

    // Object are grabbed using the cross-hair. The cross-hair is always centered on the screen. 
    // The distance to the camera is determined when the object is grabbed.
    private float _springAttachmentDistanceFromObserver;


    public Grab(Game game)
      : base(game)
    {
      _inputService = (IInputService)game.Services.GetService(typeof(IInputService));
      _simulation = (Simulation)game.Services.GetService(typeof(Simulation));
    }


    public override void Update(GameTime gameTime)
    {
      if (_spring != null && !_inputService.IsDown(MouseButtons.Left) && !_inputService.IsDown(Buttons.LeftTrigger, PlayerIndex.One))
      {
        // The user has released the object.
        _spring.Simulation.Constraints.Remove(_spring);
        _spring = null;
      }

      if (_inputService.IsPressed(MouseButtons.Left, false) || _inputService.IsPressed(Buttons.LeftTrigger, false, PlayerIndex.One))
      {
        // The user has pressed the grab button.

        // Remove the old joint, in case anything is grabbed.
        if (_spring != null && _spring.Simulation != null)
          _spring.Simulation.Constraints.Remove(_spring);

        // The spring is attached at the position that is targeted with the cross-hair.
        // We can perform a ray hit-test to find the position. The ray starts at the camera
        // position and shoots forward (-z direction).
        Camera camera = Game.Components.OfType<Camera>().First();
        Vector3F cameraPosition = camera.Pose.Position;
        Vector3F cameraDirection = camera.Pose.ToWorldDirection(-Vector3F.UnitZ);

        // Create a ray for picking.
        RayShape ray = new RayShape(cameraPosition, cameraDirection, 1000);

        // The ray should stop at the first hit. We only want the first object.
        ray.StopsAtFirstHit = true;  

        // The collision detection requires a CollisionObject.
        CollisionObject rayCollisionObject = new CollisionObject(new GeometricObject(ray, Pose.Identity));

        // Assign the collision object to collision group 2. (In PhysicsGame.cs a collision filter
        // based on collision groups was set.)
        rayCollisionObject.CollisionGroup = 2;

        // Get the first object that has contact with the ray.
        ContactSet contactSet = _simulation.CollisionDomain.GetContacts(rayCollisionObject).FirstOrDefault();
        if (contactSet != null &&  contactSet.Count > 0)
        {
          // The ray has hit something.

          // The contact set contains all detected contacts between the ray and the rigid body.
          // Get the first contact in the contact set. (A ray hit usually contains exactly 1 contact.)
          Contact contact = contactSet[0];

          // The contact set contains the object pair of the collision. One object is the ray.
          // The other is the object we want to grab.
          CollisionObject hitCollisionObject = (contactSet.ObjectA == rayCollisionObject) ? contactSet.ObjectB : contactSet.ObjectA;

          // Check whether a dynamic rigid body was hit.
          RigidBody hitBody = hitCollisionObject.GeometricObject as RigidBody;
          if (hitBody != null && hitBody.MotionType == MotionType.Dynamic)
          {
            // Attach the rigid body at the cursor position using a ball-socket joint.
            // (Note: We could also use a FixedJoint, if we don't want any rotations.)

            // The penetration depth tells us the distance from the ray origin to the rigid body.
            _springAttachmentDistanceFromObserver = contact.PenetrationDepth;

            // Get the position where the ray hits the other object.
            // (The position is defined in the local space of the object.)
            Vector3F hitPositionLocal = (contactSet.ObjectA == rayCollisionObject) ? contact.PositionBLocal : contact.PositionALocal;

            _spring = new BallJoint
            {
              BodyA = hitBody,
              AnchorPositionALocal = hitPositionLocal,

              // We need to attach the grabbed object to a second body. In this case we just want to 
              // anchor the object at a specific point in the world. To achieve this we can use the 
              // special rigid body "World", which is defined in the simulation.
              BodyB = _simulation.World,
              // AnchorPositionBLocal is set below.

              // Some constraint adjustments.
              ErrorReduction = 0.3f,

              // We set a softness > 0. This makes the joint "soft" and it will act like 
              // damped spring. 
              Softness = 0.00001f,
              
              // We limit the maximal force. This reduces the ability of this joint to violate
              // other constraints. 
              MaxForce = 1e6f
            };

            // Add the spring to the simulation.
            _simulation.Constraints.Add(_spring);
          }
        }
      }

      if (_spring != null)
      {
        // User has grabbed something.

        // Update the position of the object by updating the anchor position of the ball-socket
        // joint.
        Camera camera = Game.Components.OfType<Camera>().First();
        Vector3F cameraPosition = camera.Pose.Position;
        Vector3F cameraDirection = camera.Pose.ToWorldDirection(-Vector3F.UnitZ);

        _spring.AnchorPositionBLocal = cameraPosition + cameraDirection * _springAttachmentDistanceFromObserver;

        // Reduce the angular velocity by a certain factor. (This acts like a damping because we
        // do not want the object to rotate like crazy.)
        _spring.BodyA.AngularVelocity *= 0.9f;
      }

      base.Update(gameTime);
    }
  }
}