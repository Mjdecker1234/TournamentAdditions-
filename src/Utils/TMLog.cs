using System;
using TaleWorlds.Library;
using TournamentMastery.Settings;

namespace TournamentMastery.Utils
{
    /// <summary>
    /// Lightweight logging wrapper. Writes to the Bannerlord debug output channel.
    /// Debug messages are gated behind the MCM DebugLogging toggle.
    /// </summary>
    public static class TMLog
    {
        private const string Prefix = "[TournamentMastery] ";

        public static void Info(string message)
            => InternalLog(message, Color.ConvertStringToColor("#aaddff"));

        public static void Warning(string message)
            => InternalLog("WARNING: " + message, Color.ConvertStringToColor("#ffdd55"));

        public static void Error(string message)
            => InternalLog("ERROR: " + message, Color.ConvertStringToColor("#ff5555"));

        public static void Debug(string message)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.DebugLogging) return;
            InternalLog("[DEBUG] " + message, Color.ConvertStringToColor("#99ff99"));
        }

        public static void Exception(Exception ex, string context = "")
        {
            var ctx = string.IsNullOrWhiteSpace(context) ? string.Empty : $"({context}) ";
            InternalLog($"EXCEPTION {ctx}{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}", Color.ConvertStringToColor("#ff5555"));
        }

        private static void InternalLog(string message, Color color)
        {
            try
            {
                InformationManager.DisplayMessage(new InformationMessage(Prefix + message, color));
                MBLog.DebugLog(Prefix + message);
            }
            catch
            {
                // Swallow logging failures to avoid secondary crashes.
            }
        }
    }
}
