// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Windows;

namespace m3md2
{
    public static class Parser
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
            return timedesc switch
            {
                TimeDescription.Morning => "Доброе утро",
                TimeDescription.Afternoon => "Добрый день",
                TimeDescription.Evening => "Добрый вечер",
                TimeDescription.Night => "Доброй ночи",
                _ => throw new ArgumentException($"(17) Значение определено неверно: timedesc {timedesc}"),
            };
        }

        public static string GetUntilOrEmpty(this string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
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
