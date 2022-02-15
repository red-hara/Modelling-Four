using Godot;

public class Palettizer : Context
{
    [Export]
    public NodePath attachablePath;

    private Spatial attachable;

    private float velocityFast = 1000;
    private float velocitySlow = 250;

    public override void _Ready()
    {
        attachable = GetNode<Spatial>(attachablePath);
        PreparePath();
    }

    public void PreparePath()
    {
        InputWait(KeyList.Space);
        Joint(new Target4(new Pose4(new Vector3(0, 0, 750), 90), 0), 0.25f);
        ContextCommand<Palettizer>((palettizer) =>
        {
            GetNode<Gripper>(tool).Open();
        });
        Joint(new Target4(new Pose4(new Vector3(0, 0, 500), 90), 0), 0.25f);
        WaitFor(() =>
        {
            return GetNode<Gripper>(tool).CurrentState ==
                Gripper.State.Open;
        });
        Linear(new Pose4(new Vector3(0, 0, 0), 90), velocityFast, 90);
        ContextCommand<Palettizer>((palettizer) =>
        {
            GetNode<Gripper>(tool).Close();
        });
        WaitFor(() =>
        {
            return GetNode<Gripper>(tool).CurrentState ==
                Gripper.State.Closed;
        });
        ContextCommand<Palettizer>((palettizer) =>
        {
            Attach(attachable);
        });
        Linear(new Pose4(new Vector3(0, 0, 500), 90), velocitySlow, 90);

        Linear(new Pose4(new Vector3(500, 0, 500), 0), velocitySlow, 90);
        Linear(new Pose4(new Vector3(500, 0, 0), 0), velocitySlow, 90);
        ContextCommand<Palettizer>((palettizer) =>
        {
            Detach(attachable);
            GetNode<Gripper>(tool).Open();
        });
        WaitFor(() =>
        {
            return GetNode<Gripper>(tool).CurrentState ==
                Gripper.State.Open;
        });
        Linear(new Pose4(new Vector3(500, 0, 500), 0), velocityFast, 90);
    }

    public void Attach(Spatial target)
    {
        Transform transform = target.GlobalTransform;
        target.GetParent().RemoveChild(target);
        GetNode<Spatial>(tool).AddChild(target);
        target.GlobalTransform = transform;
    }

    public void Detach(Spatial target)
    {
        Transform transform = target.GlobalTransform;
        target.GetParent().RemoveChild(target);
        GetParent().AddChild(target);
        target.GlobalTransform = transform;
    }
}
