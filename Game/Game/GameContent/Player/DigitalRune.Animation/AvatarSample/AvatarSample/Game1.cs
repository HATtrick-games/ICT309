using System;
using DigitalRune.Animation;
using DigitalRune.Game.Input;
using DigitalRune.Physics;
using DigitalRune.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;


namespace AvatarSample
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager _graphicsManager;
    private AnimationManager _animationManager;
    private InputManager _inputManager;
    private Simulation _simulation;

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

      // Add the physics simulation to the service and add some force effects. 
      // (TODO: Create a IPhysicsService that owns the simulation.)
      _simulation = new Simulation();
      _simulation.ForceEffects.Add(new Gravity());
      _simulation.ForceEffects.Add(new Damping());
      Services.AddService(typeof(Simulation), _simulation);

      // ----- Add GameComponents
      Components.Add(new GamerServicesComponent(this));               // XNA gamer services needed for avatars.
      Components.Add(new Camera(this));                               // Controls the camera.
      Components.Add(new BallShooter(this));                          // Shoot balls at the target position.
      Components.Add(new Grab(this));                                 // Allows to grab objects with the mouse/gamepad
      Components.Add(new RigidBodyRenderer(this) { DrawOrder = 10 }); // Renders all rigid bodies of the simulation.
      Components.Add(new Reticle(this) { DrawOrder = 20 });           // Draws a cross-hair for aiming.
      Components.Add(new Help(this) { DrawOrder = 30 });              // Draws help text.      
      Components.Add(new Profiler(this) { DrawOrder = 40 });          // Displays profiling data.            

      // Initialize the sample factory methods. All samples must be added to this 
      // array to be shown.
      _createSampleDelegates = new Func<Sample>[]
      {
        () => new BasicAvatarSample(this), 
        () => new WrappedAnimationSample(this), 
        () => new BakedAnimationSample(this), 
        () => new CustomAnimationSample(this), 
        () => new AttachmentSample(this), 
        () => new RagdollSample(this), 
        () => new AdvancedRagdollSample(this), 
      };

      // Start the first sample in the array.
      _activeSampleIndex = 0;
      _activeSample = _createSampleDelegates[0]();
      Components.Add(_activeSample);

      base.Initialize();
    }


    protected override void Update(GameTime gameTime)
    {
      // Tell the profiler that a new frame has begun.
      Profiler.MainProfiler.NewFrame();

      Profiler.MainProfiler.Start("Update");
      {
        TimeSpan deltaTime = gameTime.ElapsedGameTime;

        // Update input manager. The input manager gets the device states and performs other work.
        Profiler.MainProfiler.Start("InputManager");
        _inputManager.Update(deltaTime);
        Profiler.MainProfiler.Stop();

        // If Escape or Back is pressed, the game exits.
        if (_inputManager.IsDown(Keys.Escape) || _inputManager.IsPressed(Buttons.Back, false, PlayerIndex.One))
        {
          Exit();
        }

        SwitchSamples();

        // Update the game components.
        Profiler.MainProfiler.Start("Game Components");
        base.Update(gameTime);
        Profiler.MainProfiler.Stop();

        Profiler.MainProfiler.Start("AnimationManager");
        {
          // Update the animations. (The animations results are stored internally but not yet applied).
          Profiler.MainProfiler.Start("AnimationManager.Update");
          _animationManager.Update(deltaTime);
          Profiler.MainProfiler.Stop();

          // Apply the animations. (This method changes the animated objects. It must be called
          // somewhere where it is thread-safe to change animated objects.)
          Profiler.MainProfiler.Start("AnimationManager.ApplyAnimations");
          _animationManager.ApplyAnimations();
          Profiler.MainProfiler.Stop();
        }
        Profiler.MainProfiler.Stop();

        // Update physics simulation.
        Profiler.MainProfiler.Start("Simulation");
        _simulation.Update(deltaTime);
        Profiler.MainProfiler.Stop();
      }
      Profiler.MainProfiler.Stop();

      // Reset profiler if <H> or <Left Shoulder> are pressed.
      if (_inputManager.IsPressed(Keys.H, false) || _inputManager.IsPressed(Buttons.LeftShoulder, false, PlayerIndex.One))
        Profiler.MainProfiler.Reset();
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
      Profiler.MainProfiler.Start("Draw");

      GraphicsDevice.Clear(new Color(119, 160, 217));      
      base.Draw(gameTime);

      Profiler.MainProfiler.Stop();
    }
  }
}
