﻿using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample shows how to make a model look at a certain target.
  // Several bones (starting at the spine, up to the head) are affected by LookAtIKISolvers.
  public class LookAtIKSample : Sample
  {
    private Model _model;
    private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0), Matrix33F.CreateRotationY(ConstantsF.Pi));
    private SkeletonPose _skeletonPose;

    private Vector3F _targetPosition = new Vector3F(-1, 0, 0);

    // The IK solver - one per affected bone.
    private LookAtIKSolver _spine1IK;
    private LookAtIKSolver _spine2IK;
    private LookAtIKSolver _spine3IK;
    private LookAtIKSolver _neckIK;
    private LookAtIKSolver _headIK;


    public LookAtIKSample(Game game)
      : base(game)
    {
      DisplayMessage = "LookAtIKSample\n"
                       + "This sample shows how to make a character look at a specific target.\n"
                       + "LookAtIKSolvers are used to modify spine, neck and head bones.\n"
                       + "Press <4>-<9> on the numpad to move the target.\n";
    }


    protected override void LoadContent()
    {
      // Load model and start a looping animation.
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

      // Create LookAtIKSolvers for some spine bones, the neck and the head.
      
      _spine1IK = new LookAtIKSolver
      {
        SkeletonPose = _skeletonPose,
        BoneIndex = 3,

        // The bone space axis that points in look direction.
        Forward = Vector3F.UnitY,

        // The bone space axis that points in up direction
        Up = Vector3F.UnitX,

        // An arbitrary rotation limit.
        Limit = ConstantsF.PiOver4,

        // We use a weight of 1 for the head, and lower weights for all other bones. Thus, most
        // of the looking will be done by the head bone, and the influence on the other bones is
        // smaller.
        Weight = 0.2f,

        // It is important to set the EyeOffsets. If we do not set EyeOffsets, the IK solver 
        // assumes that the eyes are positioned in the origin of the bone. 
        // Approximate EyeOffsets are sufficient.
        EyeOffset = new Vector3F(0.8f, 0, 0),
      };

      _spine2IK = new LookAtIKSolver
      {
        SkeletonPose = _skeletonPose,
        BoneIndex = 4,
        Forward = Vector3F.UnitY,
        Up = Vector3F.UnitX,
        Limit = ConstantsF.PiOver4,
        Weight = 0.2f,
        EyeOffset = new Vector3F(0.64f, 0, 0),
      };

      _spine3IK = new LookAtIKSolver
      {
        SkeletonPose = _skeletonPose,
        BoneIndex = 5,
        Forward = Vector3F.UnitY,
        Up = Vector3F.UnitX,
        Limit = ConstantsF.PiOver4,
        Weight = 0.3f,
        EyeOffset = new Vector3F(0.48f, 0, 0),
      };

      _neckIK = new LookAtIKSolver
      {
        SkeletonPose = _skeletonPose,
        BoneIndex = 6,
        Forward = Vector3F.UnitY,
        Up = Vector3F.UnitX,
        Limit = ConstantsF.PiOver4,
        Weight = 0.4f,
        EyeOffset = new Vector3F(0.32f, 0, 0),
      };

      _headIK = new LookAtIKSolver
      {
        SkeletonPose = _skeletonPose,
        BoneIndex = 7,
        Forward = Vector3F.UnitY,
        Up = Vector3F.UnitX,
        EyeOffset = new Vector3F(0.16f, 0.16f, 0),
        Weight = 1.0f,
        Limit = ConstantsF.PiOver4,
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

      // Convert target world space position to model space.
      // (The IK solvers work in model space.)
      Vector3F localTargetPosition = _pose.ToLocalPosition(_targetPosition);

      // Update the IK solver target positions.
      _spine1IK.Target = localTargetPosition;
      _spine2IK.Target = localTargetPosition;
      _spine3IK.Target = localTargetPosition;
      _neckIK.Target = localTargetPosition;
      _headIK.Target = localTargetPosition;

      // Run the IK solvers. - This immediately modifies the affected bones. Therefore, 
      // it is important to run the solvers in the correct order (from parent to child 
      // bone).
      _spine1IK.Solve(deltaTime);
      _spine2IK.Solve(deltaTime);
      _spine3IK.Solve(deltaTime);
      _neckIK.Solve(deltaTime);
      _headIK.Solve(deltaTime);
      
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
