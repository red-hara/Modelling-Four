/// <summary>Specific robot target that takes into account number of flange
/// revolutions.</summary>
public struct Target4
{
    /// <summary>The robot pose.</summary>
    public Pose4 pose;

    /// <summary>Amount of flange rotations.</summary>
    public int flangeRevolutions;

    /// <summary>Create new <c>Target4</c> instance.</summary>
    /// <param name="pose">The target pose.</summary>
    /// <param name="flangeRevolutions">The number of full flange
    /// revolutions.</param>
    public Target4(Pose4 pose, int flangeRevolutions)
    {
        this.pose = pose;
        this.flangeRevolutions = flangeRevolutions;
    }

    /// <summary>Calculate offsetting the target by given pose on the right. The
    /// amount of flange revolutions stays the same.</summary>
    public static Target4 operator *(Target4 target, Pose4 pose)
    {
        return new Target4(target.pose * pose, target.flangeRevolutions);
    }


    /// <summary>Calculate offsetting the target by given pose on the left. The
    /// amount of flange revolutions stays the same.</summary>
    public static Target4 operator *(Pose4 pose, Target4 target)
    {
        return new Target4(pose * target.pose, target.flangeRevolutions);
    }
}
