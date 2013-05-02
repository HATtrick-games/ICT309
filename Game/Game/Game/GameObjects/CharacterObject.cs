using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.Graphics;
using Microsoft.Xna.Framework.Content;
using DigitalRune.Graphics;
using DigitalRune.Animation;
using Microsoft.Xna.Framework;
using DigitalRune.Graphics.Effects;
using ICT309Game.GameObjects.Board;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.GameObjects
{
    class CharacterObject : GameObject
    {
        protected ModelNode _model;
        protected Vector4 _color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        public static readonly int NameID =
            CreateProperty<String>("Name", "Common", "Defines the name of the character.", "Character").Id;

        public static readonly int HitPointsID = 
            CreateProperty<int>("HP", "Common", "Defines the number of hit points remaining.", 100).Id;

        public static readonly int DamageID = 
            CreateProperty<int>("Damage", "Common", "Defines the amount of damage done with attacks.", 0).Id;

        public static readonly int ArmorDamageID =
            CreateProperty<int>("ArmorDamage", "Common", "Defines the amount of damage done to armor.", 0).Id;

        public static readonly int ArmorID =
            CreateProperty<int>("Armor", "Common", "Defines the amount of damage reduced by attacks.", 0).Id;

        public static readonly int RangeID =
            CreateProperty<int>("Range", "Common", "Defines the distance at which a character can attack.", 0).Id;

        public static readonly int MovementID =
            CreateProperty<int>("Movement", "Common", "Defines the distance the character can move in a turn.", 2).Id;

        public static readonly int PositionXID =
            CreateProperty<int>("PosX", "Common", "Defines the X Position on the game board.", 0).Id;

        public static readonly int PositionYID =
            CreateProperty<int>("PosY", "Common", "Defines the Y Position on the game board.", 0).Id;

        public String CharacterName
        {
            get { return GetValue<String>(NameID); }
            set { SetValue(NameID, value); }
        }

        public int HitPoints
        {
            get { return GetValue<int>(HitPointsID); }
            set { SetValue(HitPointsID, value); }
        }

        public int Damage
        {
            get { return GetValue<int>(DamageID); }
            set { SetValue(DamageID, value); }
        }

        public int ArmorDamage
        {
            get { return GetValue<int>(ArmorDamageID); }
            set { SetValue(ArmorDamageID, value); }
        }

        public int Armor
        {
            get { return GetValue<int>(ArmorID); }
            set { SetValue(ArmorID, value); }
        }

        public int Range
        {
            get { return GetValue<int>(RangeID); }
            set { SetValue(RangeID, value); }
        }

        public int Movement
        {
            get { return GetValue<int>(MovementID); }
            set { SetValue(MovementID, value); }
        }

        public int PosX
        {
            get { return GetValue<int>(PositionXID); }
            set { SetValue(PositionXID, value); }
        }

        public int PosY
        {
            get { return GetValue<int>(PositionYID); }
            set { SetValue(PositionYID, value); }
        }

        public bool isTurn = false;
        public bool isAlly { get; protected set; }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            _model.PoseWorld = new Pose(GameBoardManagerObject.Positions[PosX, PosY]);

            base.OnUpdate(deltaTime);
        }

        protected override void OnUnload()
        {
            if(_model != null) _model.Dispose();

            base.OnUnload();
        }

        protected void LoadModel(string filepath)
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((GameScreen)graphicsService.Screens["Default"]);

            _model = contentManager.Load<ModelNode>(filepath).Clone();

            foreach (var meshNode in _model.GetSubtree().OfType<MeshNode>())
            {
                Mesh mesh = meshNode.Mesh;
                foreach (var material in mesh.Materials)
                {
                    var effectBinding = material["Default"];
                    effectBinding.Set("DiffuseColor", _color);
                    ((BasicEffectBinding)effectBinding).LightingEnabled = false;
                }
            }

            screen.Scene.Children.Add(_model);
        }
    }
}
