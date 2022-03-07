using System.Threading.Tasks;
using Godot;

/// <summary>The robot execution context.</summary>
public class Context : Node
{
    /// <summary>Path to the robot <c>Controller</c>.</summary>
    [Export]
    public NodePath controller;

    /// <summary>Robot <c>Controller</c> instance.</summary>
    private Controller control;

    /// <summary>Robot's translation in relation to the world origin.</summary>
    [Export]
    public Vector3 originPosition;

    /// <summary>Robot's rotation in relation to the world origin.</summary>
    [Export]
    public float originAngle;

    /// <summary>Robot's Pose in relation to the world origin.</summary>
    public Pose4 Origin
    {
        get => new Pose4(originPosition, originAngle);
    }

    /// <summary>Path to the <c>Part</c> attached to the robot base.</summary>
    [Export]
    public NodePath part;
    /// <summary><c>Part</c> to work with.</summary>
    public Pose4 Part
    {
        // It is possible to get part, but not to set it.
        get => GetPartOrigin();
    }

    /// <summary>Path to the <c>Tool</c> attached to the robot.</summary>
    [Export]
    public NodePath tool;

    /// <summary><c>Tool</c> to work with.</summary>
    public Pose4 Tool
    {
        // It is possible to get tool, but not to set it.
        get => GetToolCenterPoint();
    }

    /// <summary>Internal <c>Run</c> <c>Task</c> storage.</summary>
    private Task task;

    public override void _EnterTree()
    {
        control = GetNode<Controller>(controller);
        control.Init(this);
        task = Run();
    }

    public async virtual Task Run()
    {
        await Task.Delay(0);
    }


    /// <summary>Get current TCP geometry.</summary>
    /// <returns>TCP in relation to the flange.</returns>
    public Pose4 GetToolCenterPoint()
    {
        // Return zero Pose4 if either
        // - we are working with flange
        // - no tool path is set
        // - the tool path is empty
        if (tool is null)
        {
            return new Pose4();
        }
        if (tool.IsEmpty())
        {
            return new Pose4();
        }
        Tool tcp = GetNode<Tool>(tool);
        return tcp.GetToolCenterPoint();
    }

    /// <summary>Get current part origin geometry.</summary>
    /// <returns>Part origin in relation to the robot base.</summary>
    public Pose4 GetPartOrigin()
    {
        // Return zero Pose4 if either
        // - no part path is set
        // - the part path is empty
        if (part is null)
        {
            return new Pose4();
        }
        if (part.IsEmpty())
        {
            return new Pose4();
        }
        Part pt = GetNode<Part>(part);
        return pt.GetOrigin();
    }

    public async Task<Controller.CompletionStatus> SpawnCommand(Command command)
    {
        return await control.EnqueueCommand(command);
    }
    public async Task<Controller.CompletionStatus> Linear(
        Pose4 target,
        float linearVelocity,
        float angularVelocity
    )
    {
        return await SpawnCommand(
            new Linear(target, linearVelocity, angularVelocity)
        );
    }

    public async Task<Controller.CompletionStatus> Joint(
        Target4 target,
        float speed
    )
    {
        return await SpawnCommand(new Joint(target, speed));
    }

    public async Task<Controller.CompletionStatus> InputWait(KeyList key)
    {
        return await SpawnCommand(new InputWait(key));
    }

    public async Task<Controller.CompletionStatus> ContextCommand<T>(
        ContextCommand<T>.UpdateContext command
    )
    where
        T : Context
    {
        return await SpawnCommand(new ContextCommand<T>(command));
    }

    public async Task<Controller.CompletionStatus> WaitFor(
        WaitFor.Condition condition
    )
    {
        return await SpawnCommand(new WaitFor(condition));
    }
}
