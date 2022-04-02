using System;

namespace CutLang.Token
{
    public class TimestampToken : IToken
    {
        public int Hours { get; }

        public int Minutes { get; }

        public double Seconds { get; }

        public TimeSpan AsTimeSpan => new TimeSpan(TimeSpan.TicksPerHour * Hours + TimeSpan.TicksPerMinute * Minutes + (long)(TimeSpan.TicksPerSecond * Seconds));

        public TimestampToken(int hours, int minutes, double seconds)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }
    }
}
