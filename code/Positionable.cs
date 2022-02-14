/// <summary>The <c>Positionable</c> instance provides several methods for
/// working with its spatial position.</summary>
public interface Positionable
{
    /// <summary>Try setting spatial position for this
    /// <c>Positionable</c>.</summary>
    /// <param name="position">Desired spatial position.</param>
    /// <returns>Resulting generalized position if the provided position is
    /// reachable.</returns>
    Generalized4? SetPosition(Target4 position);

    /// <summary>Try solving the inverse kinematics problem for given
    /// position.</summary>
    /// <param name="position">Target position.</param>
    /// <returns>Generalized coordinates if the provided position is
    /// reachable.</returns>
    Generalized4? Inverse(Target4 position);

    /// <summary>Get current <c>Positionable</c> position.</summary>
    Target4 GetCurrentPosition();
}
