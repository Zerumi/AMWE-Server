// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace m3md2
{
    public static class ColorThemes
    {
        /// <summary>
        /// Получает цветовую тему по ее имени
        /// </summary>
        /// <param name="name">Название цветовой темы</param>
        /// <returns>Массив цветов этой темы</returns>
        public static Color[] GetColors(string name)
        {
            Color[] colors = colorthemes.Find(x => x.Name == name).Colors;
            if (colors == default)
            {
                _ = MessageBox.Show($"(1.2) Тема {name} не была найдена. Вероятно, она была удалена из программы. Последняя удаленная тема: Pinkerity");
                colors = colorthemes.Find(x => x.Name == "Standard").Colors;
            }
            return colors;
        }

        /// <summary>
        /// Получает градиент цветовой темы по ее имени
        /// </summary>
        /// <param name="name">Название цветовой темы</param>
        /// <returns>Массив цветов этой темы</returns>
        public static Color[] GetGradient(string name)
        {
            Color[] colors = colorthemes.Find(x => x.Name == name).ReportGradient;
            if (colors == default)
            {
                _ = MessageBox.Show($"(1.19) Градиент {name} не был найден. Вероятно, у вас устаревшая версия библиотеки m3md2.dll.");
                colors = colorthemes.Find(x => x.Name == "Standard").ReportGradient;
            }
            return colors;
        }

        public static string[] GetColorNames()
        {
            return colorthemes.Select(x => x.Name).ToArray();
        }

        private static readonly List<ColorTheme> colorthemes = new()
        {
            new ColorTheme()
            {
                Name = "Standard",
                Colors = new Color[]
                {
                    SystemColors.InfoColor, // main color
                    Color.FromRgb(255,255,255), // second color
                    Color.FromRgb(0,0,0), // font color
                    Color.FromRgb(255,255,255), // third color
                    Color.FromRgb(67,181,129), // green color
                    Color.FromRgb(240,71,71), // red color
                    SystemColors.ControlColor, // control color
                    Color.FromRgb(68, 114, 196) // linechart color
                },
                ReportGradient = new Color[]
                {
                    (Color)ColorConverter.ConvertFromString("#C0ff0000"), // 0.1
                    (Color)ColorConverter.ConvertFromString("#C0ff4000"), // 0.2
                    (Color)ColorConverter.ConvertFromString("#C0ff8000"), // 0.3
                    (Color)ColorConverter.ConvertFromString("#C0ffc000"), // 0.4
                    (Color)ColorConverter.ConvertFromString("#C0ffff00"), // 0.5
                    (Color)ColorConverter.ConvertFromString("#C0c0ff00"), // 0.6
                    (Color)ColorConverter.ConvertFromString("#C0a0ff00"), // 0.7
                    (Color)ColorConverter.ConvertFromString("#C080ff00"), // 0.8
                    (Color)ColorConverter.ConvertFromString("#C060ff00"), // 0.9
                    (Color)ColorConverter.ConvertFromString("#C020ff00") // 1.0
                }
            },
            new ColorTheme()
            {
                Name = "Hackerman",
                Colors = new Color[]
                {
                    Color.FromRgb(66,255,91), // main color
                    Color.FromRgb(66,230,255), // second color
                    Color.FromRgb(0,0,0), // font color
                    Color.FromRgb(148,255,66), // third color
                    Color.FromRgb(67,181,129), // green color
                    Color.FromRgb(240,71,71), // red color
                    SystemColors.ControlColor, // control color
                    Color.FromRgb(68, 114, 196) // linechart color
                },
                ReportGradient = new Color[]
                {
                    (Color)ColorConverter.ConvertFromString("#C0ff0000"), // 0.1
                    (Color)ColorConverter.ConvertFromString("#C0ff4000"), // 0.2
                    (Color)ColorConverter.ConvertFromString("#C0ff8000"), // 0.3
                    (Color)ColorConverter.ConvertFromString("#C0ffc000"), // 0.4
                    (Color)ColorConverter.ConvertFromString("#C0ffff00"), // 0.5
                    (Color)ColorConverter.ConvertFromString("#C0c0ff00"), // 0.6
                    (Color)ColorConverter.ConvertFromString("#C0a0ff00"), // 0.7
                    (Color)ColorConverter.ConvertFromString("#C080ff00"), // 0.8
                    (Color)ColorConverter.ConvertFromString("#C060ff00"), // 0.9
                    (Color)ColorConverter.ConvertFromString("#C020ff00") // 1.0
                }
            },
            new ColorTheme()
            {
                Name = "Dark",
                Colors = new Color[]
                {
                    Color.FromRgb(47,49,54), // main color
                    Color.FromRgb(54,57,63), // second color
                    Color.FromRgb(227,225,230), // font color
                    Color.FromRgb(64,68,75), // third color
                    Color.FromRgb(67,181,129), // green color
                    Color.FromRgb(240,71,71), // red color
                    Color.FromRgb(32,34,37), // control color
                    Color.FromRgb(255, 239, 0) // linechart color
                },
                ReportGradient = new Color[]
                {
                    (Color)ColorConverter.ConvertFromString("#ff231f"), // 0.1
                    (Color)ColorConverter.ConvertFromString("#fb451e"), // 0.2
                    (Color)ColorConverter.ConvertFromString("#fd6131"), // 0.3
                    (Color)ColorConverter.ConvertFromString("#ef6f32"), // 0.4
                    (Color)ColorConverter.ConvertFromString("#f48416"), // 0.5
                    (Color)ColorConverter.ConvertFromString("#1f4140"), // 0.6
                    (Color)ColorConverter.ConvertFromString("#01515c"), // 0.7
                    (Color)ColorConverter.ConvertFromString("#006876"), // 0.8
                    (Color)ColorConverter.ConvertFromString("#009fa2"), // 0.9
                    (Color)ColorConverter.ConvertFromString("#00b7b3") // 1.0
                }
            }
        };
    }
}