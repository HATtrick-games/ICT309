using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using Microsoft.Xna.Framework;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.Game_Components.UI
{
    class MainGameHUD : UIScreen
    {
        TextBlock _text;
        Button _turnButton;

        public String CurrentCharacterName { get; set; }
        public bool PlayerTurn { get; set; }
        public bool MovementPhase { get; set; }

        public bool EndButtonClicked { get; private set; }

        public MainGameHUD(string name, IUIRenderer renderer)
            : base(name, renderer)
        {
            
        }

        protected override void OnLoad()
        {
            CurrentCharacterName = " ";
            PlayerTurn = false;
            MovementPhase = false;

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
            

            Children.Add(_text);
            Children.Add(_turnButton);

            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            EndButtonClicked = false;
            _text.Text = "Current Turn " + CurrentCharacterName;

            if (MovementPhase)
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
            _text.Render(context);

            _turnButton.IsVisible = PlayerTurn;
            _turnButton.Render(context);

            base.OnRender(context);
        }
    }
}
