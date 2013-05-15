using System;
using DigitalRune.Game.Input;
using DigitalRune.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace KinectAnimationSample
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager _graphicsManager;
    private InputManager _inputManager;
    private Simulation _simulation;

    // A array of methods that create the sample instances.
    private Func<SampleBase>[] _createSampleDelegates;
    private int _activeSampleIndex = -1;
    private SampleBase _activeSample;


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

      // Add the physics simulation to the service. 
      _simulation = new Simulation();
      // We do not add the standard force effects because our bodies should only 
      // be modified by Kinect data!
      //_simulation.ForceEffects.Add(new Gravity());
      //_simulation.ForceEffects.Add(new Damping());
      Services.AddService(typeof(Simulation), _simulation);

      // ----- Add GameComponents
      Components.Add(new Camera(this));                         // Controls the camera.
      Components.Add(new RigidBodyRenderer(this, _simulation)); // Renders rigid bodies for debugging.
      Components.Add(new Help(this) { DrawOrder = 30 });        // Draws help text.      
      Components.Add(new KinectWrapper(this));                  // Wraps the Kinect sensor.

      // Initialize the sample factory methods. All samples must be added to this 
      // array to be shown.
      _createSampleDelegates = new Func<SampleBase>[]
      {
        () => new SkeletonMappingSample(this), 
        () => new RagdollMarionetteSample(this), 
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

      // Update physics simulation.
      _simulation.Update(deltaTime);

      // Update the game components.
      base.Update(gameTime);
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


    protected override void Draw(GameTime gameTime)
    {
      // Just clear the background.
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // The actual rendering is done in the game components.
      base.Draw(gameTime);
    }
  }
}
