namespace Machine.Utilities
{
    /// <summary>
    /// Used for signaling the OS state:
    /// Free -> when the simulation is not yet started or it has finished.
    /// Idle -> when not required to perform any operation regarding the simulation's commands, during the simulation.
    /// Busy -> when performing certain operations regarding the commands of the simulation.
    /// </summary> 
    public enum OsState
    {
        Free,
        Idle,
        Busy
    }
}
