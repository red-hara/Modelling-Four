/// <summary>Hold position indefinitely. This command is intended to be used
/// with other commands, like the <c>InputWait</c> command.</summary>
public class PositionHold : Command
{
    /// <summary>Internal reference to the controlled mechanism.</summary>
    private Controllable controllable;

    /// <summary>Internal reference to the execution context.</summary>
    private Context context;

    /// <summary>The target position to hold the mechanism in.</summary>
    private Target4 target;

    public void Init(Controllable controllable, Context context)
    {
        this.controllable = controllable;
        this.context = context;
        // Calculate current tool position using the flange position and TCP
        // geometry information.
        target = context.Origin *
            controllable.GetCurrentPosition() *
            context.Tool;
    }
    public State Process(float delta)
    {
        // Calculate current flange position.
        Target4 flangeTarget = context.Origin.Inverse() *
            target *
            context.Tool.Inverse();
        // Apply it to the robot.
        if (controllable.SetPosition(flangeTarget) is null)
        {
            return State.Error;
        }
        return State.Going;
    }
}
