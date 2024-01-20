using System;
using System.Threading;

namespace TemplateWorkApp
{
    ///<summary>
    ///статический класс описывающий оповещения возникающие в приложении
    /// </summary>
    internal static class Alert
    {
        public static void CustomAlert(string alertMessage, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(alertMessage);
            Console.ResetColor();
            Thread.Sleep(4000);
            Console.Clear();
        }
        public static void CloseAppAlert()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Программа завершает свою работу");
            Thread.Sleep(2000);
            Console.Clear();
            Environment.Exit(0);
        }
    }
}
