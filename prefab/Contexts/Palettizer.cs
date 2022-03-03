using System.Threading.Tasks;
using Godot;

public class Palettizer : Context
{
    public static Target4 transition = new Target4(
        new Pose4(new Vector3(0, 0, 1500), 0),
        0
    );

    [Export]
    public NodePath attachables;
    private Node attachablesCollection;
    private Spatial currentAttachable;
    public override void _Ready()
    {
        attachablesCollection = GetNode<Node>(attachables);
    }

    override public async Task Run()
    {
        await InputWait(KeyList.Space);

        for (int index = 0; index < 4; index++)
        {
            currentAttachable = attachablesCollection.GetChild<Spatial>(0);
            await Pick(new Pose4(
                currentAttachable.Translation,
                90 + currentAttachable.RotationDegrees.z
            ));
            await Joint(
                transition,
                0.25f
            );
            await Place(new Pose4(
                new Vector3(250, -1000, 500 + index * 250),
                0
            ));
            await Joint(
                transition,
                0.25f
            );
        }
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
        Attach(attachablesCollection.GetChild<Spatial>(0));
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
        Detach(currentAttachable);
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
