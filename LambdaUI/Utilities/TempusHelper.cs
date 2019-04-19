﻿using System;
using LambdaUI.Constants;

namespace LambdaUI.Utilities
{
    public static class TempusHelper
    {
        public static string GetClass(int id)
        {
            switch (id)
            {
                case 4:
                    return "D";
                case 3:
                    return "S";
                default:
                    return id.ToString();
            }
        }

        public static string TicksToFormattedTime(long ticks)
        {
            var timeSpan = TicksToTimeSpan(ticks);
            return TimeSpanToFormattedTime(timeSpan);
        }

        public static string TimeSpanToFormattedTime(TimeSpan timeSpan)
        {
            var factor = (int)Math.Pow(10, 7 - TempusConstants.RoundingSize);
            var roundedTimeSpan = new TimeSpan((long)Math.Round(1.0 * timeSpan.Ticks / factor) * factor);
            return
                $"{roundedTimeSpan.Days}:{roundedTimeSpan.Hours}:{roundedTimeSpan.Minutes}:{roundedTimeSpan.Seconds}.{Math.Round((double)roundedTimeSpan.Milliseconds)}"
                    .Trim('0', ':', '.');
        }
        public static TimeSpan TicksToTimeSpan(long ticks) => new TimeSpan(ticks * 149998);
    }
}