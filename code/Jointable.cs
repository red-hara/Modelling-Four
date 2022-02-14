/// <summary>The <c>Jointable</c> instance provides several methods for working
/// with its joints.</summary>
public interface Jointable
{
    /// <summary>Try setting generalized coordinates for this
    /// <c>Jointable</c>.</summary>
    /// <param name="generalized">Generalized coordinates to try to set.</param>
    /// <returns>Position of this <c>Jointable</c>'s flange if operation
    /// successful.</returns>
    Target4? SetJoints(Generalized4 generalized);

    /// <summary>Try solving the forward kinematics problem for this
    /// <c>Jointable</c>.</summary>
    /// <param name="generalized">Generalized coordinates to solve the forward
    /// kinematics problem for.</param>
    /// <returns>Position of this <c>Jointable</c>'s flange for provided
    /// generalized coordinates if the solution exists.</returns>
    Target4? Forward(Generalized4 generalized);

    /// <summary>Get current generalized coordinates.</summary>
    /// <returns>Current generalized coordinates.</returns>
    Generalized4 GetCurrentJoints();

    /// <summary>Get maximum joint velocity set.</summary>
    /// <returns>Maximum joint velocity.</returns>
    Generalized4 MaximumJointVelocity();
}
