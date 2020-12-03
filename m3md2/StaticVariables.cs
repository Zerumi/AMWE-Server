// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;

namespace m3md2
{
    public static partial class StaticVariables
    {
        public static class Diagnostics
        {
            public static string ProgramInfo { get; set; }
#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
            public static int ExceptionCount = 0;
            public static List<Exception> exceptions = new List<Exception>();
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        }
    }
}