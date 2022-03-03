using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

/// <summary>The <c>Controller</c> node interacts with the <c>Context</c> node
/// and controlls the <c>Controllable</c> node.</summary>
public class Controller : Node
{
    /// <summary>Current command to be polled during execution.</summary>
    private Command currentCommand;

    private Queue<(Command, TaskCompletionSource<CompletionStatus>)> next =
        new Queue<(Command, TaskCompletionSource<CompletionStatus>)>();

    private TaskCompletionSource<CompletionStatus> actionCompletionSource;

    private CompletionStatus? status;

    private Context context;

    /// <summary>Path to the <c>Controllable</c> node.</summary>
    [Export]
    public NodePath controllable;
    public Controllable Controllable
    {
        get => GetNode<Controllable>(controllable);
    }

    public override void _Process(float delta)
    {
        Controllable control = GetNode<Controllable>(controllable);
        if (!(status is null))
        {
            actionCompletionSource.SetResult(status.Value);
            status = null;
        }
        // Poll current command if present.
        if (!(currentCommand is null))
        {
            State state = currentCommand.Process(delta);
            switch (state)
            {
                case State.Going:
                    break;
                case State.Done:
                    currentCommand = null;
                    status = CompletionStatus.Success;
                    break;
                case State.Error:
                    currentCommand = null;
                    status = CompletionStatus.Error;
                    GD.Print("Error!");
                    break;
            }
        }
        else if (next.Count > 0)
        {
            (Command, TaskCompletionSource<CompletionStatus>) data =
                next.Dequeue();
            currentCommand = data.Item1;
            currentCommand.Init(Controllable, context);
            actionCompletionSource = data.Item2;

        }
    }

    public void Init(Context context)
    {
        this.context = context;
    }

    public Task<CompletionStatus> EnqueueCommand(Command command)
    {
        TaskCompletionSource<CompletionStatus> source =
            new TaskCompletionSource<CompletionStatus>();
        next.Enqueue((command, source));
        return source.Task;
    }

    public enum CompletionStatus
    {
        Success,
        Error,
    }
}
