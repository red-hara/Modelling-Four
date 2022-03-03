using System;
using Godot;

/// <summary>Perform linear motion to the target poisition.</summary>
public class Linear : Command
{
    /// <summary>Internal reference to the controlled mechanism.</summary>
    private Controllable controllable;

    /// <summary>Internal reference to the execution context.</summary>
    private Context context;

    /// <summary>The start position of the robot.</summary>
    private Target4 start;

    /// <summary>The target pose of the robot.</summary>
    private Pose4 target;

    /// <summary>Maximum linear velocity during the motion execution, millimeter
    /// per second.</summary>
    private float linearVelocity;

    /// <summary>Maximum angular velocity during the motion execution, degree
    /// per second.</summary>
    private float angularVelocity;

    /// <summary>Internal progress counter.</summary>
    private float progress;

    /// <summary>Create new <c>Linear</c> command.</summary>
    /// <param name="target">Target robot pose.</param>
    /// <param name="linearVelocity">Maximum linear velocity during motion
    /// execution, millimeter per second.</param>
    /// <param name="angularVelocity">Maximum angular velocity during motion
    /// execution, degree per second.</param>
    public Linear(Pose4 target, float linearVelocity, float angularVelocity)
    {
        // Note that the target is Pose4, not Target4. That is due to the robot
        // performing the shortest motion between the start and the target,
        // ignoring target flange rotations amount.
        this.target = target;
        // Throw an exception if the velocity parameter is wrong.
        if (linearVelocity <= 0 || angularVelocity <= 0)
        {
            throw new ArgumentException("Wrong velocity");
        }
        this.linearVelocity = linearVelocity;
        this.angularVelocity = angularVelocity;
    }

    public void Init(Controllable controllable, Context context)
    {
        this.controllable = controllable;
        this.context = context;
        // Calculate the tool's position in relation to the robot origin. Just
        // combine the flange position and the TCP position. 
        start = context.Origin *
            controllable.GetCurrentPosition() *
            context.Tool;
    }

    public State Process(float delta)
    {
        // Calculate delta pose between the start and the end. To do so solve
        // simple matrix-like equation:
        //  S * D = T, find D:
        //  S^-1 * S * D = S^-1 * T
        //  D = S^-1 * T
        // The ^-1 operator analog for Pose4 is Inverse operation.
        Pose4 deltaPose = start.pose.Inverse() * target;

        float length = deltaPose.position.Length();
        if (length <= 0.0)
        {
            return State.Done;
        }

        // Calculate times required to perform the linear motion and the angular
        // motion individually.
        float lengthTime = length / linearVelocity;
        float angleTime = Mathf.Abs(
            Mathf.Wrap(deltaPose.rotation, -180, 180)
        ) / angularVelocity;

        // Update progress taking in mind that both parts of motion should
        // finish simultaneously;
        progress += delta / Mathf.Max(lengthTime, angleTime);
        if (progress >= 1)
        {
            // Calculate Target4 by using original target and deltaPose, apply
            // inverse tool transform to obtain flange position.
            Target4 result = context.Origin.Inverse() *
                start *
                deltaPose *
                context.Tool.Inverse();
            if (controllable.SetPosition(result) is null)
            {
                return State.Error;
            }
            return State.Done;
        }
        // Find intermidiate position.
        Pose4 interpolated = new Pose4().Interpolate(deltaPose, progress);
        // Calculate resulting flange position, apply inverse tool translation
        // to obtain flange position in relation to the robot origin.
        Target4 resultTarget = context.Origin.Inverse() *
            start *
            interpolated *
            context.Tool.Inverse();

        if (controllable.SetPosition(resultTarget) is null)
        {
            return State.Error;
        }
        return State.Going;
    }
}
