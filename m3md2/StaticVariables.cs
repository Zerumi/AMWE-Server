// This code & software is licensed under the Creative Commons license. 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Net;

namespace m3md2
{
    public static class StaticVariables
    {
#pragma warning disable CA2211 // Поля, не являющиеся константами, не должны быть видимыми
        public static int ExceptionCount;
        public static UpgList<Exception> exceptions = new();
        public static string BaseScriptAddress = "https://vefez-script.glitch.me/";
        public static string BaseServerAddress = "";
        public static Cookie AuthCookie { get; set; }
#pragma warning restore CA2211 // Поля, не являющиеся константами, не должны быть видимыми
    }
}