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

        public MainGameHUD(string name, IUIRenderer renderer)
            : base(name, renderer)
        {
        }

        protected override void OnLoad()
        {

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
                Width = 200,
                Height = 60,
                Content = new TextBlock { Text = "End Turn" },
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            Children.Add(_text);
            Children.Add(_turnButton);

            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            //System.Console.WriteLine("GameHUD Update");
            _text.Text = "Current Turn " + CurrentCharacterName;

            base.OnUpdate(deltaTime);
        }

        protected override void OnRender(UIRenderContext context)
        {
            _text.Render(context);
            _turnButton.Render(context);

            base.OnRender(context);
        }
    }
}
