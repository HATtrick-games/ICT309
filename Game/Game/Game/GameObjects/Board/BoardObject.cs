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
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.GameObjects.Board
{
    class BoardObject:GameObject
    {
        private ModelNode _model;

        protected override void OnLoad()
        {
            var contentManager = ServiceLocator.Current.GetInstance<ContentManager>();
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var screen = ((GameScreen)graphicsService.Screens["Default"]);

            _model = contentManager.Load<ModelNode>("Board/testmodel");
            _model = _model.Clone();

            foreach (var meshNode in _model.GetSubtree().OfType<MeshNode>())
            {
                Mesh mesh = meshNode.Mesh;
                foreach (var material in mesh.Materials)
                {
                    var effectBinding = material["Default"];
                    effectBinding.Set("DiffuseColor", new Vector4(1.0f, 1.0f, 1.0f, 1));
                    ((BasicEffectBinding)effectBinding).LightingEnabled = false;
                }
            }

            screen.Scene.Children.Add(_model);

            _model.PoseWorld = new DigitalRune.Geometry.Pose(new Vector3F(0.0f, 0.0f, 0.0f));

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            _model.Dispose();
            base.OnUnload();
        }
    }
}
