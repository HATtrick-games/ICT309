using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample shows how to use a TwoJointIKSolver for foot placement.
  public class TwoJointIKSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0));
    private SkeletonPose _skeletonPose;

    private Vector3F _targetPosition = new Vector3F(0, 0.5f, 0.5f);

    private TwoJointIKSolver _ikSolver;


    public TwoJointIKSample(Game game)
      : base(game)
    {
      DisplayMessage = "TwoJointIKSample\n"
                     + "A TwoJointIKSolver is used for foot placement.\n"
                     + "Press <4>-<9> on the numpad to move the target.\n"
                     + "Press <Space> to limit the rotation of the foot.";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      // Create the IK solver. The TwoJointIkSolver is usually used for arms and legs.
      // it modifies two bones and supports limits for the second bone. 
      _ikSolver = new TwoJointIKSolver
      {
        SkeletonPose = _skeletonPose,

        // The chain starts at the upper leg.
        RootBoneIndex = 54,

        // The second bone modified bone is the lower leg.
        HingeBoneIndex = 55,

        // The chain ends at the foot bone.
        TipBoneIndex = 56,

        // The direction of the hinge axis (in bone space).
        HingeAxis = -Vector3F.UnitZ,

        // The hinge limits.
        MinHingeAngle = 0,
        MaxHingeAngle = ConstantsF.PiOver2,

        // The offset from the ankle to the bottom of the foot.
        TipOffset = new Vector3F(0.23f, 0, 0),        
      };

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // ----- Move target if <NumPad4-9> are pressed.
      Vector3F translation = new Vector3F();
      if (InputService.IsDown(Keys.NumPad4))
        translation.X -= 1;
      if (InputService.IsDown(Keys.NumPad6))
        translation.X += 1;
      if (InputService.IsDown(Keys.NumPad8))
        translation.Y += 1;
      if (InputService.IsDown(Keys.NumPad5))
        translation.Y -= 1;
      if (InputService.IsDown(Keys.NumPad9))
        translation.Z += 1;
      if (InputService.IsDown(Keys.NumPad7))
        translation.Z -= 1;

      translation = translation * deltaTime;
      _targetPosition += translation;

      // Convert target world space position to model space. - The IK solvers work in model space.
      Vector3F localTargetPosition = _pose.ToLocalPosition(_targetPosition);

      // Reset the affected bones. This is optional. It removes unwanted twist from the bones.
      _skeletonPose.ResetBoneTransforms(_ikSolver.RootBoneIndex, _ikSolver.TipBoneIndex);
      
      _ikSolver.Target = localTargetPosition;
      
      // We can set a target orientation for the tip bone. This can be used to place the
      // foot correctly on an inclined plane.
      if (InputService.IsDown(Keys.Space))
        _ikSolver.TipBoneOrientation = _skeletonPose.GetBonePoseAbsolute(56).Rotation;
      else
        _ikSolver.TipBoneOrientation = null;

      // Let IK solver update the bones.
      _ikSolver.Solve(deltaTime);

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      DrawSkinnedModel(_model, _pose, _skeletonPose);    
      DrawIKTarget();

      base.Draw(gameTime);
    }


    // Draws a red cross for the IK target.
    private void DrawIKTarget()
    {
      var p = GraphicsDevice.Viewport.Project(
        (Vector3)_targetPosition,
        Camera.Projection,
        Camera.View,
        Matrix.Identity);

      var length = (p - GraphicsDevice.Viewport.Project(
        (Vector3)_targetPosition + new Vector3(0, 0.2f, 0),
        Camera.Projection,
        Camera.View,
        Matrix.Identity)).Length();

      var verticalBar = new Rectangle((int)p.X - 1, (int)(p.Y - length), 2, (int)(length * 2));
      var horizontalBar = new Rectangle((int)(p.X - length), (int)p.Y - 1, (int)(length * 2), 2);

      SpriteBatch.Begin();
      SpriteBatch.Draw(WhiteTexture, verticalBar, Color.OrangeRed);
      SpriteBatch.Draw(WhiteTexture, horizontalBar, Color.OrangeRed);
      SpriteBatch.End();
    }
  }
}
