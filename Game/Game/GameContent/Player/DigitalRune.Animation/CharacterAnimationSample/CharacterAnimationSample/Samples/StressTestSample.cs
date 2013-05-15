using System;
using System.Collections.Generic;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CharacterAnimationSample
{
  // This sample renders many models with independent animations to test performance.
  public class StressTestSample : Sample
  {
    private const int NumberOfModels = 100;
    private Model _model;
    private Pose[] _poses;
    private SkeletonPose[] _skeletonPoses;
    private ITimeline[] _animations;
    private int _currentAnimationIndex;


    public StressTestSample(Game game)
      : base(game)
    {
      DisplayMessage = "StressTestSample\n"
                     + "A performance test. All models are animated independently.\n"
                     + "Press <Space> to start next animation.";
    }


    protected override void LoadContent()
    {
      _model = Game.Content.Load<Model>("PlayerMarine");
      
      var additionalData = (Dictionary<string, object>)_model.Tag;
      var skeleton = (Skeleton)additionalData["Skeleton"];
      var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];

      // Create looping animations for all animations imported animations.
      _animations = new ITimeline[animations.Count];
      int index = 0;
      foreach (var animation in animations.Values)
      {
        _animations[index] = new AnimationClip<SkeletonPose>(animation)
        {
          LoopBehavior = LoopBehavior.Cycle,
          Duration = TimeSpan.MaxValue,
        };
        index++;
      }

      // Create a lot of skeleton pose instances and start a new animation on each skeleton 
      // pose instance.
      _skeletonPoses = new SkeletonPose[NumberOfModels];
      _poses = new Pose[NumberOfModels];
      for (int i = 0; i < NumberOfModels; i++)
      {
        var n = _skeletonPoses.Length;
        var rowLength = (int)Math.Sqrt(n);
        var x = (i % rowLength) - rowLength / 2;
        var z = -i / rowLength;
        var position = new Vector3F(x, 0, z) * 1.5f;

        _poses[i] = new Pose(position);
        _skeletonPoses[i] = SkeletonPose.Create(skeleton);
        
        AnimationService.StartAnimation(_animations[0], (IAnimatableProperty)_skeletonPoses[i]).AutoRecycle();
      }

      base.LoadContent();
    }


    public override void Update(GameTime gameTime)
    {
      // <Space> --> Cross-fade to next animation.
      if (InputService.IsPressed(Keys.Space, false))
      {
        Profiler.MainProfiler.Start("SwitchAnimation");
        
        _currentAnimationIndex++;
        if (_currentAnimationIndex >= _animations.Length)
          _currentAnimationIndex = 0;

        for (int i = 0; i < _skeletonPoses.Length; i++)
        {
          // Start a next animation using a Replace transition with a fade-in time.
          AnimationService.StartAnimation(
            _animations[_currentAnimationIndex],
            (IAnimatableProperty<SkeletonPose>)_skeletonPoses[i], 
            AnimationTransitions.Replace(TimeSpan.FromSeconds(0.2)))
            .AutoRecycle();
        }

        Profiler.MainProfiler.Stop();
      }

      // SkeletonPose.Update() is a method that can be called to update all internal skeleton pose
      // data immediately. If SkeletonPose.Update() is not called, the internal data will
      // be updated when needed - for example, when SkeletonPose.SkinningMatricesXna are accessed.
      Profiler.MainProfiler.Start("SkeletonPoses.Update");

      for (int i = 0; i < NumberOfModels; i++)
        _skeletonPoses[i].Update();

      Profiler.MainProfiler.Stop();

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
      GraphicsDevice.BlendState = BlendState.Opaque;

      GroundModel.Draw(Matrix.Identity, Camera.View, Camera.Projection);
      for (int i = 0; i < NumberOfModels; i++)
        DrawSkinnedModel(_model, _poses[i], _skeletonPoses[i]);

      base.Draw(gameTime);
    }
  }
}
