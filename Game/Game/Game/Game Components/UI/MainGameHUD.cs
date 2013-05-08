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
        Button _turnButton;
        Image _hudOverlay;
        Image _currentCharacterImage;
        Image _statsBox;

        Image _endTurnButton;
        Image _endMovementButton;

        TextBlock _health;
        TextBlock _damage;
        TextBlock _range;
        TextBlock _armor;
        TextBlock _armorDamage;
        TextBlock _movement;
        
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

            _endTurnButton = new Image { Texture = content.Load<Texture2D>("UI/endturnbutton"), };
            _endMovementButton = new Image { Texture = content.Load<Texture2D>("UI/endmovementbutton"), };

            _statsBox = new Image 
            { 
                Texture = content.Load<Texture2D>("UI/characterbox"),
                X = 100,
                Y = 630,
            };

            _turnButton = new Button
            {
                Margin = new Vector4F(10),
                X = 1010,
                Y = 630,
                Content = _endTurnButton,
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

            _health = new TextBlock
            {
                Font = "stattext",
                Text = "100",
                X = 130,
                Y = 640,
            };

            _damage = new TextBlock
            {
                Font = "stattext",
                Text = "100",
                X = 130,
                Y = 662,
            };

            _range= new TextBlock
            {
                Font = "stattext",
                Text = "100",
                X = 130,
                Y = 684,
            };

            _armor = new TextBlock
            {
                Font = "stattext",
                Text = "100",
                X = 215,
                Y = 640,
            };

            _armorDamage = new TextBlock
            {
                Font = "stattext",
                Text = "100",
                X = 215,
                Y = 662,
            };

            _movement = new TextBlock
            {
                Font = "stattext",
                Text = "100",
                X = 215,
                Y = 684,
            };

            Children.Add(_hudOverlay);
            Children.Add(_currentCharacterImage);
            Children.Add(_turnButton);
            Children.Add(_statsBox);

            Children.Add(_health);
            Children.Add(_damage);
            Children.Add(_range);
            Children.Add(_armor);
            Children.Add(_armorDamage);
            Children.Add(_movement);

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            Children.Add(gameLog);

            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            EndButtonClicked = false;

            _currentCharacterImage.Texture = TurnManagerObject.CurrentTurn.Image;

            if (TurnManagerObject.CurrentTurnStatus == TurnStatus.MOVEMENT)
            {
                _turnButton.Content = _endMovementButton;
            }
            else
            {
                _turnButton.Content = _endTurnButton;
            }

            _health.Text = TurnManagerObject.CurrentTurn.HitPoints.ToString() + " / " + TurnManagerObject.CurrentTurn.MaxHitPoints.ToString();
            _damage.Text = TurnManagerObject.CurrentTurn.Damage.ToString();
            _range.Text = TurnManagerObject.CurrentTurn.Range.ToString();
            _armor.Text = TurnManagerObject.CurrentTurn.Armor.ToString();
            _armorDamage.Text = TurnManagerObject.CurrentTurn.ArmorDamage.ToString();
            _movement.Text = TurnManagerObject.CurrentTurn.Movement.ToString();

            base.OnUpdate(deltaTime);
        }

        protected override void OnRender(UIRenderContext context)
        {
            _hudOverlay.Render(context);

            _turnButton.IsVisible = TurnManagerObject.CurrentTurn.isAlly;
            _turnButton.Render(context);

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            gameLog.Render(context);

            _currentCharacterImage.Render(context);

            base.OnRender(context);
        }
    }
}
