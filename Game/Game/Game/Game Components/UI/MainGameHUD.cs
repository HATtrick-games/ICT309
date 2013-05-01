using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Game.UI.Rendering;
using Microsoft.Xna.Framework;

namespace ICT309Game.Game_Components.UI
{
    class MainGameHUD : UIScreen
    {
        StackPanel _panel;
        TextBlock _text;

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
                X = 100,
                Y = 100,
            };

            _panel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            _panel.Children.Add(_text);

            Children.Add(_panel);

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
            _panel.Render(context);

            base.OnRender(context);
        }
    }
}
