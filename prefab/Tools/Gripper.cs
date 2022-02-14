using System;
using Godot;

[Tool]
public class Gripper : Spatial, Tool
{
    [Export]
    public NodePath moverPath;
    private Spatial mover;

    [Export]
    public float openValue = 250;

    [Export]
    public float closeValue = 150;

    private TargetState? targetState = null;
    private State state = State.Closed;
    public State CurrentState
    {
        get => state;
    }

    private float counter = 0;

    public override void _Ready()
    {
        mover = GetNode<Spatial>(moverPath);
        mover.Translation = new Vector3(closeValue, 0, 0);
    }


    public override void _Process(float delta)
    {
        if (!(targetState is null))
        {
            counter += delta;
            if (counter >= 1)
            {
                switch (targetState.Value)
                {
                    case TargetState.Open:
                        mover.Translation = new Vector3(openValue, 0, 0);
                        state = State.Open;
                        break;
                    case TargetState.Closed:
                        mover.Translation = new Vector3(closeValue, 0, 0);
                        state = State.Closed;
                        break;
                }
                targetState = null;
            }
            else
            {
                float current = 0;
                switch (targetState.Value)
                {
                    case TargetState.Open:
                        current = closeValue + (openValue - closeValue) *
                           counter;
                        break;
                    case TargetState.Closed:
                        current = openValue + (closeValue - openValue) *
                            counter;
                        break;
                }
                mover.Translation = new Vector3(current, 0, 0);
            }
        }
    }

    public void Open()
    {
        if (targetState is null && state == State.Closed)
        {
            targetState = TargetState.Open;
            state = State.Transition;
            counter = 0;
        }
    }

    public void Close()
    {
        if (targetState is null && state == State.Open)
        {
            targetState = TargetState.Closed;
            state = State.Transition;
            counter = 0;
        }
    }

    public Pose4 GetToolCenterPoint()
    {
        return new Pose4(new Vector3(-250, 250, -650), 0);
    }

    public enum State
    {
        Open,
        Closed,
        Transition,
    }

    private enum TargetState
    {
        Open,
        Closed,
    }
}
