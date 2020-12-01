using System.Windows.Media;

namespace m3md2
{
    class ColorTheme
    {
        public string Name { get; set; }
        public Color[] Colors { get; set; }
    }

    public enum ColorIndex
    {
        Main,
        Second,
        Font,
        Extra
    }
}