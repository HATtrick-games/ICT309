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
    class GameScreen : GraphicsScreen, IDisposable
    {
        private bool _disposed;

        private readonly SceneRenderer _opaqueSceneRenderer;
        private readonly SceneRenderer _transparentSceneRenderer;

        public Scene Scene { get; private set; }

        public CameraNode ActiveCamera { get; set; }

        public DebugRenderer DebugRenderer { get; private set; }

        public GameScreen(IGraphicsService graphicsService) : base(graphicsService)
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();

            var meshRenderer = new MeshRenderer();

            _opaqueSceneRenderer = new SceneRenderer();
            _opaqueSceneRenderer.Renderers.Add(meshRenderer);

            _transparentSceneRenderer = new SceneRenderer();
            _transparentSceneRenderer.Renderers.Add(meshRenderer);

            DebugRenderer = new DebugRenderer(graphicsService, content.Load<SpriteFont>("MiramonteBold"));

            Scene = new Scene();
        }

        ~GameScreen()
        {
            Dispose(false);
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
                    DebugRenderer.Dispose();
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

            graphicsDevice.ResetTextures();

            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            context.RenderPass = "AlphaBlend";
            _transparentSceneRenderer.Render(SceneQuery.RenderableNodes, context, RenderOrder.BackToFront);

            context.RenderPass = null;
            context.CameraNode = null;

            /*
            if (ActiveCamera == null)
                return;

            var renderTargetPool = GraphicsService.RenderTargetPool;
           // var graphicsDevice = GraphicsService.GraphicsDevice;
            var screenRenderTarget = context.RenderTarget;
            var viewport = context.Viewport;

            var width = context.Viewport.Width;
            var height = context.Viewport.Height;
            context.Viewport = new Viewport(0, 0, width, height);

            context.Scene = Scene;
            context.CameraNode = ActiveCamera;

            SceneQuery sceneQuery = Scene.Query<SceneQuery>(ActiveCamera);

            context.RenderTarget = renderTargetPool.Obtain2D(new RenderTargetFormat(width, height, false, SurfaceFormat.HdrBlendable, DepthFormat.Depth24Stencil8));
            graphicsDevice.SetRenderTarget(context.RenderTarget);
            context.Viewport = graphicsDevice.Viewport;
            graphicsDevice.Clear(Color.CornflowerBlue);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.BlendState = BlendState.Opaque;
            context.RenderPass = "Material";
            _opaqueSceneRenderer.Render(sceneQuery.RenderableNodes, context);

            graphicsDevice.ResetTextures();

            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            context.RenderPass = "AlphaBlend";
            _transparentSceneRenderer.Render(sceneQuery.RenderableNodes, context, RenderOrder.BackToFront);
            context.RenderPass = null;
            graphicsDevice.ResetTextures();

            DebugRenderer.Render(context);

            context.CameraNode = null;
            context.Scene = null;
            context.RenderPass = null;*/
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            Scene.Update(deltaTime);
            DebugRenderer.Update(deltaTime);
        }
    }
}
