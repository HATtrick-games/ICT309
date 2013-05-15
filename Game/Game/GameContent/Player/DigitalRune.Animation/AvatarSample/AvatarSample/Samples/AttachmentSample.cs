using System;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace AvatarSample
{
  // This sample shows how to attach an object to an animated avatar.
  public class AttachmentSample : Sample
  {
    private AvatarDescription _avatarDescription;
    private AvatarRenderer _avatarRenderer;
    private AvatarPose _avatarPose;
    private Pose _pose = new Pose(new Vector3F(-0.5f, 0, 0));
    private TimelineClip _walkAnimation;
    private Model _baseballBat;


    public AttachmentSample(Game game)
      : base(game)
    {
      DisplayMessage = "AttachmentSample\n"
                     + "A baseball bat is attached to the hand of an avatar.";
    }


    protected override void LoadContent()
    {
      // Create a random avatar.
      _avatarDescription = AvatarDescription.CreateRandom();
      _avatarRenderer = new AvatarRenderer(_avatarDescription);

      // Load walk animation using the content pipeline.
      TimelineGroup animation = Game.Content.Load<TimelineGroup>("Walk");

      // Create a looping walk animation.
      _walkAnimation = new TimelineClip(animation)
      {
        LoopBehavior = LoopBehavior.Cycle,  // Cycle Walk animation...
        Duration = TimeSpan.MaxValue,       // ...forever.
      };

      _baseballBat = Game.Content.Load<Model>("BaseballBat");
      
      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (_avatarPose == null)
      {
        if (_avatarRenderer.State == AvatarRendererState.Ready)
        {
          _avatarPose = new AvatarPose(_avatarRenderer);
          AnimationService.StartAnimation(_walkAnimation, _avatarPose).AutoRecycle();
        }
      }
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      
      if (_avatarPose != null)
      {
        // ----- Draw avatar
        _avatarRenderer.World = _pose;
        _avatarRenderer.View = Camera.View;
        _avatarRenderer.Projection = Camera.Projection;
        _avatarRenderer.Draw(_avatarPose);

        // ----- Draw baseball bat
        // The offset of the baseball bat origin to the bone origin (in bone space)
        Matrix offset = Matrix.CreateRotationY(MathHelper.ToRadians(-20)) * Matrix.CreateTranslation(0.01f, 0.05f, 0.0f);

        // The bone position in model space
        Matrix bonePose = _avatarPose.SkeletonPose.GetBonePoseAbsolute((int)AvatarBone.SpecialRight);
        
        // The baseball bat matrix in world space
        Matrix batWorldMatrix = offset * bonePose * _pose;

        _baseballBat.Draw(batWorldMatrix, Camera.View, Camera.Projection);
      }

      base.Draw(gameTime);
    }
  }
}
