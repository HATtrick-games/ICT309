using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ICT309Game.Graphics
{
    class BasicScreen : DelegateGraphicsScreen
    {
        public Scene Scene { get; set; }
        public CameraNode ActiveCamera { get; set; }

        private MeshRenderer _renderer;

        public BasicScreen(IGraphicsService graphics)
            : base(graphics)
        {
            Scene = new Scene();
            ActiveCamera = null;
            _renderer = new MeshRenderer();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            Scene.Update(deltaTime);

            base.OnUpdate(deltaTime);
        }

        protected override void OnRender(RenderContext context)
        {
            if (ActiveCamera == null)
                return;

            context.CameraNode = ActiveCamera;
            context.Scene = Scene;

            var graphicsDevice = context.GraphicsService.GraphicsDevice;

            graphicsDevice.Clear(Color.CornflowerBlue);

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            var query = Scene.Query<CameraFrustumQuery>(ActiveCamera);

            context.RenderPass = "Default";
            _renderer.Render(query.SceneNodes, context);

            context.RenderPass = null;
            context.CameraNode = null;
            context.Scene = null;

            base.OnRender(context);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                GraphicsService.Screens.Clear();
                Scene.Dispose();
            }
        }
    }
}
