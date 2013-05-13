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
using DigitalRune.Graphics.SceneGraph;
using ICT309Game.GameObjects.AI;
using Microsoft.Xna.Framework.Input;
using DigitalRune.Game.UI.Controls;
using ICT309Game.Levels;

namespace ICT309Game.Game_Components
{
    class MainGameComponent: DrawableGameComponent
    {
        private BasicScreen _gameScreen;
        private MainGameHUD _gameHUD;

        // GAME OBJECTS
        private GameBoardManagerObject _gameBoardManager;
        private AIHandlerObject _aiHandler;

        public MainGameComponent(Game game):base(game)
        {
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();

            _gameScreen = new BasicScreen(graphicsService)
            {
                Name = "Default",
            };
            graphicsService.Screens.Add(_gameScreen);
        }

        public override void Initialize()
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var theme = Game.Content.Load<Theme>("UI/UITheme");
            var renderer = new UIRenderer(Game, theme);

            _gameBoardManager = new GameBoardManagerObject(new FirstLevel());
            _aiHandler = new AIHandlerObject();

            // Create the inital game objects
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
            gameObjectService.Objects.Add(new CameraObject());
            gameObjectService.Objects.Add(_aiHandler);
            gameObjectService.Objects.Add(_gameBoardManager);

            _gameHUD = new MainGameHUD("GameHUD", renderer, _gameBoardManager.TurnManager);

            var uiService = ServiceLocator.Current.GetInstance<IUIService>();
            uiService.Screens.Add(_gameHUD);
        }

        public override void Update(GameTime gameTime)
        {
            var inputService = ServiceLocator.Current.GetInstance<IInputService>();

            if (_gameHUD.EndButtonClicked)
            {
                if (_gameBoardManager.TurnManager.CurrentTurnStatus == TurnStatus.MOVEMENT) _gameBoardManager.TurnManager.ChangeStatus();
                else _gameBoardManager.TurnManager.ChangeTurn();
            }

            _aiHandler.IsAITurn = !_gameBoardManager.TurnManager.CurrentTurn.isAlly;

            if (_aiHandler.IsAITurn) _gameBoardManager.TurnManager.ChangeTurn();

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
                if(gameObjectService != null) gameObjectService.Objects.Clear();

                var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
                if(graphicsService != null) graphicsService.Screens.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
