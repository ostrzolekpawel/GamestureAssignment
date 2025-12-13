using System;

namespace GamestureAssignment.UIs
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }

}