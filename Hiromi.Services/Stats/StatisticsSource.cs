namespace Hiromi.Services.Stats
{
    /// <summary>
    /// The target statistics source.
    /// If <see cref="Guild"/>, then data will be collected from the entire Guild. If <see cref="User"/>, then data
    /// will only be collected from a user.
    /// </summary>
    public enum StatisticsSource
    {
        User,
        Guild
    }
}