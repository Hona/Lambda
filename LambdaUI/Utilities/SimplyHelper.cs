using System;
using System.Collections.Generic;
using System.Text;

namespace LambdaUI.Utilities
{
    public static class SimplyHelper
    {
        public static TimeSpan GetTimeSpan(double runTime) => new TimeSpan(0, 0, (int) Math.Truncate(runTime),
            (int) (runTime - (int) Math.Truncate(runTime)));



        internal static string ClassToString(int value)
        {
            switch (value)
            {
                case 0: return "Soldier";
                case 1: return "Demoman";
                case 2: return "Conc";
                case 3: return "Engineer";
                case 4: return "Pyro";
                default: return "Unknown";
            }
        }
        internal static string ClassToShortString(int value)
        {
            return ClassToString(value)[0].ToString();
        }

    }
}
