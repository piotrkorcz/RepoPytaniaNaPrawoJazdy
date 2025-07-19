
public static class GeneralUtils
{
    public static int ToInt(this bool value)
    {
        return value ? 1 : 0;
    }

    public static bool ToBool(this int value)
    {
        return value > 0;
    }

}
