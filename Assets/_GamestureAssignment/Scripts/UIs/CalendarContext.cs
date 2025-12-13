namespace GamestureAssignment.UIs
{
    public struct CalendarContext
    {
        public int DayIndex { get; }
        public int TotalDays { get; }

        public CalendarContext(int dayIndex, int totalDays)
        {
            DayIndex = dayIndex;
            TotalDays = totalDays;
        }
    }

}