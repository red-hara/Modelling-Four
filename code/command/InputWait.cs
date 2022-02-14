using Godot;

/// <summary>Wait for specific keyboard key being pressed.</summary>
public class InputWait : Command
{
    /// <summary>Internal <c>PositionHold</c> command.</summary>
    private PositionHold hold = new PositionHold();

    /// <summary>The specific keyboard key to be pressed.</summary>
    private KeyList key;

    /// <summary>Create <c>InputWait</c> to wait for passed key to be
    /// pressed.</summary>
    /// <param name="key">The key to be pressed.</param>
    public InputWait(KeyList key)
    {
        this.key = key;
    }
    public void Init(Controllable controllable, Context context)
    {
        hold.Init(controllable, context);
    }
    public State Process(float delta)
    {
        // Check if internal PositionHold has failed.
        if (hold.Process(delta) == State.Error)
        {
            return State.Error;
        }
        // Check for keyboard input.
        if (Input.IsPhysicalKeyPressed(((int)key)))
        {
            return State.Done;
        }
        return State.Going;
    }
}
