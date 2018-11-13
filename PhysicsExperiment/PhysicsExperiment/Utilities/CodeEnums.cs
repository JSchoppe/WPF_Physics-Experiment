namespace PhysicsExperiment
{
    /// <summary>Enumeration for cardinal directions</summary>
    public enum Direction
    {
        Up, Down, Left, Right
    }

    /// <summary>Enumeration for a binary state</summary>
    public enum State
    {
        Low, Falling, High, Rising
    }

    /// <summary>Enumeration that represents an action linked to a key</summary>
    public enum KeyboardBinding
    {
        PrimaryXPositive, PrimaryXNegative,
        PrimaryYPositive, PrimaryYNegative,

        SecondaryXPositive, SecondaryXNegative,
        SecondaryYPositive, SecondaryYNegative,
    }

}