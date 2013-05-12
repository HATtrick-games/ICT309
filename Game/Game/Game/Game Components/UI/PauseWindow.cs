using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Game.UI;
using ICT309Game.Game_Components.UI;

namespace ICT309Game.Game_Components.UI
{
    class PauseWindow : Window
    {
        public PauseWindow()
        {
            HorizontalAlignment = DigitalRune.Game.UI.HorizontalAlignment.Center;
            VerticalAlignment = DigitalRune.Game.UI.VerticalAlignment.Center;

            Title = "Game Paused";

            CloseButtonStyle = null;

            var resumeButton = new Button
            {
                Margin = new Vector4F(10),
                Width = 200,
                Height = 60,
                Content = new TextBlock {  Text = "Resume" },
                IsDefault = true,
                IsCancel = true,
            };
            resumeButton.Click += (s, e) => Close();

            var optionsButton = new Button
            {
                Margin = new Vector4F(10),
                Width = 200,
                Height = 60,
                Content = new TextBlock { Text = "Options" },
            };
            optionsButton.Click += (s, e) => new OptionsWindow().Show(this);

            var endGameButton = new Button
            {
                Margin = new Vector4F(10),
                Width = 200,
                Height = 60,
                Content = new TextBlock { Text = "Back to Main Menu" },
            };
            endGameButton.Click += (s, e) => DialogResult = false;

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            stackPanel.Children.Add(resumeButton);
            stackPanel.Children.Add(optionsButton);
            stackPanel.Children.Add(endGameButton);

            Content = stackPanel;
        }
    }
}
