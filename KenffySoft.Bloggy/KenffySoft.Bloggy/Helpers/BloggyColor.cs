using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Helpers
{
    public static class BloggyColor
    {
        public static string Turquoise = "#1abc9c";
        public static string GreeenSea = "#16a085";
        public static string SunFlower = "#f1c40f";
        public static string Orange = "#f39c12";

        public static string Emerald = "#2ecc71";
        public static string Nephritis = "#27ae60";
        public static string Carrot = "#e67e22";
        public static string Pumpkin = "#d35400";

        public static string PeterRiver = "#3498db";
        public static string BelizeHole = "#2980b9";
        public static string Alizarin = "#e74c3c";
        public static string Pomegranate = "#c0392b";

        public static string Amethyst = "#9b59b6";
        public static string Visteria = "#8e44ad";
        public static string Clouds = "#ecf0f1";
        public static string Silver = "#bdc3c7";

        public static string WetAsphalt = "#34495e";
        public static string MidnightBlue = "#2c3e50";
        public static string Concrete = "#95a5a6";
        public static string Asbestos = "#7f8c8d";

        public static List<string> GetListHexColor()
        {
            List<string> Colors = new List<string>();
            Colors.Add(Turquoise);
            Colors.Add(GreeenSea);
            Colors.Add(SunFlower);
            Colors.Add(Orange);

            Colors.Add(Emerald);
            Colors.Add(Nephritis);
            Colors.Add(Carrot);
            Colors.Add(Pumpkin);

            Colors.Add(PeterRiver);
            Colors.Add(BelizeHole);
            Colors.Add(Alizarin);
            Colors.Add(Pomegranate);

            Colors.Add(Amethyst);
            Colors.Add(Visteria);
            Colors.Add(Clouds);
            Colors.Add(Silver);

            Colors.Add(WetAsphalt);
            Colors.Add(MidnightBlue);
            Colors.Add(Concrete);
            Colors.Add(Asbestos);

            return Colors;
        }

        public static string GetRandomHexColor()
        {
            var colors = GetListHexColor();
            var random = new Random();

            return colors[random.Next(0, colors.Count)];
        }

    }
}
