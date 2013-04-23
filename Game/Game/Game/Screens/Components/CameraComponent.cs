using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Graphics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Geometry;

namespace ICT309Game.Screens.Components
{
    class CameraComponent
    {
        private Vector3F _position;
        public Vector3F _Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private QuaternionF _orientation;
        public QuaternionF _Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        private CameraNode _cameraNode;
        public CameraNode _CameraNode
        {
            get { return _cameraNode; }
            set { _cameraNode = value; }
        }

        private float _yaw;
        public float _Yaw
        {
            get { return _yaw; }
            set { _yaw = value; }
        }

        private float _pitch = MathHelper.ToRadians(-45.0f);

        public CameraComponent(GameScreen sc)
        {
            PerspectiveProjection projection = new PerspectiveProjection();
            projection.SetFieldOfView(MathHelper.PiOver4,
                sc._graphicsService.GraphicsDevice.Viewport.AspectRatio,
                0.1f, 1000.0f);

            _cameraNode = new CameraNode(new Camera(projection));

            _position = new Vector3F(50f, 120.0f, 50f);
            _yaw = MathHelper.ToRadians(45.0f);
        }

        public void Update(TimeSpan deltaTime)
        {
            // Update the position based upon keyboard
            _orientation = QuaternionF.CreateRotationY(_yaw) * QuaternionF.CreateRotationX(_pitch);

            _cameraNode.PoseWorld = new Pose(_position, _orientation);
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
