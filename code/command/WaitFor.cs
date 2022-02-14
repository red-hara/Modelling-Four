public class WaitFor : Command
{
    private PositionHold hold = new PositionHold();

    private Condition condition;

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

    public delegate bool Condition();
}
