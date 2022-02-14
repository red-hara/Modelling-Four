/// <summary>The command executed to modify the <c>Context</c> state.</summary>
public class ContextCommand<T> : Command where T : Context
{
    /// <summary>Current context.</summary>
    private Context context;

    /// <summary>The delegate to be called on the context when the
    /// <c>ContextCommand</c> is executed.</summary>
    private UpdateContext updater;

    /// <summary>Create new <c>ContexCommand</c>.</summary>
    /// <param name="updater">The delegate to be called durinc this command
    /// execution.</summary>
    public ContextCommand(UpdateContext updater)
    {
        this.updater = updater;
    }

    public void Init(Controllable controllable, Context context)
    {
        this.context = context;
    }
    public State Process(float delta)
    {
        // Execute the updater passing current context as an argument.
        updater((T)context);
        // We are done.
        return State.Done;
    }

    /// <summary>The delegate to take <c>Context</c> and modify it.</summary>
    // Delegate means that anything that is compatible with it may be passed as
    // the first-order function, stored in this delegate and later called.
    public delegate void UpdateContext(T context);
}
