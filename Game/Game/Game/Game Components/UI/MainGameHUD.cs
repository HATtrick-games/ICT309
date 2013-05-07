using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using Microsoft.Xna.Framework;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ICT309Game.GameObjects.Board;

namespace ICT309Game.Game_Components.UI
{
    class MainGameHUD : UIScreen
    {
        TextBlock _text;
        Button _turnButton;
        Image _hudOverlay;
        Image _currentCharacterImage;
        
        public TurnManager TurnManagerObject { get; set; }

        public bool EndButtonClicked { get; private set; }

        public MainGameHUD(string name, IUIRenderer renderer, TurnManager turnManager)
            : base(name, renderer)
        {
            TurnManagerObject = turnManager;
        }

        protected override void OnLoad()
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();

            _text = new TextBlock
            {
                Text = "Current Turn",
                Background = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            _turnButton = new Button
            {
                Margin = new Vector4F(10),
                Background = new Color(0.0f, 0.0f, 0.0f),
                Content = new TextBlock { Text = "End Turn" },
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            _turnButton.Click += (s, e) => EndButtonClicked = true;

            _currentCharacterImage = new Image
            {
                Width = 80,
                Height = 80,
                Texture = null,
                X = 20,
                Y = 630,
            };

            _hudOverlay = new Image
            {
                Texture = content.Load<Texture2D>("UI/UIOverlay"),
            };

            Children.Add(_hudOverlay);
            Children.Add(_currentCharacterImage);
            Children.Add(_text);
            Children.Add(_turnButton);

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            Children.Add(gameLog);

            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            EndButtonClicked = false;
            _text.Text = "Current Turn " + TurnManagerObject.CurrentTurn.CharacterName;

            _currentCharacterImage.Texture = TurnManagerObject.CurrentTurn.Image;

            if (TurnManagerObject.CurrentTurnStatus == TurnStatus.MOVEMENT)
            {
                _turnButton.Content = new TextBlock { Text = "End Movement Phase" };
            }
            else
            {
                _turnButton.Content = new TextBlock { Text = "End Turn" };
            }

            base.OnUpdate(deltaTime);
        }

        protected override void OnRender(UIRenderContext context)
        {
            _hudOverlay.Render(context);

            _text.Render(context);

            _turnButton.IsVisible = TurnManagerObject.CurrentTurn.isAlly;
            _turnButton.Render(context);

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            gameLog.Render(context);

            _currentCharacterImage.Render(context);

            base.OnRender(context);
        }
    }
}
