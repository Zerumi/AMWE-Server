// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace m3md2
{
    public static partial class StaticVariables
    {
        public static class Diagnostics
        {
            public static string ProgramInfo { get; set; }
            public static int ExceptionCount = 0;
            public static List<Exception> exceptions = new List<Exception>();
        }
    }
}