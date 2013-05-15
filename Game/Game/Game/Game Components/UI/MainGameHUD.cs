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
using DigitalRune.Game;
using ICT309Game.GameObjects;

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

        List<Image> _healthBars;
        List<Image> _healthAmounts;

        List<Image> _turnListBoxes;
        List<Image> _turnListImages;
        List<Image> _turnListHealth;

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

            _healthBars = new List<Image>(TurnManagerObject.characterList.Count);
            _healthAmounts = new List<Image>(_healthBars.Count);
            _turnListBoxes = new List<Image>();
            _turnListImages = new List<Image>();
            _turnListHealth = new List<Image>();
            for (int i = 0; i < TurnManagerObject.characterList.Count;  i++)
            {
                _healthBars.Add(new Image { Texture = content.Load<Texture2D>("UI/Healthbar") });
                _healthAmounts.Add(new Image { Texture = content.Load<Texture2D>("UI/health") });

                _turnListBoxes.Add(new Image { Texture = content.Load<Texture2D>("UI/turnlistbox"),});
                _turnListImages.Add(new Image {
                    Texture = content.Load<Texture2D>("Placeholder"), 
                    X = 1,
                    Width = 50, Height = 50,
                });
                _turnListHealth.Add(new Image { Texture = content.Load<Texture2D>("UI/turnlisthealth"), X = 1 });
            }

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

            foreach (Image bar in _healthBars) Children.Add(bar);
            foreach (Image bar in _healthAmounts) Children.Add(bar);

            Children.Add(_hudOverlay);

            foreach (Image bar in _turnListBoxes) Children.Add(bar);
            foreach (Image bar in _turnListImages) Children.Add(bar);
            foreach (Image bar in _turnListHealth) Children.Add(bar);

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

            // draw health bars
            var gameObjectService = ServiceLocator.Current.GetInstance<IGameObjectService>();
            CameraObject camera;
            gameObjectService.Objects.TryGet("Camera", out camera);
            Matrix mat = Matrix.Identity * camera.View * camera.Projection;

            for (int i = 0; i < _healthBars.Count; i++)
            {
                if (i < TurnManagerObject.characterList.Count)
                {


                    Vector4 v4 = Vector4.Transform(GameBoardManagerObject.Positions[TurnManagerObject.characterList[i].PosX, TurnManagerObject.characterList[i].PosX].ToXna() + new Vector3(0.0f, 40.0f, 0.0f), mat);
                    var pt = new Point((int)((v4.X / v4.W + 1) * (1280 / 2)), (int)((1 - v4.Y / v4.W) * (720 / 2)));

                    _healthBars[i].X = pt.X - 25;
                    _healthBars[i].Y = pt.Y;

                    _healthAmounts[i].Width = (int)(((float)TurnManagerObject.characterList[i].HitPoints / (float)TurnManagerObject.characterList[i].MaxHitPoints) * 48.0f);
                    _healthAmounts[i].X = pt.X - 24;
                    _healthAmounts[i].Y = pt.Y + 1;

                    if (i != 0)
                    {
                        _turnListBoxes[i].Y = 617 - (i * 58);
                        _turnListImages[i].Texture = TurnManagerObject.characterList[i].Image;
                        _turnListImages[i].Y = 618 - (i * 58);
                        _turnListHealth[i].Y = 669 - (i * 58);
                        _turnListHealth[i].Width = (int)(((float)TurnManagerObject.characterList[i].HitPoints / (float)TurnManagerObject.characterList[i].MaxHitPoints) * 50.0f);
                    }
                    else
                    {
                        _turnListBoxes[i].IsVisible = false;
                        _turnListHealth[i].IsVisible = false;
                        _turnListImages[i].IsVisible = false;
                    }
                }
                else
                {
                    _healthAmounts[i].IsVisible = false;
                    _healthBars[i].IsVisible = false;

                    _turnListBoxes[i].IsVisible = false;
                    _turnListHealth[i].IsVisible = false;
                    _turnListImages[i].IsVisible = false;
                }
            }

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
