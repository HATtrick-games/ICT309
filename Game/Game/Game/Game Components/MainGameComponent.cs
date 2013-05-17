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
using Microsoft.Xna.Framework.Media;

namespace ICT309Game.Game_Components
{
    class MainGameComponent: DrawableGameComponent
    {
        private BasicScreen _gameScreen;
        private MainGameHUD _gameHUD;

        // GAME OBJECTS
        private GameBoardManagerObject _gameBoardManager;
        private AIHandlerObject _aiHandler;
        private CameraObject _camera;

        private List<Level> _levelObjects;

        public MainGameComponent(Game game):base(game)
        {
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();

            _levelObjects = new List<Level>();
            _levelObjects.Add(new FirstLevel());
            _levelObjects.Add(new SecondLevel());

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
            var gameSettings = ServiceLocator.Current.GetInstance<GameSettings>();

            _gameBoardManager = new GameBoardManagerObject(_levelObjects[GameSettings.LevelNumber]);

            var content = ServiceLocator.Current.GetInstance<ContentManager>();
            Song music = content.Load<Song>(_levelObjects[GameSettings.LevelNumber]._backgroundSong);
            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true;

            _aiHandler = new AIHandlerObject(_gameBoardManager);
            _camera = new CameraObject();

            // Create the inital game objects
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
            gameObjectService.Objects.Add(_camera);
            gameObjectService.Objects.Add(_gameBoardManager);
            gameObjectService.Objects.Add(_aiHandler);

            _gameHUD = new MainGameHUD("GameHUD", renderer, _gameBoardManager.TurnManager);

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            gameLog.ResetLog();

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

            if (_aiHandler.IsAITurn && _aiHandler.EndAITurn)
            {
                _aiHandler.EndAITurn = false;
                _gameBoardManager.TurnManager.ChangeTurn();
            }

            if (_gameBoardManager.TurnManager.allyWin || _gameBoardManager.TurnManager.enemyWin)
            {
                System.Console.WriteLine("Game over"); 
                Game.Components.Remove(this);
                Dispose(true);
                Game.Components.Add(new EndLevelComponent(Game, _gameBoardManager.TurnManager.allyWin));               
            }

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var uiService = ServiceLocator.Current.GetInstance<IUIService>();
                if(uiService != null) uiService.Screens.Remove(_gameHUD);

                var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
                if(gameObjectService != null) gameObjectService.Objects.Clear();

                var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
                if(graphicsService != null) graphicsService.Screens.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
