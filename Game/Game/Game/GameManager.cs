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
using ICT309Game.Screens;
using DigitalRune.Game.UI;

namespace ICT309Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameManager : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;

        private StateMachine _screenManager;
        public StateMachine _ScreenManager
        {
            get { return _screenManager; }
            set { _screenManager = value; }
        }

        private GameObjectManager _objectManager;
        public GameObjectManager _ObjectManager
        {
            get { return _objectManager; }
            set { _objectManager = value; }
        }

        private InputManager _inputManager;
        public InputManager _InputManager
        {
            get { return _inputManager; }
            set { _inputManager = value; }
        }

        private UIManager _uiManager;
        public UIManager _UIManager
        {
            get { return _uiManager; }
            set { _uiManager = value; }
        }

        private MenuScreen menuScreen;
        private GameScreen gameScreen;

        public Transition menuToGameTransition;

        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // Adds the input service, which manages device input, button presses etc.
            _inputManager = new InputManager(false);
            Services.AddService(typeof(IInputService), _inputManager);

            // Adds the UI service which manages UI screens
            _uiManager = new UIManager(this, _inputManager);
            Services.AddService(typeof(IUIService), _uiManager);

            Services.AddService(typeof(ContentManager), Content);

            // Game States
            _screenManager = new StateMachine();
            menuScreen = new MenuScreen(this);
            gameScreen = new GameScreen(this);

            menuToGameTransition = new Transition
            {
                Name = "MenuToGame",
                TargetState = gameScreen,
            };

            menuScreen.Transitions.Add(menuToGameTransition);

            _screenManager.States.Add(menuScreen);
            _screenManager.States.Add(gameScreen);

            _screenManager.States.InitialState = menuScreen;

            base.Initialize();
        }

        protected override void LoadContent()
        {

            base.LoadContent();
            
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.ElapsedGameTime;

            _inputManager.Update(deltaTime);
            _screenManager.Update(deltaTime);
            _uiManager.Update(deltaTime);

            base.Update(gameTime);
        }

    }
}
