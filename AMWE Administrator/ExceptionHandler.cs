// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System;
using System.Windows;

namespace AMWE_Administrator
{
    static class ExceptionHandler
    {
        private static void RegisterToM3MD2(Exception ex)
        {
            m3md2.StaticVariables.exceptions.Add(ex);
            m3md2.StaticVariables.ExceptionCount++;
        }
        public static void RegisterNew(Exception ex)
        {
            MessageBox.Show(ex.ToString());
            RegisterToM3MD2(ex);
        }
        public static void RegisterNew(Exception ex, bool iswithmessage)
        {
            if (iswithmessage)
            {
                MessageBox.Show(ex.ToString());
            }
            RegisterToM3MD2(ex);
        }
    }
}
