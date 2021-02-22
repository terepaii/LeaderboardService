using System.Diagnostics;

public static class Utility
{
    public static string GetFunctionName()
    {
        // Code from https://stackoverflow.com/questions/2652460/how-to-get-the-name-of-the-current-method-from-code
        return new StackTrace().GetFrame(1).GetMethod().Name;
    }
}