// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;

namespace m3md2
{
    public class Parser
    {
        public static TimeDescription GetTimeDescription(DateTime dateTime)
        {
            if (dateTime.Hour <= 3)
            {
                return TimeDescription.Night;
            }
            else if (dateTime.Hour <= 11)
            {
                return TimeDescription.Morning;
            }
            else if (dateTime.Hour <= 16)
            {
                return TimeDescription.Afternoon;
            }
            else if (dateTime.Hour <= 23)
            {
                return TimeDescription.Evening;
            }
            throw new ArgumentException($"(17.1) Получено неожиданное значение для datetime.Hour: {dateTime.Hour}");
        }

        public static string GetWelcomeLabel(TimeDescription timedesc)
        {
            switch (timedesc)
            {
                case TimeDescription.Morning:
                    return "Доброе утро";
                case TimeDescription.Afternoon:
                    return "Добрый день";
                case TimeDescription.Evening:
                    return "Добрый вечер";
                case TimeDescription.Night:
                    return "Доброй ночи";
                default:
                    throw new ArgumentException($"(17) Значение определено неверно: timedesc {timedesc}");
            }
        }
    }

    public enum TimeDescription
    {
        Night,
        Morning,
        Afternoon,
        Evening
    }
}
