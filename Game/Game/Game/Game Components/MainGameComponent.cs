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

namespace ICT309Game.Game_Components
{
    class MainGameComponent: GameComponent
    {
        private BasicScreen _gameScreen;
        private MainGameHUD _gameHUD;

        // GAME OBJECTS
        private GameBoardManagerObject _gameBoardManager;

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
            var theme = Game.Content.Load<Theme>("UI/Theme");
            var renderer = new UIRenderer(Game, theme);

            _gameHUD = new MainGameHUD("GameHUD", renderer);

            var uiService = ServiceLocator.Current.GetInstance<IUIService>();
            uiService.Screens.Add(_gameHUD);

            _gameBoardManager = new GameBoardManagerObject();

            // Create the inital game objects
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
            gameObjectService.Objects.Add(new CameraObject());
            gameObjectService.Objects.Add(new BoardObject());
            gameObjectService.Objects.Add(_gameBoardManager);
            //gameObjectService.Objects.Add(new MainCharacter());
            //gameObjectService.Objects.Add(new AIRangedCharacter());
        }

        public override void Update(GameTime gameTime)
        {
            //_Scene.Update(gameTime.ElapsedGameTime);
            //_gameScreen.DebugRenderer.Clear();

            //_gameHUD.CurrentCharacterName = _gameBoardManager.TurnManager.CurrentTurn.CharacterName;
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
                gameObjectService.Objects.Clear();

                var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
                graphicsService.Screens.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
