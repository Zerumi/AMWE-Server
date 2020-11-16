using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m3md2
{
    public class Parser
    {
        public static TimeDescription GetTimeDescription(DateTime dateTime)
        {
            if (dateTime.Hour >= 4 && dateTime.Hour <= 11)
            {
                return TimeDescription.Morning;
            }
            else if (dateTime.Hour >= 12 && dateTime.Hour <= 16)
            {
                return TimeDescription.Afternoon;
            }
            else if (dateTime.Hour >= 17 && dateTime.Hour <= 23)
            {
                return TimeDescription.Evening;
            }
            else if (dateTime.Hour >= 0 && dateTime.Hour <= 3)
            {
                return TimeDescription.Night;
            }
            return TimeDescription.Undefinded;
        }

        public static string GetWelcomeLabel(TimeDescription timedesc)
        {
            switch (timedesc)
            {
                case TimeDescription.Undefinded:
                    return "Добро пожаловать";
                case TimeDescription.Morning:
                    return "Доброе утро";
                case TimeDescription.Afternoon:
                    return "Добрый день";
                case TimeDescription.Evening:
                    return "Добрый вечер";
                case TimeDescription.Night:
                    return "Доброй ночи";
                default:
                    return "Приветствую";
            }
        }
    }

    public enum TimeDescription
    {
        Undefinded,
        Morning,
        Afternoon,
        Evening,
        Night
    }
}
