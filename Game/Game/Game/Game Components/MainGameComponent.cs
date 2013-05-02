using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DigitalRune.Game.Input;
using DigitalRune.Game.UI;
using DigitalRune.Graphics;
using DigitalRune.Game;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.Graphics;
using ICT309Game.GameObjects;
using ICT309Game.GameObjects.Board;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Geometry;
using ICT309Game.Game_Components.UI;
using Microsoft.Xna.Framework.Content;
using DigitalRune.Game.UI.Rendering;

namespace ICT309Game.Game_Components
{
    class MainGameComponent: GameComponent
    {
        private GameScreen _gameScreen;
        private MainGameHUD _gameHUD;

        private readonly IInputService _inputService;
        private readonly IUIService _uiService;
        private readonly IGraphicsService _graphicsService;
        private readonly IGameObjectService _gameObjectService;

        // GAME OBJECTS
        private GameBoardManagerObject _gameBoardManager;

        public MainGameComponent(Game game):base(game)
        {
            _inputService = ServiceLocator.Current.GetInstance<IInputService>();
            _uiService = ServiceLocator.Current.GetInstance<IUIService>();
            _graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            _gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
        }

        public override void Initialize()
        {
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            _gameScreen = new GameScreen(graphicsService)
            {
                Name = "Default"
            };
            graphicsService.Screens.Add(_gameScreen);

            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var theme = Game.Content.Load<Theme>("UI/Theme");
            var renderer = new UIRenderer(Game, theme);

            _gameHUD = new MainGameHUD("GameHUD", renderer);

            _uiService.Screens.Add(_gameHUD);

            _gameBoardManager = new GameBoardManagerObject();

            // Create the inital game objects
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
            gameObjectService.Objects.Add(new CameraObject());
            gameObjectService.Objects.Add(new BoardObject());
            gameObjectService.Objects.Add(_gameBoardManager);
            //gameObjectService.Objects.Add(new MainCharacter());
            //gameObjectService.Objects.Add(new AIRangedCharacter());

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _gameScreen.DebugRenderer.Clear();

            _gameHUD.CurrentCharacterName = _gameBoardManager.TurnManager.CurrentTurn.CharacterName;

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _gameObjectService.Objects.Clear();

                _gameScreen.GraphicsService.Screens.Clear();
                _gameScreen.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
