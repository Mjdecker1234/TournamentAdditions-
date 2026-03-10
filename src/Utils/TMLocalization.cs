using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace TournamentMastery.Utils
{
    /// <summary>
    /// Wraps TaleWorlds' localization system. All user-visible strings should go through here.
    /// This keeps the mod localization-ready without requiring string table XML until the
    /// translator community provides translations.
    /// </summary>
    public static class TMLocalization
    {
        /// <summary>
        /// Resolve a localization key or fall back to <paramref name="fallback"/> if the
        /// key is not found.  Never throws.
        /// </summary>
        public static string Get(string key, string fallback)
        {
            try
            {
                var text = new TextObject(key);
                var resolved = text.ToString();
                return string.IsNullOrWhiteSpace(resolved) || resolved == key ? fallback : resolved;
            }
            catch
            {
                return fallback;
            }
        }

        /// <summary>
        /// Build a TextObject with named variable substitution.
        /// Usage: Format("{=TM_foo}Settlement: {SETTLEMENT}", "SETTLEMENT", "Epicrotea")
        /// </summary>
        public static string Format(string template, params (string key, object value)[] variables)
        {
            try
            {
                var obj = new TextObject(template);
                foreach (var (k, v) in variables)
                {
                    switch (v)
                    {
                        case string s: obj.SetTextVariable(k, s); break;
                        case int i:    obj.SetTextVariable(k, i); break;
                        case float f:  obj.SetTextVariable(k, f); break;
                        case Hero h:   obj.SetCharacterProperties(k, h.CharacterObject); break;
                        default:       obj.SetTextVariable(k, v.ToString()); break;
                    }
                }
                return obj.ToString();
            }
            catch (Exception ex)
            {
                TMLog.Debug($"Localization format error: {ex.Message}");
                return template;
            }
        }
    }
}
