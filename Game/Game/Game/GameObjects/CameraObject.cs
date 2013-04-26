using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Game.Input;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.Graphics;
using DigitalRune.Mathematics;
using DigitalRune.Geometry;
using Microsoft.Xna.Framework.Input;

namespace ICT309Game.GameObjects
{
    class CameraObject : GameObject
    {
        private IInputService _inputService;
        private DebugRenderer _debugRenderer;

        private CameraNode _cameraNode;

        private Vector3F _position;
        private QuaternionF _orientation;
        private float _yaw;
        private float _pitch;

        public CameraObject()
        {
            _inputService = ServiceLocator.Current.GetInstance<IInputService>();
        }

        protected override void OnLoad()
        {
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var screen = ((GameScreen)graphicsService.Screens["Default"]);
            _debugRenderer = screen.DebugRenderer;

            PerspectiveProjection projection = new PerspectiveProjection();
            projection.SetFieldOfView(Microsoft.Xna.Framework.MathHelper.PiOver4,
                graphicsService.GraphicsDevice.Viewport.AspectRatio,
                0.1f, 1000.0f);

            _cameraNode = new CameraNode(new Camera(projection));
            screen.Scene.Children.Add(_cameraNode);

            screen.ActiveCamera = _cameraNode;

            ResetCamera();

            base.OnLoad();
        }

        public void ResetCamera()
        {
            _position = new Vector3F(160.0f, 160.0f, 160.0f);
            _yaw = MathHelper.ToRadians(-45.0f);
            _pitch = MathHelper.ToRadians(45.0f);

            _cameraNode.PoseWorld = new Pose(_position);
            _cameraNode.SetLastPose(true);
        }

        protected override void OnUnload()
        {
            var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
            var screen = ((GameScreen)graphicsService.Screens["Default"]);

            if (screen.ActiveCamera == _cameraNode)
            {
                screen.ActiveCamera = null;
            }

            _cameraNode.Parent.Children.Remove(_cameraNode);
            _cameraNode.Dispose();
            _cameraNode = null;
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (_inputService.IsDown(Keys.W))
            {
                _position -= new Vector3F(GetForwardVector().X, 0.0f, GetForwardVector().Z);
            }

            if (_inputService.IsDown(Keys.A))
            {
                _position -= GetRightVector();
            }

            if (_inputService.IsDown(Keys.S))
            {
                _position += new Vector3F(GetForwardVector().X, 0.0f, GetForwardVector().Z);
            }

            if (_inputService.IsDown(Keys.D))
            {
                _position += GetRightVector();
            }

            if (_inputService.IsDown(Keys.E))
            {
                _pitch -= MathHelper.ToRadians(deltaTime.Milliseconds / 50.0f);
            }

            if (_inputService.IsDown(Keys.Q))
            {
                _pitch += MathHelper.ToRadians(deltaTime.Milliseconds / 50.0f);
            }

            _position -= new Vector3F(0.0f, (_inputService.MouseWheelDelta / 20.0f) * GetUpVector().Y, 0.0f);

            _orientation = QuaternionF.CreateRotationY(_pitch) * QuaternionF.CreateRotationX(_yaw);

            _position.X = MathHelper.Clamp(_position.X, -200.0f, 200.0f);
            _position.Y = MathHelper.Clamp(_position.Y, 40.0f, 220.0f);
            _position.Z = MathHelper.Clamp(_position.Z, -200.0f, 200.0f);

            _cameraNode.PoseWorld = new Pose(_position, _orientation);

            base.OnUpdate(deltaTime);
        }

        public Vector3F GetForwardVector()
        {
            return new Vector3F(2 * (_orientation.X * _orientation.Z + _orientation.W * _orientation.Y),
                            2 * (_orientation.Y * _orientation.X - _orientation.W * _orientation.X),
                            1 - 2 * (_orientation.X * _orientation.X + _orientation.Y * _orientation.Y));
        }

        public Vector3F GetUpVector()
        {
            return new Vector3F(2 * (_orientation.X * _orientation.Y - _orientation.W * _orientation.Z),
                    1 - 2 * (_orientation.X * _orientation.X + _orientation.Z * _orientation.Z),
                    2 * (_orientation.Y * _orientation.Z + _orientation.W * _orientation.X));
        }

        public Vector3F GetRightVector()
        {
            return new Vector3F(1 - 2 * (_orientation.Y * _orientation.Y + _orientation.Z * _orientation.Z),
                    2 * (_orientation.X * _orientation.Y + _orientation.W * _orientation.Z),
                    2 * (_orientation.X * _orientation.Z - _orientation.W * _orientation.Y));
        }
    }
}
