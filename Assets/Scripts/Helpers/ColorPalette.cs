using System;
using UnityEngine;

static class ColorPalette {

    public static readonly Color White = Color.white;
    public static readonly Color Black = FromHSV(0, 100, 2);

    public static readonly Color Grey = FromHSV(0, 0, 20);
    public static readonly Color DarkGrey = FromHSV(0, 30, 4);


    public static readonly Color Yellow = FromHSV(57, 98, 96);
    public static readonly Color Golden = FromHSV(48, 100, 100);

    public static readonly Color Orange = FromHSV(14, 91, 95);
    public static readonly Color DarkOrange = FromHSV(14, 91, 12);

    public static readonly Color Brown = FromHSV(16, 43, 20);
    public static readonly Color DarkBrown = FromHSV(16, 43, 8);

    public static readonly Color Red = FromHSV(0, 100, 75);
    public static readonly Color DarkRed = FromHSV(354, 98, 12);
    public static readonly Color Purple = FromHSV(259, 100, 32);

    public static readonly Color LightBlue = FromHSV(217, 79, 100);
    public static readonly Color Blue = FromHSV(227, 84, 60);
    public static readonly Color DarkBlue = FromHSV(227, 84, 8);

    public static readonly Color DarkCyan = FromHSV(151, 52, 55);

    public static readonly Color Green = FromHSV(127, 45, 57);
    public static readonly Color DarkGreen = FromHSV(193, 70, 11);









    private static Color FromHSV(float H, float S, float V) {
        return Color.HSVToRGB(H / 360.0f, S * 0.01f, V * 0.01f);
    }

}
