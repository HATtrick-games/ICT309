using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;
using DigitalRune.Graphics;
using ICT309Game.Graphics;
using Microsoft.Xna.Framework;
using DigitalRune.Graphics.Effects;
using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Geometry.Meshes;
using Microsoft.Xna.Framework.Graphics;
using DigitalRune.Animation;

namespace ICT309Game.GameObjects.Board
{
    class WhiteSquareObject : GameObject
    {
        private ModelNode _model;

        protected Vector3F _position;
        protected Vector4 _color;

        public bool InUse { get; set; }

        public WhiteSquareObject(Vector3F position)
        {
            _position = position;
            _color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var screen = ((BasicScreen)graphicsService.Screens["Default"]);
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();

            _model = contentManager.Load<ModelNode>("Board/whiteSquare").Clone();

            _model.PoseWorld = new Pose(_position);

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

            InUse = true;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (InUse)
            {
                _model.ScaleLocal = Vector3F.One;
            }
            else
            {
                _model.ScaleLocal = Vector3F.Zero;
            }

            base.OnUpdate(deltaTime);
        }
    }
}
