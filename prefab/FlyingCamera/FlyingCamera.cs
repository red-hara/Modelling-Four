using Godot;

/// <summary>The Camera with keyboard control.</summary>
public class FlyingCamera : Spatial
{
    /// <summary>Rotation around the vertical axis, degrees.</summary>
    [Export]
    public float alpha;

    /// <summary>Rotation around the horizontal axis, degrees.</summary>
    [Export]
    public float beta;

    /// <summary>Rotation velocity, degrees per second.</summary>
    [Export]
    float angularVelocity = 15;

    /// <summary>Linear velocity, millimeter per second.</summary>
    [Export]
    float linearVelocity = 500;

    public override void _Process(float delta)
    {
        RotationDegrees = new Vector3(beta, alpha, 0);

        if (!Input.IsPhysicalKeyPressed(((int)KeyList.Shift)))
        {
            // FPV translations are performed in the local space.
            if (Input.IsActionPressed("camera_left"))
            {
                Translation += delta *
                    (Transform.basis.Xform(new Vector3(-linearVelocity, 0, 0)));
            }
            if (Input.IsActionPressed("camera_right"))
            {
                Translation += delta *
                    (Transform.basis.Xform(new Vector3(linearVelocity, 0, 0)));
            }
            if (Input.IsActionPressed("camera_forward"))
            {
                Translation += delta *
                    (Transform.basis.Xform(new Vector3(0, 0, -linearVelocity)));
            }
            if (Input.IsActionPressed("camera_back"))
            {
                Translation += delta *
                    (Transform.basis.Xform(new Vector3(0, 0, linearVelocity)));
            }
            // Ascention and decention are in global space.
            if (Input.IsActionPressed("camera_up"))
            {
                Translation += delta * new Vector3(0, linearVelocity, 0);
            }
            if (Input.IsActionPressed("camera_down"))
            {
                Translation += delta * new Vector3(0, -linearVelocity, 0);
            }
        }
        else
        {
            if (Input.IsActionPressed("camera_left"))
            {
                alpha += delta * angularVelocity;
            }
            if (Input.IsActionPressed("camera_right"))
            {
                alpha -= delta * angularVelocity;
            }
            if (Input.IsActionPressed("camera_forward"))
            {
                beta += delta * angularVelocity;
            }
            if (Input.IsActionPressed("camera_back"))
            {
                beta -= delta * angularVelocity;
            }
        }
    }
}
