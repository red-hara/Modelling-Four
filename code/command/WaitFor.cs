/// <summary>Wait for specified event while holding spatial position.</summary>
public class WaitFor : Command
{
    /// <summary>Internal <c>PositionHold</c> command.</summary>
    private PositionHold hold = new PositionHold();

    /// <summary>The end condition delegate.</summary>
    private Condition condition;

    /// <summary>Create <c>WaitFor</c> command to wait for specified event to
    /// occur.</summary>
    // <param name="condition">Condition to wait for.</param>
    public WaitFor(Condition condition)
    {
        this.condition = condition;
    }

    public void Init(Controllable controllable, Context context)
    {
        hold.Init(controllable, context);
    }

    public State Process(float delta)
    {
        if (hold.Process(delta) == State.Error)
        {
            return State.Error;
        }
        if (condition())
        {
            return State.Done;
        }
        return State.Going;
    }

    /// <summary>The condition delegate to represent end of waiting
    /// event.</summary>
    /// <returns><c>true</c> if the event happened.</returns>
    public delegate bool Condition();
}
