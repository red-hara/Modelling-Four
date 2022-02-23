using Godot;

/// <summary>4-DoF pose representation. Stores consecutive translation by
/// <c>position</c> and <c>rotation</c> around local Z axis.<summary>
public struct Pose4
{
    /// <summary>The position part of this pose, millimeters.</summary>
    public Vector3 position;

    /// <summary>The rotation part of this pose, degrees around local Z
    /// axis.</summary>
    public float rotation;

    /// <summary>Create new <c>Pose4</c> struct.</summary>
    /// <param name="position">The <c>Pose4</c> position.</param>
    /// <param name="rotation">The <c>Pose4</c> rotation, degrees.</param>
    public Pose4(Vector3 position, float rotation)
    {
        this.position = position;
        this.rotation = Mathf.Wrap(rotation, -180, 180);
    }

    /// <summary>Calculate inverse pose <c>P^-1</c> such that <c>P * P^-1 = new
    /// Pose4(new vector3(0, 0, 0), 0)</c>.</summary>
    /// <returns>A <c>Pose4</c> such, that combining it with the original will
    /// yeld zero translation and zero rotation.</returns>
    public Pose4 Inverse()
    {
        return new Pose4(
            -position.Rotated(new Vector3(0, 0, 1), -Mathf.Deg2Rad(rotation)),
            -rotation
        );
    }

    /// <summary>Find interpolated <c>Pose4</c> between <c>this</c> and
    /// <c>other</c> poses with respect to progress <c>t</c>.</summary>
    /// <param name="other">The target pose to interpolate to.</param>
    /// <param name="t">The interpolation value, in range [0, 1].</param>
    /// <returns>The interpolated pose.</returns>
    public Pose4 Interpolate(Pose4 other, float t)
    {
        Vector3 position = this.position.LinearInterpolate(other.position, t);
        float deltaAngle = Mathf.Wrap(
            other.rotation - rotation,
            -180,
            180
        );
        return new Pose4(
            position,
            rotation + deltaAngle * t
        );
    }

    /// <summary>Combine two poses to calculate resulting translation and
    /// rotation. Let the poses be <c>A</c> and <c>B</c>. Then <c>A*B</c>
    /// denotes translation and rotation by the <c>A</c> value and, after that,
    /// translation and rotation by the <c>B</c> value in the local space of
    /// <c>A</c>.</summary>
    public static Pose4 operator *(Pose4 self, Pose4 other)
    {
        return new Pose4(
            self.position + other.position.Rotated(
                new Vector3(0, 0, 1),
                Mathf.Deg2Rad(self.rotation)
            ),
            self.rotation + other.rotation
        );
    }

    public override string ToString()
    {
        return string.Format(
            "[{0}, {1}]",
            position,
            rotation
        );
    }
}
