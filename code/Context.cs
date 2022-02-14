using System.Collections.Generic;
using Godot;

/// <summary>The robot execution context.</summary>
public class Context : Node
{
    /// <summary>List on enqueued commands.</summary>
    public Queue<Command> commands = new Queue<Command>();

    [Export]
    public Vector3 originPosition;

    [Export]
    public float originAngle;

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

    /// <summary>Add command to the command list.</summary>
    /// <param name="command">The <c>Command</c> to be enqueued.</param>
    public void AddCommand(Command command)
    {
        commands.Enqueue(command);
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

    /// <summary>Enqueue <c>Linear</c> command.</summary>
    /// <param name="target">Target <c>Pose4</c> for this motion.</param>
    /// <param name="linearVelocity">Maximum motion linear velocity, millimeter
    /// per second.</param>
    /// <param name="angularVelocity">Maximum motion angular velocity, degree
    /// per second.</param>
    public void Linear(
        Pose4 target,
        float linearVelocity,
        float angularVelocity
    )
    {
        AddCommand(new Linear(target, linearVelocity, angularVelocity));
    }

    /// <summary>Enqueue <c>Joint</c> command.</summary>
    /// <param name="target">Target <c>Target4</c> for this motion.</param>
    /// <param name="velocity">Motion velocity, fraction, in (1, 0]
    /// range.<param>
    public void Joint(
        Target4 target,
        float velocity
    )
    {
        AddCommand(new Joint(target, velocity));
    }

    /// <summary>Enqueue <c>ContextCommand</c> command.</summary>
    /// <param name="command">The <c>UpdateContext</c> delegate to be called on
    /// this <c>Context</c>.</param>
    public void ContextCommand<T>(ContextCommand<T>.UpdateContext command)
    where T : Context
    {
        AddCommand(new ContextCommand<T>(command));
    }

    /// <summary>Enqueue <c>InputWait</c> command.</summary>
    /// <param name="key">The key (from the <c>KeyList</c>) to wait for.</param>
    public void InputWait(KeyList key)
    {
        AddCommand(new InputWait(key));
    }

    /// <summary>Enqueue <c>WaitFor</c> command.</summary>
    /// <param name="condition">The condition to be met to continue.</param>
    public void WaitFor(WaitFor.Condition condition)
    {
        AddCommand(new WaitFor(condition));
    }
}
