using System;
using DigitalRune.Animation;
using DigitalRune.Animation.Easing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AnimationSample
{
  // This sample shows how to blend two animations.
  // Animation blending can be used to "average" two different animations. For example, when
  // animating characters a 2 second "Walk" animation cycle can be blended with a 1 second 
  // "Run" animation cycle to create a 1.5 second "FastWalk" animation cycle.
  // To blend animations you put them into a BlendGroup. The BlendGroup has a weight for each
  // animation. If the animations have different durations and you want to average the durations
  // you must call BlendGroup.SynchronizeDurations once.
  // In this sample a 4 second color and horizontal movement animation at the top of the screen
  // is blended with a 1 second color and horizontal movement animation at the bottom of the screen.
  public class BlendedAnimationSample : Sample
  {
    // An animatable sprite (see AnimatableObjectSample).
    private AnimatableSprite _animatableSprite;

    private BlendGroup _blendedAnimation;


    public BlendedAnimationSample(Game game)
      : base(game)
    {
      DisplayMessage = "BlendedAnimationSample\n"
                     + "This sample shows how to blend two animations.\n"
                     + "The first animation has a duration of 4 seconds. It moves the sprite horizontally in the upper half of the screen. \n"
                     + "The tint color changes from red to green.\n"
                     + "The second animation has a duration of 1 seconds. It moves the sprite horizontally in the lower half of the screen. \n"
                     + "The tint color changes from black to white.\n"
                     + "Press <Up> or <Down> to change the blend weight and smoothly transition between both animations.";
    }


    protected override void LoadContent()
    {
      base.LoadContent();

      Rectangle bounds = GraphicsDevice.Viewport.TitleSafeArea;

      // Create the animatable object.
      _animatableSprite = new AnimatableSprite("SpriteA", SpriteBatch, Logo)
      {
        Position = new Vector2(bounds.Center.X, bounds.Center.Y / 2.0f),
        Color = Color.Red,
      };

      Vector2FromToByAnimation slowLeftRightAnimation = new Vector2FromToByAnimation()
      {
        TargetProperty = "Position",
        From = new Vector2(bounds.Left + 100, bounds.Top + 200),
        To = new Vector2(bounds.Right - 100, bounds.Top + 200),
        Duration = TimeSpan.FromSeconds(4),
        EasingFunction = new HermiteEase { Mode = EasingMode.EaseInOut },
      };

      ColorKeyFrameAnimation redGreenAnimation = new ColorKeyFrameAnimation
      {
        TargetProperty = "Color",
        EnableInterpolation = true,
      };
      redGreenAnimation.KeyFrames.Add(new KeyFrame<Color>(TimeSpan.FromSeconds(0), Color.Red));
      redGreenAnimation.KeyFrames.Add(new KeyFrame<Color>(TimeSpan.FromSeconds(4), Color.Green));

      TimelineGroup animationA = new TimelineGroup
      {
        slowLeftRightAnimation,
        redGreenAnimation,
      };

      Vector2FromToByAnimation fastLeftRightAnimation = new Vector2FromToByAnimation
      {
        TargetProperty = "Position",
        From = new Vector2(bounds.Left + 100, bounds.Bottom - 200),
        To = new Vector2(bounds.Right - 100, bounds.Bottom - 200),
        Duration = TimeSpan.FromSeconds(1),
        EasingFunction = new HermiteEase { Mode = EasingMode.EaseInOut },
      };

      ColorKeyFrameAnimation blackWhiteAnimation = new ColorKeyFrameAnimation
      {
        TargetProperty = "Color",
        EnableInterpolation = true,
      };
      blackWhiteAnimation.KeyFrames.Add(new KeyFrame<Color>(TimeSpan.FromSeconds(0), Color.Black));
      blackWhiteAnimation.KeyFrames.Add(new KeyFrame<Color>(TimeSpan.FromSeconds(1), Color.White));

      TimelineGroup animationB = new TimelineGroup
      {
        fastLeftRightAnimation,
        blackWhiteAnimation,
      };

      // Create a BlendGroup that blends animationA and animationB. 
      // The BlendGroup uses the TargetProperty values of the contained animations to 
      // to match the animations that should be blended:
      //   slowLeftRightAnimation with fastLeftRightAnimation
      //   redGreenAnimation with blackWhiteAnimation
      _blendedAnimation = new BlendGroup
      {
        LoopBehavior = LoopBehavior.Oscillate,
        Duration = TimeSpan.MaxValue,
      };
      _blendedAnimation.Add(animationA, 1);
      _blendedAnimation.Add(animationB, 0);
      _blendedAnimation.SynchronizeDurations();

      // Start blended animation.
      AnimationService.StartAnimation(_blendedAnimation, _animatableSprite);
    }


    public override void Update(GameTime gameTime)
    {
      // <Up> --> Increase blend weight of first animation.
      if (InputService.IsDown(Keys.Up))
      {
        float weight = _blendedAnimation.GetWeight(0);

        weight += 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (weight > 1)
          weight = 1;

        _blendedAnimation.SetWeight(0, weight);
        _blendedAnimation.SetWeight(1, 1 - weight);
      }

      // <Down> --> Increase blend weight of second animation.
      if (InputService.IsDown(Keys.Down))
      {
        float weight = _blendedAnimation.GetWeight(0);

        weight -= 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (weight < 0)
          weight = 0;

        _blendedAnimation.SetWeight(0, weight);
        _blendedAnimation.SetWeight(1, 1 - weight);
      }

      base.Update(gameTime);
    }


    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.White);

      _animatableSprite.Draw();

      base.Draw(gameTime);
    }
  }
}
