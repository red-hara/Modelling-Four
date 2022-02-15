using Godot;

public class Palettizer : Context
{
    [Export]
    public NodePath attachable;

    public override void _Ready()
    {
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
        WaitFor(
            () =>
            {
                return GetNode<Gripper>(tool).CurrentState ==
                    Gripper.State.Open;
            }
        );
        Linear(new Pose4(new Vector3(0, 0, 0), 90), 500, 90);
        ContextCommand<Palettizer>((palettizer) =>
        {
            GetNode<Gripper>(tool).Close();
        });
        WaitFor(
            () =>
            {
                return GetNode<Gripper>(tool).CurrentState ==
                    Gripper.State.Closed;
            }
        );
        ContextCommand<Palettizer>((palettizer) =>
        {
            Spatial target = GetNode<Spatial>(attachable);
            Transform transform = target.GlobalTransform;
            target.GetParent().RemoveChild(target);
            GetNode<Spatial>(tool).AddChild(target);
            target.GlobalTransform = transform;
        });
        Linear(new Pose4(new Vector3(0, 0, 500), 0), 500, 90);
    }
}
