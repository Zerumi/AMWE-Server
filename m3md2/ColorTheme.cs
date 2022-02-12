// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Windows.Media;

namespace m3md2
{
    internal class ColorTheme
    {
        public string Name { get; set; }
        public Color[] Colors { get; set; }
        public Color[] ReportGradient { get; set; }
    }

    public enum ColorIndex
    {
        Main,
        Second,
        Font,
        Extra,
        Green,
        Red,
        Control,
        LineChart,
        Border,
        Button,
        ButtonHighlight
    }
}