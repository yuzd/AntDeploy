using System.Drawing;

namespace AltoControls
{
    public static class ExtensionMethods
    {
        public static Color FromHex(this string hex)
        {
            return ColorTranslator.FromHtml(hex);
        }
    }
}
