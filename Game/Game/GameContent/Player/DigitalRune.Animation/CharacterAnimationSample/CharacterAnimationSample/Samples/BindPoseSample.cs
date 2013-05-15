using System.Collections.Generic;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CharacterAnimationSample
{
  // This sample renders the Dude and the PlayerMarine model in their bind poses.
  // The skeleton is drawn for debugging. This sample is useful to analyze the skeletons,
  // check bone name, bone indices and bone coordinate systems.
  public class BindPoseSample : Sample
  {
    private BasicEffect _basicEffect;
   
    // The model of the dude.
    private Model _dudeModel;
    // The world space position and orientation of the dude.
    private Pose _dudePose = new Pose(new Vector3F(-1f, 0, 0));
    // The skeleton pose that manages the skeleton and bone transforms.
    private SkeletonPose _dudeSkeletonPose;

    // The marine model.
    private Model _marineModel;
    private Pose _marinePose = new Pose(new Vector3F(1f, 0, 0));
    private SkeletonPose _marineSkeletonPose;


    public BindPoseSample(Game game)
      : base(game)
    {
      DisplayMessage = "BindPoseSample\n"
                       + "This sample renders two models in their bind poses.\n"
                       + "The skeletons are drawn for debugging. This sample is useful to analyze the skeletons,\n"
                       + "check bone name, bone indices and bone coordinate systems.";
    }


    protected override void LoadContent()
    {
      _basicEffect = new BasicEffect(GraphicsDevice);

      // Load dude model.
      _dudeModel = Game.Content.Load<Model>("Dude");

      // The SkinnedModelProcessor stores additional data in the tag.
      Dictionary<string, object> additionalData = (Dictionary<string, object>)_dudeModel.Tag;

      // Get the skeleton.
      Skeleton skeleton = (Skeleton)additionalData["Skeleton"];

      // Create a skeleton pose that can be used transform and animate the skeleton.
      _dudeSkeletonPose = SkeletonPose.Create(skeleton);

      // Repeat the above steps for the marine model:
      _marineModel = Game.Content.Load<Model>("dude");
      additionalData = (Dictionary<string, object>)_marineModel.Tag;
      skeleton = (Skeleton)additionalData["Skeleton"];
      _marineSkeletonPose = SkeletonPose.Create(skeleton);

      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      // Restore render states.
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      // Draw a green ground plane model.
      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);

      // Draw the Dude model. DrawSkinnedModel() is defined in Sample.cs.
      DrawSkinnedModel(_dudeModel, _dudePose, _dudeSkeletonPose);      

      // Draw skeleton for debugging.
      _basicEffect.World = _dudePose;
      _basicEffect.View = Camera.View;
      _basicEffect.Projection = Camera.Projection;
      SkeletonHelper.DrawBones(_dudeSkeletonPose, GraphicsDevice, _basicEffect, 0.1f,
                               SpriteBatch, SpriteFont, Color.Orange);

      // Repeat the above steps for the marine model:
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      DrawSkinnedModel(_marineModel, _marinePose, _marineSkeletonPose);

      // Draw skeleton for debugging.
      _basicEffect.World = _marinePose;
      _basicEffect.View = Camera.View;
      _basicEffect.Projection = Camera.Projection;
      SkeletonHelper.DrawBones(_marineSkeletonPose, GraphicsDevice, _basicEffect, 0.1f,
                               SpriteBatch, SpriteFont, Color.Orange);

      base.Draw(gameTime);
    }


    /* If you want to modify the debug rendering or if you want to know how it works, 
       here is a DrawBones() method that produces the same result as SkeletonHelper.DrawBones().
     
    /// <summary>
    /// Draws the skeleton bones, bone space axes and bone names for debugging. 
    /// </summary>
    /// <param name="skeletonPose">The skeleton pose.</param>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <param name="effect">
    /// An initialized basic effect instance. BasicEffect.World, BasicEffect.View and 
    /// BasicEffect.Projection must be correctly initialized before this method is called.
    /// </param>
    /// <param name="axisLength">The visible length of the bone space axes.</param>
    /// <param name="spriteBatch"> A SpriteBatch. Can be null to skip text rendering.  </param>
    /// <param name="spriteFont"> A SpriteFont. Can be null to skip text rendering.  </param>
    /// <param name="color">The color for the bones and the bone names.</param>
    private static void DrawBones(SkeletonPose skeletonPose, GraphicsDevice graphicsDevice,
      BasicEffect effect, float axisLength, SpriteBatch spriteBatch, SpriteFont spriteFont,
      Color color)
    {
      if (skeletonPose == null)
        throw new ArgumentNullException("skeletonPose");
      if (graphicsDevice == null)
        throw new ArgumentNullException("graphicsDevice");
      if (effect == null)
        throw new ArgumentNullException("effect");
 
      var oldVertexColorEnabled = effect.VertexColorEnabled;
      effect.VertexColorEnabled = true;
 
      // No font, then we don't need the sprite batch.
      if (spriteFont == null)
        spriteBatch = null;
 
      if (spriteBatch != null)
        spriteBatch.Begin();
 
      List<VertexPositionColor> vertices = new List<VertexPositionColor>();
 
      var skeleton = skeletonPose.Skeleton;
      for (int i = 0; i < skeleton.NumberOfBones; i++)
      {
        // Data of bone i:
        string name = skeleton.GetName(i);
        SrtTransform bonePose = skeletonPose.GetBonePoseAbsolute(i);
        var translation = (Vector3)bonePose.Translation;
        var rotation = (Quaternion)bonePose.Rotation;
 
        // Draw line to parent joint representing the parent bone.
        int parentIndex = skeleton.GetParent(i);
        if (parentIndex >= 0)
        {
          SrtTransform parentPose = skeletonPose.GetBonePoseAbsolute(parentIndex);
          vertices.Add(new VertexPositionColor(translation, color));
          vertices.Add(new VertexPositionColor((Vector3)parentPose.Translation, color));
        }
 
        // Add three lines in Red, Green and Blue that visualize the bone space.
        vertices.Add(new VertexPositionColor(translation, Color.Red));
        vertices.Add(new VertexPositionColor(
          translation + Vector3.Transform(Vector3.UnitX, rotation) * axisLength, Color.Red));
        vertices.Add(new VertexPositionColor(translation, Color.Green));
        vertices.Add(new VertexPositionColor(
          translation + Vector3.Transform(Vector3.UnitY, rotation) * axisLength, Color.Green));
        vertices.Add(new VertexPositionColor(translation, Color.Blue));
        vertices.Add(new VertexPositionColor(
          translation + Vector3.Transform(Vector3.UnitZ, rotation) * axisLength, Color.Blue));
 
        // Draw name.
        if (spriteBatch != null && !string.IsNullOrEmpty(name))
        {
          // Compute the 3D position in view space. Text is rendered near drawn x axis.
          Vector3 textPosition = translation + Vector3.TransformNormal(Vector3.UnitX, bonePose)
                                 * axisLength * 0.5f;
          var textPositionWorld = Vector3.Transform(textPosition, effect.World);
          var textPositionView = Vector3.Transform(textPositionWorld, effect.View);
 
          // Check if the text is in front of the camera.
          if (textPositionView.Z < 0)
          {
            // Project text position to screen.            
            Vector3 textPositionProjected = graphicsDevice.Viewport.Project(
              textPosition, effect.Projection, effect.View, effect.World);
 
            spriteBatch.DrawString(spriteFont, name + " " + i,
              new Vector2(textPositionProjected.X, textPositionProjected.Y), color);
          }
        }
      }
 
      if (spriteBatch != null)
        spriteBatch.End();
 
      // Draw axis lines in one batch.
      graphicsDevice.DepthStencilState = DepthStencilState.None;
      effect.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0,
        vertices.Count / 2);
 
      effect.VertexColorEnabled = oldVertexColorEnabled;
    }
    */
  }
}
