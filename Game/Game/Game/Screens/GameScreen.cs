using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DigitalRune.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using DigitalRune.Graphics.SceneGraph;
using ICT309Game.Screens.Components;
using DigitalRune.Graphics.Effects;
using DigitalRune.Game.Input;
using Microsoft.Xna.Framework.Input;
using DigitalRune.Mathematics.Algebra;

namespace ICT309Game.Screens
{
    class GameScreen: IScreen
    {
        private GameManager _game;

        public readonly GraphicsManager _graphicsService;
        private readonly ContentManager _contentService;
        private readonly InputManager _inputService;

        private CameraComponent _camera;

        private ModelNode _model;
        private Model _xnamodel;
        private MeshRenderer _meshRenderer;

        public GameScreen(Game game):base()
        {
            _game = (GameManager)game;

            _graphicsService = (GraphicsManager)game.Services.GetService(typeof(IGraphicsService));
            _contentService = (ContentManager)game.Services.GetService(typeof(ContentManager));
            _inputService = (InputManager)game.Services.GetService(typeof(IInputService));
        }

        public override void Initialize()
        {
            var delegateGraphics = new DelegateGraphicsScreen(_graphicsService)
            {
                RenderCallback = Render,
            };
            _graphicsService.Screens.Add(delegateGraphics);

            // Initialise components
            _camera = new CameraComponent(this);

            _meshRenderer = new MeshRenderer();

            Console.WriteLine("GameScreen");
            base.Initialize();
        }

        public override void LoadContent()
        {                
            _model = _graphicsService.Content.Load<ModelNode>("testmodel").Clone();

            // Let's loop through the mesh nodes of the model:
            foreach (var meshNode in _model.GetSubtree().OfType<MeshNode>())
            {
                // Each MeshNode references a Mesh.
                Mesh mesh = meshNode.Mesh;

                // The mesh consists of several submeshes and several materials - usually 
                // one material per submesh, but several submeshes could reference the same 
                // materials.

                // Let's loop through the materials of this mesh.
                foreach (var material in mesh.Materials)
                {
                    // A material is a collection of EffectBindings - one EffectBinding for each
                    // render pass. For example, a complex game uses several render passes, like
                    // a pre-Z pass, a G-buffer pass, a shadow map pass, a deferred material pass, 
                    // etc.In simple games there is only one pass which is called "Default".
                    var effectBinding = material["Default"];

                    // An EffectBinding references an Effect (the XNA Effect class) and it has
                    // "parameter bindings" and "technique bindings". These bindings select the 
                    // values for the shader parameters when the mesh node is rendered. 

                    // Let's change the binding for the DiffuseColor of the shader to give tank 
                    // a red color.
                    //effectBinding.Set("DiffuseColor", new Vector4(1, 0.7f, 0.7f, 1));

                    // The tank uses the default effect binding which is a BasicEffectBinding - this
                    // effect binding uses the XNA BasicEffect. 
                    // In this sample we do not define any lights, therefore we disable the lighting
                    // in the shader.
                    ((BasicEffectBinding)effectBinding).LightingEnabled = false;
                }
            }

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(TimeSpan gameTime)
        {
            if (_inputService.IsDown(Keys.W))
            {
                _camera._Position -= new Vector3F(_camera.GetForwardVector().X, 0.0f, _camera.GetForwardVector().Z);
            }

            if (_inputService.IsDown(Keys.A))
            {
                _camera._Position -= _camera.GetRightVector();
            }

            if (_inputService.IsDown(Keys.S))
            {
                _camera._Position += new Vector3F(_camera.GetForwardVector().X, 0.0f, _camera.GetForwardVector().Z);
            }

            if (_inputService.IsDown(Keys.D))
            {
                _camera._Position += _camera.GetRightVector();
            }

            if (_inputService.IsDown(Keys.E))
            {
               _camera._Yaw -= MathHelper.ToRadians(gameTime.Milliseconds / 50.0f);
            }

            if (_inputService.IsDown(Keys.Q))
            {
                _camera._Yaw += MathHelper.ToRadians(gameTime.Milliseconds / 50.0f);
            }

            _camera._Position -= new Vector3F(0.0f, (_inputService.MouseWheelDelta / 20.0f) * _camera.GetUpVector().Y, 0.0f);

            _camera.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(TimeSpan gameTime)
        {
            base.Draw(gameTime);
        }

        private void Render(RenderContext context)
        {
            context.CameraNode = _camera._CameraNode;

            var graphicsDevice = context.GraphicsService.GraphicsDevice;
         
            graphicsDevice.Clear(Color.CornflowerBlue);

            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            context.RenderPass = "Default";


            // Model rendering code
            //var world = Matrix.CreateFromYawPitchRoll(_camera._Yaw, _camera._Pitch, 0.0f) * Matrix.CreateTranslation(_camera._Position.ToXna());
            //var world = Matrix.CreateFromYawPitchRoll(0.5f, 0.3f, 0.0f) * Matrix.CreateTranslation(0.0f, -10.0f, 5.0f);
            //_xnamodel.Draw(world, (Matrix)_camera._CameraNode.View, _camera._CameraNode.Camera.Projection);
            foreach (var meshNode in _model.GetSubtree().OfType<MeshNode>())
                _meshRenderer.Render(meshNode, context);

            context.RenderPass = null;
            context.CameraNode = null;
        }
    }
}
