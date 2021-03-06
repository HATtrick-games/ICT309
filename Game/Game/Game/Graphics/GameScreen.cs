﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ICT309Game.Graphics
{
    class GameScreen : GraphicsScreen
    {
        private bool _disposed;

        private readonly SceneRenderer _opaqueSceneRenderer;

        public Scene Scene { get; private set; }

        public CameraNode ActiveCamera { get; set; }

        //public DebugRenderer DebugRenderer { get; private set; }

        public GameScreen(IGraphicsService graphicsService) : base(graphicsService)
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();

            var meshRenderer = new MeshRenderer();

            _opaqueSceneRenderer = new SceneRenderer();
            _opaqueSceneRenderer.Renderers.Add(meshRenderer);

            //DebugRenderer = ServiceLocator.Current.GetInstance<DebugRenderer>();

            Scene = new Scene();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Scene.Dispose();
                    //DebugRenderer.Dispose();
                }

                _disposed = true;
            }
        }

        protected override void OnRender(RenderContext context)
        {
            context.CameraNode = ActiveCamera;

            var graphicsDevice = context.GraphicsService.GraphicsDevice;

            graphicsDevice.Clear(Color.CornflowerBlue);

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            context.RenderPass = "Default";
            context.Scene = Scene;

            SceneQuery SceneQuery = Scene.Query<SceneQuery>(ActiveCamera);
            _opaqueSceneRenderer.Render(SceneQuery.RenderableNodes, context);

            //DebugRenderer.Render(context);

            context.RenderPass = null;
            context.CameraNode = null;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            Scene.Update(deltaTime);
            //DebugRenderer.Update(deltaTime);
        }
    }
}
