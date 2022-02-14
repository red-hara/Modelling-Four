using Godot;

/// <summary>The <c>Controller</c> node interacts with the <c>Context</c> node
/// and controlls the <c>Controllable</c> node.</summary>
public class Controller : Node
{

    /// <summary>Path to the <c>Context</c> node to work with.</summary>
    [Export]
    public NodePath context;

    /// <summary>The <c>Context</c> to work with.</summary>
    private Context ctx;

    /// <summary>Current command to be polled during execution.</summary>
    private Command currentCommand;

    /// <summary>Path to the <c>Controllable</c> node.</summary>
    [Export]
    public NodePath controllable;

    /// <summary>Path to the <c>Trace</c> to be drawn on the current
    /// TCP.</summary>
    [Export]
    public NodePath tcpTrace;

    public override void _Ready()
    {
        // The context's path has to be initialized before running the program.
        ctx = GetNode<Context>(context);
    }

    public override void _Process(float delta)
    {
        Controllable control = GetNode<Controllable>(controllable);
        // Poll current command if present, else try dequeueing another one from
        // the Context's queue.
        if (!(currentCommand is null))
        {
            State state = currentCommand.Process(delta);
            switch (state)
            {
                case State.Going:
                    break;
                case State.Done:
                    currentCommand = null;
                    break;
                case State.Error:
                    currentCommand = null;
                    ctx.commands.Clear();
                    GD.Print("Error!");
                    break;
            }
        }
        else if (ctx.commands.Count > 0)
        {
            currentCommand = ctx.commands.Dequeue();
            currentCommand.Init(control, ctx);
        }

        Trace trace = GetNode<Trace>(tcpTrace);
        Spatial controlSpatial = (Spatial)control;
        // Generate transform chain "Controllable -> Flange -> TCP"
        trace.GlobalTransform = controlSpatial.GlobalTransform *
            new Transform(
                Quat.Identity,
                (
                    control.GetCurrentPosition().pose *
                    ctx.Tool
                ).position
            );
    }
}
