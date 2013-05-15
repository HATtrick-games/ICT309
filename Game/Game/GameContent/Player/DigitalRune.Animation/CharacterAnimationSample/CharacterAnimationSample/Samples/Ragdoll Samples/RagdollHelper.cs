using System;
using System.Diagnostics;
using DigitalRune.Geometry;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics.Constraints;
using DigitalRune.Physics.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CharacterAnimationSample
{
  /// <summary>
  /// Provides useful methods for <see cref="Ragdoll"/>s.
  /// </summary>
  public static class RagdollHelper
  {
    // A buffer that holds the vertices of the lines.
    private static readonly VertexPositionColor[] _points = new VertexPositionColor[2048];
    
    // The number of used points in _points.
    private static int _pointCount;

    private static GraphicsDevice _graphicsDevice;
    private static float _scale;


    /// <summary>
    /// Visualizes the constraints of the ragdoll (for debugging).
    /// </summary>
    /// <param name="ragdoll">The ragdoll.</param>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <param name="effect">
    /// A BasicEffect. The world, view and projection matrices must be initialized.
    /// </param>
    /// <param name="scale">
    /// A scale factor that determines the size of the drawn elements.
    /// </param>
    /// <remarks>
    /// Currently, only <see cref="TwistSwingLimit"/>s and <see cref="AngularLimit"/>s are
    /// supported. 
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="ragdoll"/>, <paramref name="graphicsDevice"/> or <paramref name="effect"/>
    /// is <see langword="null"/>.
    /// </exception>
    public static void DrawConstraints(Ragdoll ragdoll, GraphicsDevice graphicsDevice, 
                                       BasicEffect effect, float scale)
    {
      if (ragdoll == null)
        throw new ArgumentNullException("ragdoll");
      if (graphicsDevice == null)
        throw new ArgumentNullException("graphicsDevice");
      if (effect == null)
        throw new ArgumentNullException("effect");

      _graphicsDevice = graphicsDevice;
      _scale = scale;
      _pointCount = 0;

      Debug.Assert(_points.Length % 2 == 0, "_points array must have an even number of elements.");
      _pointCount = 0;

      // Disable lighting, enable vertex colors.
      bool originalLightingEnabled = effect.LightingEnabled;
      bool originalVertexColorEnabled = effect.VertexColorEnabled;
      effect.LightingEnabled = false;
      effect.VertexColorEnabled = true;

      effect.CurrentTechnique.Passes[0].Apply();

      // Render information for each limit.
      foreach (Constraint limit in ragdoll.Limits)
      {
        // Get the ball joint constraint that connects the two bodies of the limit.
        BallJoint joint = null;
        foreach (Constraint constraint in ragdoll.Joints)
        {
          if (constraint.BodyA == limit.BodyA && constraint.BodyB == limit.BodyB
              || constraint.BodyA == limit.BodyB && constraint.BodyB == limit.BodyA)
          {
            joint = constraint as BallJoint;
            break;
          }
        }

        // Skip this limit if no joint was found.
        if (joint == null)
          continue;
        
        TwistSwingLimit twistSwingLimit = limit as TwistSwingLimit;
        if (twistSwingLimit != null)
        {
          DrawTwistSwingLimit(joint, twistSwingLimit);
          continue;
        }

        AngularLimit angularLimit = limit as AngularLimit;
        if (angularLimit != null)
        {
          DrawAngularLimit(joint, angularLimit);
          continue;
        }
      }

      // Draw everything that is in the buffer.
      Flush();

      // Restore original effect settings.
      effect.LightingEnabled = originalLightingEnabled;
      effect.VertexColorEnabled = originalVertexColorEnabled;
    }


    private static void DrawTwistSwingLimit(BallJoint joint, TwistSwingLimit limit)
    {
      // ----- Draw swing cone.
      // The tip of the swing cone:
      Vector3F coneTip = joint.BodyA.Pose.ToWorldPosition(joint.AnchorPositionALocal);
      
      // The first point on the swing cone:
      Vector3 previousConePoint = (Vector3)limit.GetPointOnCone(0, coneTip, _scale);

      // Draw swing cone.
      const int numberOfSegments = 24;
      const float segmentAngle = ConstantsF.TwoPi / numberOfSegments; 
      Color color = Color.Violet;
      for (int i = 0; i < numberOfSegments; i++)
      {
        Vector3 conePoint = (Vector3)limit.GetPointOnCone((i + 1) * segmentAngle, coneTip, _scale);        

        // Line from cone tip to cone base.
        _points[_pointCount] = new VertexPositionColor((Vector3)coneTip, color);
        IncrementPointCount();
        _points[_pointCount] = new VertexPositionColor(conePoint, color);
        IncrementPointCount();

        // Line on the cone base.
        _points[_pointCount] = new VertexPositionColor(previousConePoint, color);
        IncrementPointCount();
        _points[_pointCount] = new VertexPositionColor(conePoint, color);
        IncrementPointCount();

        previousConePoint = conePoint;
      }

      // ----- Draw twist axis.      
      // The x-axis is the twist direction. 
      Vector3F twistAxis = Vector3F.UnitX;
      // The twist axis relative to body B.
      Vector3F twistAxisDirectionBLocal = limit.AnchorOrientationBLocal * twistAxis;
      // The twist axis relative to world space.
      Vector3F twistAxisDirection = limit.BodyB.Pose.ToWorldDirection(twistAxisDirectionBLocal);
      // (A similar computation is used in DrawArc() below.)

      // Line in twist direction.
      _points[_pointCount] = new VertexPositionColor((Vector3)coneTip, Color.Red);
      IncrementPointCount();
      _points[_pointCount] = new VertexPositionColor((Vector3)(coneTip + twistAxisDirection * _scale), Color.Red);
      IncrementPointCount();

      // A transformation that converts from constraint anchor space to world space.
      Pose constraintToWorld = limit.BodyA.Pose * new Pose(limit.AnchorOrientationALocal);

      // Draw an arc that visualizes the twist limits.
      DrawArc(constraintToWorld, coneTip, Vector3F.UnitX, Vector3F.UnitY, limit.Minimum.X, limit.Maximum.X, Color.Red);
    }


    private static void DrawAngularLimit(BallJoint joint, AngularLimit limit)
    {
      Vector3F jointPosition = joint.BodyA.Pose.ToWorldPosition(joint.AnchorPositionALocal);

      // A transformation that converts from constraint anchor space to world space.
      Pose constraintToWorld = limit.BodyA.Pose * new Pose(limit.AnchorOrientationALocal);

      // Draw an arc for each rotation axis. 
      DrawArc(constraintToWorld, jointPosition, Vector3F.UnitX, Vector3F.UnitY, limit.Minimum.X, limit.Maximum.X, Color.Red);
      DrawArc(constraintToWorld, jointPosition, Vector3F.UnitY, Vector3F.UnitX, limit.Minimum.Y, limit.Maximum.Y, Color.Green);
      DrawArc(constraintToWorld, jointPosition, Vector3F.UnitZ, Vector3F.UnitX, limit.Minimum.Z, limit.Maximum.Z, Color.Blue);
    }


    /// <summary>
    /// Draws an arc to visualize a rotation limit about an axis.
    /// </summary>
    /// <param name="constraintToWorld">
    /// A transformation that transforms from constraint anchor space to world space.
    /// </param>
    /// <param name="center">The center of the circle.</param>
    /// <param name="axis">The rotation axis.</param>
    /// <param name="direction">A direction vector (e.g. the direction of a bone).</param>
    /// <param name="minimum">The minimum angle.</param>
    /// <param name="maximum">The maximum angle.</param>
    /// <param name="color">The color.</param>
    private static void DrawArc(Pose constraintToWorld, Vector3F center, Vector3F axis, Vector3F direction, float minimum, float maximum, Color color)
    {
      if (minimum == 0 && maximum == 0)
        return;

      // Line from circle center to start of arc.
      _points[_pointCount] = new VertexPositionColor((Vector3)center, color);
      IncrementPointCount();
      Vector3F previousArcPoint = center + _scale * constraintToWorld.ToWorldDirection(QuaternionF.CreateRotation(axis, minimum).Rotate(direction));
      _points[_pointCount] = new VertexPositionColor((Vector3)previousArcPoint, color);
      IncrementPointCount();

      // Draw arc.
      int numberOfSegments = (int)Math.Max((maximum - minimum) / (ConstantsF.Pi / 24), 1);
      float segmentAngle = (maximum - minimum) / numberOfSegments;
      for (int i = 0; i < numberOfSegments; i++)
      {
        Vector3F arcPoint = center + _scale * constraintToWorld.ToWorldDirection(QuaternionF.CreateRotation(axis, minimum + (i + 1) * segmentAngle).Rotate(direction));

        _points[_pointCount] = new VertexPositionColor((Vector3)previousArcPoint, color);
        IncrementPointCount();
        _points[_pointCount] = new VertexPositionColor((Vector3)arcPoint, color);
        IncrementPointCount();

        previousArcPoint = arcPoint;
      }

      // Line from end of arc to circle center.
      _points[_pointCount] = new VertexPositionColor((Vector3)previousArcPoint, color);
      IncrementPointCount();
      _points[_pointCount] = new VertexPositionColor((Vector3)center, color);
      IncrementPointCount();      
    }


    /// <summary>
    /// Increments the point count. If the buffer limit is reached, the buffer content is flushed to
    /// make space for new points.
    /// </summary>
    private static void IncrementPointCount()
    {
      _pointCount++;

      if (_pointCount + 1 > _points.Length)
        Flush();      
    }


    /// <summary>
    /// Draws current lines and resets _pointCount.
    /// </summary>
    private static void Flush()
    {
      if (_pointCount > 0)
      {
        _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _points, 0, _pointCount / 2);
        _pointCount = 0;
      }
    }
  }
}
