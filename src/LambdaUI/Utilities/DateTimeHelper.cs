using System;
using System.Collections.Generic;
using System.Text;

namespace LambdaUI.Utilities
{
    public static class DateTimeHelper
    {
        public static string ShortDateTimeNowString => DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
    }
}
