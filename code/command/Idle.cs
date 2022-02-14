/// <summary>This command does absolutely nothing for a limited time.</summary>
public class Idle : Command
{
    /// <summary>Internal counter to calculate the time passed,
    /// seconds.</summary>
    private float counter;

    /// <summary>Internal measurement of time required to pass,
    /// seconds.</summary>
    private float total;

    /// <summary>Create an <c>Idle</c> command to do nothing for given
    /// time.</summary>
    /// <param name="total">The time to be idle for.</param>
    public Idle(float total)
    {
        this.total = total;
    }

    public void Init(Controllable controllable, Context context) { }
    public State Process(float delta)
    {
        // Increase the counter and if it's greater than total mark state as
        // done.
        counter += delta;
        if (counter >= total)
        {
            return State.Done;
        }
        return State.Going;
    }
}
