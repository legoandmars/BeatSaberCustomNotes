using System;
using System.Reflection;
using System.Diagnostics;
using IPALogger = IPA.Logging.Logger;
using LogLevel = IPA.Logging.Logger.Level;

namespace CustomNotes
{
    public static class Logger
    {
        internal static IPALogger log { get; set; }

        public static void Log(string message, LogLevel severity = LogLevel.Info)
        {
            string caller = new StackTrace()?.GetFrame(1)?.GetMethod()?.ReflectedType?.FullName ?? Assembly.GetCallingAssembly().GetName().Name;

            if (log != null) log.Log(severity, $"{caller} - {message}");
            else Console.WriteLine($"[{Plugin.PluginName}] {severity.ToString().ToUpper()} {caller} - {message}");
        }

        public static void Log(Exception error, LogLevel severity = LogLevel.Error)
        {
            string caller = new StackTrace()?.GetFrame(1)?.GetMethod()?.ReflectedType?.FullName ?? Assembly.GetCallingAssembly().GetName().Name;

            if (log != null) log.Log(severity, error);
            else Console.WriteLine($"[{Plugin.PluginName}] {severity.ToString().ToUpper()} {caller} - {error.Message}\n{error.StackTrace}");
        }
    }
}
