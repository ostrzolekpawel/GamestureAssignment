using System;

namespace GamestureAssignment.UIs
{
    public interface IDailyRewardTimePolicy
    {
        TimeSpan GetNextRewardDelay(CalendarContext context);
    }

}