#define DEBUG

using System;
using System.Diagnostics;

public class DebugUtils
{
    [Conditional("DEBUG")]
    static void Assert(bool condition)
    {
        if (!condition) throw new Exception();
    }
}