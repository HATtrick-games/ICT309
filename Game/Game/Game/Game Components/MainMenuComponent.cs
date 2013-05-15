using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DigitalRune.Game.UI;
using DigitalRune.Game.Input;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.Game_Components.UI;
using Microsoft.Xna.Framework.Content;
using DigitalRune.Game.UI.Rendering;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.Game_Components
{
    class MainMenuComponent : DrawableGameComponent
    {
        //private MenuUI _menuUI;

        private UIScreen _menuUI;

        Button _startGame;
        Button _about;
        Button _exit;

        public MainMenuComponent(Game game) : base(game)
        {
            
        }

        public override void Initialize()
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();
            var theme = content.Load<Theme>("UI/UITheme");
            var renderer = new UIRenderer(this.Game, theme);

            _menuUI = new UIScreen("MenuUI", renderer);

            var uiService = ServiceLocator.Current.GetInstance<IUIService>();
            uiService.Screens.Add(_menuUI);

            _startGame = new Button
            {
                Margin = new Vector4F(10),
                Width = 200,
                Height = 60,
                Y = 330,
                X = 200,
                Content = new TextBlock { Text = "Start Game" },
            };
            _startGame.Click += (s, e) => MenuToGame();

            _about = new Button
            {
                Margin = new Vector4F(10),
                Width = 200,
                Height = 60,
                Y = 410,
                X = 200,
                Content = new TextBlock { Text = "About Us" },
            };

            var Game = ServiceLocator.Current.GetInstance<Game>();
            _exit = new Button
            {
                Margin = new Vector4F(10),
                Width = 200,
                Height = 60,
                X = 200,
                Y = 480,
                Content = new TextBlock { Text = "Exit" },
            };
            _exit.Click += (s, e) => Game.Exit();

            _menuUI.Children.Add(_startGame);
            _menuUI.Children.Add(_about);
            _menuUI.Children.Add(_exit);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            _menuUI.Children.RemoveRange(0, _menuUI.Children.Count);

            var uiService = ServiceLocator.Current.GetInstance<IUIService>();
            if(uiService != null) uiService.Screens.Remove(_menuUI);

            base.Dispose(disposing);
        }

        public void MenuToGame()
        {
            Game.Components.Remove(this);
            Game.Components.Add(new MainGameComponent(Game));
            Dispose(true);
        }
    }
}
