using System.Threading.Tasks;
using Godot;

public class Palettizer : Context
{
    [Export]
    public NodePath attachablePath;

    private Spatial attachable;
    public override void _Ready()
    {
        attachable = GetNode<Spatial>(attachablePath);
    }

    override public async Task Run()
    {
        await InputWait(KeyList.Space);
        await Pick(new Pose4(new Vector3(0, 0, 0), 90));
        for (int i = 0; i < 3; ++i)
        {
            Pose4 target = new Pose4(new Vector3(250, -1000, 500), i * 120);
            await Place(target);
            await Pick(target);
        }
        await Place(new Pose4(new Vector3(250, -1000, 500), 0));
    }

    public async Task Pick(Pose4 targetPose)
    {
        await Joint(
            new Target4(targetPose * new Pose4(new Vector3(0, 0, 300), 0), 0),
            0.25f
        );
        GetNode<Gripper>(tool).Open();
        await WaitFor(() =>
        {
            return GetNode<Gripper>(tool).CurrentState == Gripper.State.Open;
        });
        await Linear(targetPose, 250, 180);
        GetNode<Gripper>(tool).Close();
        await WaitFor(() =>
        {
            return GetNode<Gripper>(tool).CurrentState == Gripper.State.Closed;
        });
        Attach(attachable);
        await Linear(
            targetPose * new Pose4(new Vector3(0, 0, 300), 0),
            250,
            180
        );
    }


    public async Task Place(Pose4 targetPose)
    {
        await Joint(
            new Target4(targetPose * new Pose4(new Vector3(0, 0, 300), 0), 0),
            0.25f
        );
        await Linear(targetPose, 250, 180);
        GetNode<Gripper>(tool).Open();
        await WaitFor(() =>
        {
            return GetNode<Gripper>(tool).CurrentState == Gripper.State.Open;
        });
        Detach(attachable);
        await Linear(
            targetPose * new Pose4(new Vector3(0, 0, 300), 0),
            250,
            180
        );
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
