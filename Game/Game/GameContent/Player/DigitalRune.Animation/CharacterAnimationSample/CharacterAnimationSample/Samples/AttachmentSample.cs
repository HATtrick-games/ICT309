using System;
using System.Collections.Generic;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CharacterAnimationSample
{
  // This sample shows how to attach a model to a bone of a skinned model.
  public class AttachmentSample : Sample
  {
    private Model _marine;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0));
    private SkeletonPose _skeletonPose;

    // The model that will be attached to a bone.
    private Model _weapon;


    public AttachmentSample(Game game)
      : base(game)
    {
      DisplayMessage = "AttachmentSample\n"
                       + "This sample shows how to attach a weapon model to the hand bone of the marine model.";
    }


    protected override void LoadContent()
    {
      _marine = Game.Content.Load<Model>("PlayerMarine");
      var additionalData = (Dictionary<string, object>)_marine.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      _skeletonPose = SkeletonPose.Create(skeleton);

      // Play a looping 'Idle' animation.
      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
      var idleAnimation = animations["Idle"];
      var loopingAnimation = new AnimationClip<SkeletonPose>(idleAnimation)
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };
      var animationController = AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_skeletonPose);
      animationController.UpdateAndApply();
      animationController.AutoRecycle();
      
      _weapon = Game.Content.Load<Model>("WeaponMachineGun");
      
      base.LoadContent();
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);

      DrawSkinnedModel(_marine, _pose, _skeletonPose);
      
      // ----- Draw weapon
      // The offset of the weapon origin to the hand bone origin (in bone space)
      Matrix offset = Matrix.Identity;

      // Get hand bone index.
      int handBoneIndex = _skeletonPose.Skeleton.GetIndex("R_Hand2");

      // The hand bone position in model space
      Matrix bonePose = _skeletonPose.GetBonePoseAbsolute(handBoneIndex);

      // The world space matrix of the weapon. 
      // (XNA matrices are multiplied from left to right.)
      Matrix weaponWorldMatrix = offset * bonePose * _pose;

      // Draw weapon.
      _weapon.Draw(weaponWorldMatrix, Camera.View, Camera.Projection);

      base.Draw(gameTime);
    }
  }
}
