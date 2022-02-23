using System.Threading.Tasks;
using Godot;

/// <summary>The <c>Controller</c> node interacts with the <c>Context</c> node
/// and controlls the <c>Controllable</c> node.</summary>
public class Controller : Node
{
    /// <summary>Current command to be polled during execution.</summary>
    private Command currentCommand;

    private TaskCompletionSource<CompletionStatus> actionCompletionSource;

    private CompletionStatus? status;

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
    }

    public Task<CompletionStatus> SpawnCommand(Command command)
    {
        currentCommand = command;
        actionCompletionSource = new TaskCompletionSource<CompletionStatus>();
        return actionCompletionSource.Task;
    }

    public enum CompletionStatus
    {
        Success,
        Error,
    }
}
