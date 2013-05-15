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
using Microsoft.Xna.Framework.Graphics;
using DigitalRune.Animation.Character;
using DigitalRune.Game.UI;


namespace ICT309Game.GameObjects
{
    class CharacterObject : GameObject
    {
        public Vector3F MovingPos;
        public bool Moving;
        protected Model _model;
        private Pose _pose = new Pose(new Vector3F(0.5f, 0, 0));
        private SkeletonPose _skeletonPose;
        public int ID;
        public int RotationAmount;

       // protected ModelNode _model;
        protected Vector4 _color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        private static ContentManager Content = ServiceLocator.Current.GetInstance<ContentManager>();

        public static readonly int NameID =
            CreateProperty<String>("Name", "Common", "Defines the name of the character.", "Character").Id;

        public static readonly int HitPointsID = 
            CreateProperty<int>("HP", "Common", "Defines the number of hit points remaining.", 100).Id;

        public static readonly int MaxHitPointsID =
            CreateProperty<int>("MaxHP", "Common", "Defines the maximum number of hit points.", 100).Id;

        public static readonly int DamageID = 
            CreateProperty<int>("Damage", "Common", "Defines the amount of damage done with attacks.", 0).Id;

        public static readonly int ArmorDamageID =
            CreateProperty<int>("ArmorDamage", "Common", "Defines the amount of damage done to armor.", 0).Id;

        public static readonly int ArmorID =
            CreateProperty<int>("Armor", "Common", "Defines the amount of damage reduced by attacks.", 0).Id;

        public static readonly int MinArmorID =
            CreateProperty<int>("MinArmor", "Common", "Defines the minimum armor value for the character", 0).Id;

        public static readonly int RangeID =
            CreateProperty<int>("Range", "Common", "Defines the distance at which a character can attack.", 1).Id;

        public static readonly int MovementID =
            CreateProperty<int>("Movement", "Common", "Defines the distance the character can move in a turn.", 2).Id;

        public static readonly int PositionXID =
            CreateProperty<int>("PosX", "Common", "Defines the X Position on the game board.", 0).Id;

        public static readonly int PositionYID =
            CreateProperty<int>("PosY", "Common", "Defines the Y Position on the game board.", 0).Id;

        public static readonly int ImageID =
            CreateProperty<Texture2D>("Character Image", "Common", "Defines the characters splash image.", Content.Load<Texture2D>("Placeholder")).Id;

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

        public int MaxHitPoints
        {
            get { return GetValue<int>(MaxHitPointsID); }
            set { SetValue(MaxHitPointsID, value); }
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

        public int MinArmor
        {
            get { return GetValue<int>(ArmorID); }
            set { SetValue(MinArmorID, value); }
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

        public Texture2D Image
        {
            get { return GetValue<Texture2D>(ImageID); }
            set { SetValue(ImageID, value); }
        }


        /*
        public Vector3 Position
        {
            get { return _model.PoseWorld.Position.ToXna(); }
        }
        */

        public bool isTurn = false;
        public bool isAlly { get; protected set; }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        public void Rotation(int rotate)
        {
            RotationAmount = rotate;
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);
            screen.Rotation(ID, rotate);
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);


            if (Moving == false)
            {
                //Console.WriteLine("DIS");
                screen.SetPos(ID, new Pose(GameBoardManagerObject.Positions[PosX, PosY], Matrix33F.CreateRotationY(MathHelper.ToRadians(RotationAmount))));
                //_model.PoseWorld = new Pose(GameBoardManagerObject.Positions[PosX, PosY]);
            }
            else if(Moving == true)
            {
                //Console.WriteLine("MOVING FUCKERE");
                screen.SetPos(ID,  MovingPos);
            }
          

            if (MaxHitPoints < HitPoints) HitPoints = MaxHitPoints;


            base.OnUpdate(deltaTime);
        }

        protected override void OnUnload()
        {

            //if(_model != null) model.Dispose();

            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);

            //screen.Scene.Children.Remove(_model);

           // if(_model != null) _model.Dispose();


            base.OnUnload();
        }

        public void LoopWalk()
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);
            screen.Walk(ID);

        }
        public void Attack()
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);
            screen.Attack(ID);
        }

        public void idle()
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);
            screen.idle(ID);
        }

        public void PauseWalk()
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);
            screen.PauseWalk(ID);
        }

        protected void LoadModel(string filepath)
        {

            Moving = false;
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var animationService = ServiceLocator.Current.GetInstance<IAnimationService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);

            _model = Content.Load<Model>("Player/Archer");
            var additionalData = (Dictionary<string, object>)_model.Tag;
            var skeleton = (Skeleton)additionalData["Skeleton"];
            _skeletonPose = SkeletonPose.Create(skeleton);
           // var animations = (Dictionary<string, SkeletonKeyFrameAnimation>)additionalData["Animations"];
          //  SkeletonKeyFrameAnimation walkAnimation = animations.Values.First();

            //_model = contentManager.Load<ModelNode>(filepath).Clone();

          /*  foreach (var meshNode in _model.GetSubtree().OfType<MeshNode>())
            {
                Mesh mesh = meshNode.Mesh;
                foreach (var material in mesh.Materials)
                {
                    var effectBinding = material["Default"];
                    effectBinding.Set("DiffuseColor", _color);
                    ((BasicEffectBinding)effectBinding).LightingEnabled = false;
                }
            }*/

           ID = screen.Add(_model, _pose, _skeletonPose);
           
            //screen.Scene.Children.Add(_model);
        }
    }
}
