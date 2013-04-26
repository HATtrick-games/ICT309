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

namespace ICT309Game.GameObjects
{
    class SquareObject:GameObject
    {
        private ModelNode _model;
        private Vector3F _position;

        public SquareObject(Vector3F position)
        {
            _position = position;
        }

        protected override void OnLoad()
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var screen = ((GameScreen)graphicsService.Screens["Default"]);

            _model = contentManager.Load<ModelNode>("square").Clone();

            screen.Scene.Children.Add(_model);

            _model.PoseWorld = new Pose(_position);

            base.OnLoad();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

        protected override void OnUnload()
        {
            _model.Dispose();
            base.OnUnload();
        }

        private void ChangeColor()
        {
            foreach (var meshNode in _model.GetSubtree().OfType<MeshNode>())
            {
                Mesh mesh = meshNode.Mesh;

                foreach (var material in mesh.Materials)
                {
                    var effectBinding = material["Default"];

                    effectBinding.Set("DiffuseColor", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

                    ((BasicEffectBinding)effectBinding).LightingEnabled = false;
                }
            }

        }
    }
}
