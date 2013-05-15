using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample shows how to use a ClosedFormIKSample to let an arm reach for a target.
  public class ClosedFormIKSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0));
    private SkeletonPose _skeletonPose;
    private Vector3F _targetPosition = new Vector3F(1, 1, 1);
    private ClosedFormIKSolver _ikSolver;


    public ClosedFormIKSample(Game game)
      : base(game)
    {
      DisplayMessage = "ClosedFormIKSample\n"
                     + "A ClosedFormIKSolver modifies the arm of the model.\n"
                     + "Press <4>-<9> on the numpad to move the target.\n";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("Dude");
      var additionalData = (Dictionary<string, object>)_model.Tag;

      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      // Create the IK solver. The ClosedFormIKSolver uses an analytic solution to compute
      // IK for arbitrary long bone chains. It does not support bone rotation limits.
      _ikSolver = new ClosedFormIKSolver
      {
        SkeletonPose = _skeletonPose,

        // The chain starts at the upper arm.
        RootBoneIndex = 13,

        // The chain ends at the hand bone.
        TipBoneIndex = 15,

        // The offset from the hand center to the hand origin.
        TipOffset = new Vector3F(0.1f, 0, 0),
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
