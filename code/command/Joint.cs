using System;

/// <summary>Perform joint motion to the target poisition.</summary>
public class Joint : Command
{
    /// <summary>Internal reference to the controlled mechanism.</summary>
    private Controllable controllable;

    /// <summary>Internal reference to the execution context.</summary>
    private Context context;

    /// <summary>The start position of the robot.</summary>
    private Target4 start;

    /// <summary>The target position of the robot.</summary>
    private Target4 target;

    /// <summary>The motion execution velocity, fraction.</summary>
    private float velocity;

    /// <summary>Internal progress counter.</summary>
    private float progress;

    /// <summary>Create new <c>Joint</c> command.</summary>
    /// <param name="target">The robot target.<param>
    /// <param name="velocity">The motion velocity, must be in range
    /// (0, 1].<param>
    public Joint(Target4 target, float velocity)
    {
        // Note that the target is Target4 and stores the amount of flange
        // revolutions.
        this.target = target;
        // Throw an exception if the velocity parameter is wrong.
        if (velocity > 1 || velocity <= 0)
        {
            throw new ArgumentException("Wrong velocity");
        }
        this.velocity = velocity;
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
        // Try calculating generalized positions for the start and target
        // positions. Note that the tool geometry is inversed to calculate the
        // flange position.
        Generalized4? generalizedStart = controllable.Inverse(
            context.Origin.Inverse() * start * context.Tool.Inverse()
        );
        Generalized4? generalizedEnd = controllable.Inverse(
            context.Origin.Inverse() * target * context.Tool.Inverse()
        );
        // If any of those solutions is absent, return an error.
        if (generalizedStart is null || generalizedEnd is null)
        {
            return State.Error;
        }
        // Calculate the generalized delta between the starting and ending
        // positions.
        Generalized4 generalizedDelta =
            generalizedEnd.Value - generalizedStart.Value;
        // Calculate the time required to move each joint.
        Generalized4 times = generalizedDelta.Abs() /
            controllable.MaximumJointVelocity() * (1 / velocity);
        // Get maximum time.
        float maximum = times.Max();

        if (maximum <= 0.0)
        {
            return State.Done;
        }

        // Increase the progress.
        progress += delta / maximum;

        // If progress is >= 1, then we are done here.
        if (progress >= 1)
        {
            controllable.SetJoints(generalizedEnd.Value);
            return State.Done;
        }
        // Interpolate generalized coordinates.
        Generalized4 current = generalizedStart.Value +
            progress * generalizedDelta;
        controllable.SetJoints(current);
        return State.Going;
    }
}
