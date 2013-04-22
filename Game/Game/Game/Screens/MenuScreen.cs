using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.Input;
using Microsoft.Xna.Framework;
using DigitalRune.Game.UI.Rendering;
using ICT309Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.Screens
{
    class MenuScreen: IScreen
    {
        private readonly GameManager _game;
        private readonly IInputService _inputService;
        private readonly IUIService _uiService;
        private readonly ContentManager _contentService;

        private UIScreen _screen;

        public MenuScreen(Microsoft.Xna.Framework.Game game) : base()
        {
            _game = (GameManager)game;
            _contentService = (ContentManager)game.Services.GetService(typeof(ContentManager));
            _inputService = (IInputService)game.Services.GetService(typeof(IInputService));
            _uiService = (IUIService)game.Services.GetService(typeof(IUIService));
        }

        public override void Initialize()
        {
            System.Console.WriteLine("Menu Screen");
            

            base.Initialize();
        }

        public override void LoadContent()
        {
            var theme = _contentService.Load<Theme>("Theme");

            var renderer = new UIRenderer(_game, theme);

            _screen = new UIScreen("Default", renderer)
            {
                Background = new Color(0, 0, 0, 0),
            };

            _uiService.Screens.Add(_screen);

            AddWindowComponents();

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(TimeSpan gameTime)
        {

            base.Update(gameTime);
        }

        public override void Draw(TimeSpan gameTime)
        {
            _screen.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void AddWindowComponents()
        {
            var menuWindow = new Window
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _screen.Children.Add(menuWindow);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Vector4F(150, 0, 0, 0),
            };
            menuWindow.Content = stackPanel;

            var startButton = new Button
            {
                Name = "StartButton",
                Content = new TextBlock { Text = "Start" },
                FocusWhenMouseOver = true,
            };
            startButton.Click += OnStartButtonClicked;

            stackPanel.Children.Add(startButton);

            startButton.Focus();

            _game.ResetElapsedTime();
        }

        private void OnStartButtonClicked(object sender, EventArgs eventArgs)
        {
            _game._ScreenManager.States.ActiveState.Transitions["MenuToGame"].Fire();
        }
    }
}