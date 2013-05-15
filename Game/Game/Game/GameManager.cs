using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DigitalRune.Game.States;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Graphics;
using DigitalRune.ServiceLocation;
using DigitalRune.Threading;
using Microsoft.Practices.ServiceLocation;
using DigitalRune.Animation;
using ICT309Game.Game_Components;
using DigitalRune.Physics;
using DigitalRune.Game.UI.Controls;
using ICT309Game.Game_Components.UI;
using DigitalRune.Game.UI.Rendering;

namespace ICT309Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameManager : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;

        private ServiceContainer _serviceContainer;

        private GraphicsManager _graphicsManager;
        private InputManager _inputManager;
        private AnimationManager _animationManager;
        private UIManager _uiManager;
        private GameObjectManager _gameObjectManager;
        private DebugRenderer _debugRenderer;
        private SpriteBatch _spriteBatch;

        private GameLog _gameLog;

        private MainGameComponent _mainGameComponent;

        //private Action _updateAnimation;

        //private Task _updateAnimationTask;

        private TimeSpan _deltaTime;

        static GameManager()
        {
            DigitalRune.Licensing.AddSerialNumber("tgCcAcuJg2I1Hs4BYfH2YgY9zwEiACNUaW1vdGh5IFZlbGV0dGEjMSMzI05vbkNvbW1lcmNpYWxAg7LDkM1RVAQDAfwPTE1CWShg8ydjAzN820/FDwY81X7iRsmnEZIi9Zr3Fz46IC9N7KhUL6OSReHCzBLn");
        }

        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferMultiSampling = false,
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            _serviceContainer = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => _serviceContainer);

            _serviceContainer.Register(typeof(Game), null, this);
            _serviceContainer.Register(typeof(ContentManager), null, Content);

            // Adds the input service, which manages device input, button presses etc.
            _inputManager = new InputManager(false);
            _serviceContainer.Register(typeof(IInputService), null, _inputManager);

            // Adds the UI service which manages UI screens
            _uiManager = new UIManager(this, _inputManager);
            _serviceContainer.Register(typeof(IUIService), null, _uiManager);

            _graphicsManager = new GraphicsManager(GraphicsDevice, Window, Content);
            Services.AddService(typeof(IGraphicsService), _graphicsManager);
            _serviceContainer.Register(typeof(IGraphicsService), null, _graphicsManager);

            _animationManager = new AnimationManager();
            _serviceContainer.Register(typeof(IAnimationService), null, _animationManager);

            _gameObjectManager = new GameObjectManager();
            _serviceContainer.Register(typeof(IGameObjectService), null, _gameObjectManager);

            _debugRenderer = new DebugRenderer(_graphicsManager, Content.Load<SpriteFont>("UI/MiramonteBold"));
            _serviceContainer.Register(typeof(DebugRenderer), null, _debugRenderer);

            _serviceContainer.Register(typeof(ContentManager), null, Content);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _serviceContainer.Register(typeof(SpriteBatch), null, _spriteBatch);

            _gameLog = new GameLog();
            _serviceContainer.Register(typeof(GameLog), null, _gameLog);

            var uiTheme = Content.Load<Theme>("UI/UITheme");
            UIRenderer renderer = new UIRenderer(this, uiTheme);

            var screen = new UIScreen("Default", renderer)
            {
                Background = new Color(0, 0, 0, 0),
            };

            _uiManager.Screens.Add(screen);

            _mainGameComponent = new MainGameComponent(this);
            Components.Add(new StartScreenComponent(this));
            Components.Add(new GamerServicesComponent(this));

            //_updateAnimation = () => _animationManager.Update(_deltaTime);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(_deltaTime);

            //_updateAnimationTask.Wait();

            _animationManager.ApplyAnimations();
            //Parallel.RunCallbacks();

            _deltaTime = gameTime.ElapsedGameTime;

            base.Update(gameTime);

            _uiManager.Update(_deltaTime);
            _gameObjectManager.Update(_deltaTime);

            //_updateAnimationTask = Parallel.Start(_updateAnimation);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphicsManager.Update(_deltaTime);

            // Render to back buffer
            _graphicsManager.Render(false);

            foreach (UIScreen screen in _uiManager.Screens)
                screen.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            System.Console.WriteLine("Exiting");
            _mainGameComponent.Dispose();

            _serviceContainer.Clear();

            base.OnExiting(sender, args);
        }
    }
}
