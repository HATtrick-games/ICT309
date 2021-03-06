﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ICT309Game.GameObjects;
using ICT309Game.Save;
using Microsoft.Practices.ServiceLocation;
using DigitalRune.Game.UI.Controls;
using Microsoft.Xna.Framework.Content;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Game.UI;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace ICT309Game.Game_Components
{
    /// <summary> End level component.</summary>
    class EndLevelComponent : DrawableGameComponent
    {
        private UIScreen _endLevelUI;

        TextBlock _gameOver;

        Image _background;

        Button _nextLevel;
        Button _saveGame;
        Button _returnToMenu;

        TextBlock _display;

        Song _backgroundMusic;

        ContentManager content = ServiceLocator.Current.GetInstance<ContentManager>();

        /// <summary> Constructor.</summary>
        ///
        /// <param name="game">    The game.</param>
        /// <param name="allyWin"> true to ally window.</param>
        public EndLevelComponent(Game game, bool allyWin)
            : base(game)
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();
            var theme = content.Load<Theme>("UI/UITheme");
            var renderer = new UIRenderer(this.Game, theme);

            _endLevelUI = new UIScreen("EndLevelUI", renderer);

            var uiService = ServiceLocator.Current.GetInstance<IUIService>();
            uiService.Screens.Add(_endLevelUI);

            _returnToMenu = new Button
            {
                Margin = new Vector4F(10),
                Width = 300,
                Height = 60,
                Y = 480,
                X = 200,
                Content = new TextBlock { Text = "Return to Menu", }
            };
            _returnToMenu.Click += (s, e) => EndToMenu();

            _background = new Image
            {
                Texture = content.Load<Texture2D>("UI/background"),
            };

            _endLevelUI.Children.Add(_background);
            _endLevelUI.Children.Add(_returnToMenu);
             
            if (GameSettings.LevelNumber == 1)
            {
                GameSettings.LevelNumber = 0;
                LoadCredits();
            }
            else if(allyWin)
            {
                GameSettings.LevelNumber++;
                AllyWin();
            }
            else
            {
                EnemyWin();
            }

            MediaPlayer.Play(_backgroundMusic);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary> Ends to menu.</summary>
        private void EndToMenu()
        {
            Game.Components.Remove(this);
            Game.Components.Add(new MainMenuComponent(Game));
            Dispose(true);
        }

        /// <summary> Loads the credits.</summary>
        private void LoadCredits()
        {
            _backgroundMusic = content.Load<Song>("SoundFX/end_level");
            _gameOver = new TextBlock
            {
                Font = "stattext",
                Text = "Thanks for playing! This has been another HATtrick Games production from \n the awesome people, Hamish Carrier, Arran Ford and Timothy Veletta. \n Remember this game is licensed under the BEER-WARE license, \n so if you like what we do, feel free to buy us a beer in return.",
                Width = 500,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _endLevelUI.Children.Add(_gameOver);
        }

        /// <summary> Ally win.</summary>
        private void AllyWin()
        {
            _backgroundMusic = content.Load<Song>("SoundFX/end_level");
            _nextLevel = new Button
            {
                Margin = new Vector4F(10),
                Width = 300,
                Height = 60,
                Y = 360,
                X = 200,
                Content = new TextBlock { Text = "Continue", }
            };
            _nextLevel.Click += (s, e) => ReturnToGame(); 

            _saveGame = new Button
            {
                Margin = new Vector4F(10),
                Width = 300,
                Height = 60,
                Y = 420,
                X = 200,
                Content = new TextBlock { Text = "Save Game", }
            };
            _saveGame.Click += (s, e) => SaveLoadGame.InitiateSave(GameSettings.LevelNumber);

            _display = new TextBlock
            {
                Text = "YOU WIN!",
                HorizontalAlignment = HorizontalAlignment.Center,
                Y = 100,
            };

            _endLevelUI.Children.Add(_display);
            _endLevelUI.Children.Add(_nextLevel);
            _endLevelUI.Children.Add(_saveGame);
        }

        /// <summary> Enemy win.</summary>
        private void EnemyWin()
        {
            _backgroundMusic = content.Load<Song>("SoundFX/enemy_win");
            _nextLevel = new Button
            {
                Margin = new Vector4F(10),
                Width = 300,
                Height = 60,
                Y = 360,
                X = 200,
                Content = new TextBlock { Text = "Try Again?", }
            };
            _nextLevel.Click += (s, e) => ReturnToGame();

            _display = new TextBlock
            {
                Text = "YOU LOSE!",
                HorizontalAlignment = HorizontalAlignment.Center,
                Y = 100,
            };

            _endLevelUI.Children.Add(_display);
            _endLevelUI.Children.Add(_nextLevel);
        }

        /// <summary> Releases the unmanaged resources used by the
        ///           DrawableGameComponent and optionally releases the managed
        ///           resources.</summary>
        ///
        /// <param name="disposing"> true to release both managed and unmanaged
        ///                          resources; false to release only unmanaged
        ///                          resources.</param>
        protected override void Dispose(bool disposing)
        {
            MediaPlayer.Stop();
            var uiService = ServiceLocator.Current.GetInstance<IUIService>();
            if (uiService != null) uiService.Screens.Remove(_endLevelUI);

            base.Dispose(disposing);
        }

        /// <summary> Returns to game.</summary>
        public void ReturnToGame()
        {
            if (GameSettings.LevelNumber > 0)
            {
                Game.Components.Remove(this);
                Game.Components.Add(new CutSceneComponent(Game, "Cutscenes/CutScene2"));
                Dispose(true);
            }
            else
            {
                Game.Components.Remove(this);
                Game.Components.Add(new MainGameComponent(Game));
                Dispose(true);
            }
            
        }
    }
}
