using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample shows how to use a JacobianTransposeIKSolver to let an arm reach for a target.
  public class JacobianTransposeIKSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0));
    private SkeletonPose _skeletonPose;
    private Vector3F _targetPosition = new Vector3F(1, 1, 1);
    private JacobianTransposeIKSolver _ikSolver;


    public JacobianTransposeIKSample(Game game)
      : base(game)
    {
      DisplayMessage = "JacobianTransposeIKSample\n"
                     + "A JacobianTransposeIKSolver modifies the arm of the model.\n"
                     + "Limits are used to keep the palm of the hand parallel to the ground.\n"
                     + "Press <4>-<9> on the numpad to move the target.\n";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      // Create the IK solver. The JacobianTranspose method can solve long bone chains with 
      // limits. It allocates heap memory as is not recommended for performance critical 
      // console or phone games.
      _ikSolver = new JacobianTransposeIKSolver
      {
        SkeletonPose = _skeletonPose,

        // The chain starts at the upper arm.
        RootBoneIndex = 13,

        // The chain ends at the hand bone.
        TipBoneIndex = 15,

        // The offset from the hand center to the hand origin.
        TipOffset = new Vector3F(0.1f, 0, 0),

        // This solver uses an iterative method and will make up to 100 iterations if necessary.
        NumberOfIterations = 100,

        // This parameter must be hand-tuned. Make it too large and the solver is unstable.
        // Make it too low and the solver needs a crazy amount of iterations.
        StepSize = 1,

        // A method that applies bone limits.
        LimitBoneTransforms = LimitBoneTransform,
      };

      base.LoadContent();
    }


    private void LimitBoneTransform()
    {
      // This method is called by the JacobianTransposeIKSolver after each internal iteration.
      // The job of this method is to apply bone limits; for example, the elbow should not
      // bend backwards, etc. 
      // To apply a limit, get the bone transform or bone pose from skeleton pose, check if 
      // is in the allowed range. If it is outside the allowed range, rotate it back to the
      // nearest allowed rotation.

      // Here, for example, we only make sure that the palm of the hand is always parallel 
      // to the ground plane - as if the character wants to grab a horizontal bar or as 
      // if it wants to place the hand on horizontal plane.
      SrtTransform bonePoseAbsolute = _skeletonPose.GetBonePoseAbsolute(15);
      Vector3F palmAxis = bonePoseAbsolute.ToParentDirection(-Vector3F.UnitY);
      bonePoseAbsolute.Rotation = QuaternionF.CreateRotation(palmAxis, Vector3F.UnitY) * bonePoseAbsolute.Rotation;
      _skeletonPose.SetBonePoseAbsolute(15, bonePoseAbsolute);
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

      // Let IK solver update the bones.
      _ikSolver.Target = localTargetPosition;
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
