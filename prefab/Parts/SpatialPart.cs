using Godot;

/// <summary>The <c>SpatialPart</c> is template for custom
/// <c>Part</c>.</summary>
class SpatialPart : Spatial, Part
{
    public override void _Ready()
    {
    }

    public override void _Process(float delta)
    {
    }

    public Pose4 GetOrigin()
    {
        // Just return position in local space. Ignore BC angles and hope that
        // they are zero.
        return new Pose4(Translation, RotationDegrees.z);
    }
}
