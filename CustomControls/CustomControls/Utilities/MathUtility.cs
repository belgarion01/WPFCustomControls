using System;

namespace CustomControls.Utilities;

public class MathUtility
{

    public static double UnclampedLerp(double a, double b, double f)
    {
        return a + f * (b - a);
    }
    public static double Lerp(double a, double b, double f)
    {
        return Math.Clamp(UnclampedLerp(a, b, f), a, b);
    }

    public static double UnclampedInverseLerp(double a, double b, double f)
    {
        return (f - a) / (b - a);
    }
    public static double InverseLerp(double a, double b, double f)
    {
        return Math.Clamp(UnclampedInverseLerp(a, b, f), 0.0, 1.0);
    }
}