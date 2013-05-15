using System;
using DigitalRune.Animation;
using DigitalRune.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AnimationSample
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager _graphicsManager;
    private AnimationManager _animationManager;
    private InputManager _inputManager;

    // A array of methods that create the sample instances.
    private Func<Sample>[] _createSampleDelegates;
    private int _activeSampleIndex = -1;
    private Sample _activeSample;


    static Game1()
    {
      // ----- License Keys -----
      // All license keys must be set before any function of a DigitalRune library can be used.
      // If you have received several license keys, call AddSerialNumber for each license key.
      //DigitalRune.Licensing.AddSerialNumber("A4DsypZMsBsI8xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
    }


    public Game1()
    {
      _graphicsManager = new GraphicsDeviceManager(this)
      {
        PreferredBackBufferWidth = 1280,
        PreferredBackBufferHeight = 720,
      };
      Content.RootDirectory = "Content";
      IsMouseVisible = false;
    }


    protected override void Initialize()
    {
      // ----- Initialize Services.
      // The services are stored in Game.Services to make them accessible for all
      // game components.

      // Add the input service, which manages device input, button presses, etc.
      _inputManager = new InputManager(false);
      Services.AddService(typeof(IInputService), _inputManager);

      // Add the animation service.
      _animationManager = new AnimationManager();
      Services.AddService(typeof(IAnimationService), _animationManager);

      // ----- Add GameComponents
      // Initialize the sample factory methods. All samples must be added to this array
      // to be shown.
      _createSampleDelegates = new Func<Sample>[]
      {
        () => new BasicAnimationSample(this),
        () => new CompositeAnimationSample(this), 
        () => new PathAnimationSample(this), 
        () => new EasingSample(this), 
        () => new CustomAnimationSample(this), 
        () => new CrossFadeSample(this), 
        () => new AnimatableObjectSample(this), 
        () => new BlendedAnimationSample(this), 
        () => new AdditiveAnimationSample(this), 
        () => new DoubleAnimationSample(this), 
        () => new StringAnimationSample(this),         
      };

      // Start the first sample in the array.
      _activeSampleIndex = 0;
      _activeSample = _createSampleDelegates[0]();
      Components.Add(_activeSample);

      base.Initialize();
    }


    protected override void Update(GameTime gameTime)
    {
      TimeSpan deltaTime = gameTime.ElapsedGameTime;

      // Update input manager. The input manager gets the device states and performs other work.
      _inputManager.Update(deltaTime);

      // If Escape or Back is pressed, the game exits.
      if (_inputManager.IsDown(Keys.Escape) || _inputManager.IsPressed(Buttons.Back, false, PlayerIndex.One))
      {
        Exit();
      }

      SwitchSamples();

      // Update the game components.
      base.Update(gameTime);

      // Update the animations. (The animations results are stored internally but not yet applied).
      _animationManager.Update(deltaTime);

      // Apply the animations. (This method changes the animated objects. It must be called
      // somewhere where it is thread-safe to change animated objects.)
      _animationManager.ApplyAnimations();
    }


    private void SwitchSamples()
    {
      // If left arrow or DPadLeft is pressed, we want to switch to the previous sample (index - 1).
      int sampleIndexIncrement = 0;
      if (_inputManager.IsPressed(Keys.Left, false) || _inputManager.IsPressed(Buttons.DPadLeft, false, PlayerIndex.One))
      {
        sampleIndexIncrement = -1;
      }

      // If right arrow or DPadRight is pressed, we want to switch to the next sample (index + 1).
      if (_inputManager.IsPressed(Keys.Right, false) || _inputManager.IsPressed(Buttons.DPadRight, false, PlayerIndex.One))
      {
        sampleIndexIncrement = +1;
      }

      if (sampleIndexIncrement != 0)
      {
        // Switch samples.

        // Stop current sample.
        Components.Remove(_activeSample);
        _activeSample.Dispose();

        // Clean up memory.
        GC.Collect();

        // Get index of next sample.
        _activeSampleIndex += sampleIndexIncrement;
        if (_activeSampleIndex >= _createSampleDelegates.Length)
          _activeSampleIndex = 0;
        else if (_activeSampleIndex < 0)
          _activeSampleIndex = _createSampleDelegates.Length - 1;

        // Add next sample.
        _activeSample = _createSampleDelegates[_activeSampleIndex]();
        Components.Add(_activeSample);
      }
    }
  }
}
