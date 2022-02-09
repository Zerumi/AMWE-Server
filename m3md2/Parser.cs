// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;

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

        /// <summary>
        /// Возвращает слова в падеже, зависимом от заданного числа
        /// </summary>
        /// <param name="number">Число от которого зависит выбранное слово</param>
        /// <param name="nominativ">Именительный падеж слова. Например "день"</param>
        /// <param name="genetiv">Родительный падеж слова. Например "дня"</param>
        /// <param name="plural">Множественное число слова. Например "дней"</param>
        /// <returns></returns>
        public static string GetDeclension(int number, string nominativ, string genetiv, string plural)
        {
            number %= 100;
            if (number is >= 11 and <= 19)
            {
                return plural;
            }

            int i = number % 10;
            return i switch
            {
                1 => nominativ,
                2 or 3 or 4 => genetiv,
                _ => plural,
            };
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
