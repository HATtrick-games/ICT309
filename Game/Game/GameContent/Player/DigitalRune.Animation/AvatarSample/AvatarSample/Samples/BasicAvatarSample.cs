using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace AvatarSample
{
  // This sample shows how to use the AvatarPose class to manipulate an avatar.
  // The bones are drawn for debugging. - This is very useful to check bone coordinate
  // systems and bone names.
  public class BasicAvatarSample : Sample
  {
    private BasicEffect _basicEffect;

    private AvatarDescription _avatarDescription;
    private AvatarRenderer _avatarRenderer;

    // The AvatarPose which defines the pose of the skeleton and the avatar expression.
    private AvatarPose _avatarPose;
    
    // The world space position and orientation of the avatar.
    private Pose _pose = new Pose(new Vector3F(-0.5f, 0, 0));

    
    public BasicAvatarSample(Game game)
      : base(game)
    {
      DisplayMessage = "BasicAvatarSample\n"
                     + "This sample shows how to use the AvatarPose class.\n"
                     + "One arm is rotated in code.\n"
                     + "The AvatarSkeleton is drawn for debugging.";
    }


    protected override void LoadContent()
    {
      _basicEffect = new BasicEffect(GraphicsDevice);

      // Create a random avatar.
      _avatarDescription = AvatarDescription.CreateRandom();
      _avatarRenderer = new AvatarRenderer(_avatarDescription);
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (_avatarPose == null)
      {
        // Must wait till renderer is ready. Before that we do not get skeleton info.
        if (_avatarRenderer.State == AvatarRendererState.Ready)
        {
          // Create AvatarPose.
          _avatarPose = new AvatarPose(_avatarRenderer);

          // A 'bone transform' is the transformation of a bone relative to its bind pose.
          // Bone transforms define the pose of a skeleton.

          // Rotate arm of avatar. 
          SkeletonPose skeletonPose = _avatarPose.SkeletonPose;
          int shoulderIndex = skeletonPose.Skeleton.GetIndex("ShoulderLeft");
          skeletonPose.SetBoneTransform(shoulderIndex, new SrtTransform(QuaternionF.CreateRotationZ(-0.9f)));

          // The class SkeletonHelper provides some useful extension methods.
          // One is SetBoneRotationAbsolute() which sets the orientation of a bone relative 
          // to model space.
          // Rotate elbow to make the lower arm point forward.
          int elbowIndex = skeletonPose.Skeleton.GetIndex("ElbowLeft");
          SkeletonHelper.SetBoneRotationAbsolute(skeletonPose, elbowIndex, QuaternionF.CreateRotationY(ConstantsF.PiOver2));
        }
      }
    }


    public override void Draw(GameTime gameTime)
    {
      // Reset render states.
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);

      if (_avatarPose != null)
      {
        // Draw avatar.
        _avatarRenderer.World = _pose;
        _avatarRenderer.View = Camera.View;
        _avatarRenderer.Projection = Camera.Projection;
        _avatarRenderer.Draw(_avatarPose);

        // Draw avatar skeleton for debugging.
        _basicEffect.World = _pose;
        _basicEffect.View = Camera.View;
        _basicEffect.Projection = Camera.Projection;
        SkeletonHelper.DrawBones(_avatarPose.SkeletonPose, GraphicsDevice, _basicEffect, 0.1f,
                                 SpriteBatch, SpriteFont, Color.Orange);
      }

      base.Draw(gameTime);
    }
  }
}
