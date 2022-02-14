using System.Collections.Generic;
using Godot;

/// <summary>The <c>Trace</c> node draws trace of it's positions in global
/// space.</summary>
[Tool]
public class Trace : ImmediateGeometry
{
    /// <summary>Maximum amount of points in this trace.</summary>
    [Export]
    public uint pointCount = 5_000;

    /// <summary>List of previous positions in global space.</summary>
    private LinkedList<Vector3> points = new LinkedList<Vector3>();

    public override void _Process(float delta)
    {
        // Extract global transform and store it.
        Transform global = GlobalTransform;
        points.AddLast(global.origin);
        // Remove old points.
        while (points.Count > pointCount)
        {
            points.RemoveFirst();
        }

        Clear();
        Begin(Mesh.PrimitiveType.LineStrip);
        // Since points are drawn in local space we need to transform global
        // coordinates back to local space. This is required due to the Trace
        // origin motion.
        Transform inv = global.Inverse();
        foreach (Vector3 point in points)
        {
            AddVertex(inv.Xform(point));
        }
        End();
    }
}
