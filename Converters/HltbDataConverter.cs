using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace SteamLike.Converters
{
    /// <summary>
    /// Container for HowLongToBeat hour values. Bound by the HltbPanel
    /// control (Main / Main + Extras / Completionist).
    /// </summary>
    public sealed class HltbInfo
    {
        public double? MainStory { get; set; }
        public double? MainExtra { get; set; }
        public double? Completionist { get; set; }

        public bool HasAnyData => MainStory.HasValue || MainExtra.HasValue || Completionist.HasValue;

        public static readonly HltbInfo Empty = new HltbInfo();
    }

    /// <summary>
    /// Resolves HowLongToBeat data for a Playnite Game from one of three
    /// sources, in priority order:
    ///   1. The HowLongToBeat plugin (Jeshibu) custom fields, accessed via
    ///      reflection on the Game object so this converter does not need a
    ///      hard reference to the plugin assembly.
    ///   2. Tags shaped like "HLTB:Main=12" / "HLTB:Extra=18" / "HLTB:Comp=30".
    ///   3. Links named "HowLongToBeat" - URL is preserved but no hours can
    ///      be parsed; the panel falls back to "—".
    ///
    /// Always returns a non-null HltbInfo (Empty when nothing matched) so
    /// XAML bindings stay null-safe.
    /// </summary>
    public sealed class HltbDataConverter : IValueConverter
    {
        private static readonly Regex HltbTagRegex = new Regex(
            @"^\s*HLTB[:\-\s]+(?<key>Main(\s*\+?\s*Extras?)?|Extra(s)?|Comp(letionist)?|All)\s*[:=]\s*(?<hours>[0-9]+(?:[.,][0-9]+)?)\s*h?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return HltbInfo.Empty;
            }

            var info = new HltbInfo();

            TryReadFromExtraProperties(value, info);
            if (!info.HasAnyData)
            {
                TryReadFromTags(value, info);
            }

            return info;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("HltbDataConverter is one-way.");
        }

        private static void TryReadFromExtraProperties(object game, HltbInfo info)
        {
            // The Jeshibu HLTB plugin exposes properties via a custom GameExtra
            // collection. We look for any IDictionary or IEnumerable property
            // on the game whose entries have a "Name" key matching one of the
            // HLTB labels.
            var type = game.GetType();
            var extraProp = type.GetProperty("GameExtraProperties", BindingFlags.Public | BindingFlags.Instance)
                            ?? type.GetProperty("ExtraProperties", BindingFlags.Public | BindingFlags.Instance);
            if (extraProp == null)
            {
                return;
            }

            if (!(extraProp.GetValue(game) is IEnumerable items))
            {
                return;
            }

            foreach (var item in items)
            {
                if (item == null) continue;
                var itemType = item.GetType();
                var nameProp = itemType.GetProperty("Name") ?? itemType.GetProperty("Key");
                var valueProp = itemType.GetProperty("Value");
                if (nameProp == null || valueProp == null) continue;

                var name = nameProp.GetValue(item) as string;
                var raw = valueProp.GetValue(item)?.ToString();
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(raw)) continue;

                AssignByLabel(info, name, raw);
            }
        }

        private static void TryReadFromTags(object game, HltbInfo info)
        {
            var tagsProp = game.GetType().GetProperty("Tags");
            if (tagsProp == null) return;
            if (!(tagsProp.GetValue(game) is IEnumerable tags)) return;

            foreach (var tag in tags)
            {
                if (tag == null) continue;
                var nameProp = tag.GetType().GetProperty("Name");
                var name = nameProp?.GetValue(tag) as string;
                if (string.IsNullOrWhiteSpace(name)) continue;

                var match = HltbTagRegex.Match(name);
                if (!match.Success) continue;

                AssignByLabel(info, match.Groups["key"].Value, match.Groups["hours"].Value);
            }
        }

        private static void AssignByLabel(HltbInfo info, string label, string raw)
        {
            if (!TryParseHours(raw, out var hours)) return;

            var normalized = (label ?? string.Empty).Replace(" ", string.Empty).ToLowerInvariant();

            if (normalized.Contains("main") && (normalized.Contains("extra") || normalized.Contains("+")))
            {
                info.MainExtra = hours;
            }
            else if (normalized.StartsWith("main"))
            {
                info.MainStory = hours;
            }
            else if (normalized.StartsWith("extra"))
            {
                info.MainExtra = hours;
            }
            else if (normalized.StartsWith("comp") || normalized.StartsWith("100") || normalized.Contains("completionist"))
            {
                info.Completionist = hours;
            }
            else if (normalized.StartsWith("all") || normalized.Contains("allstyles"))
            {
                if (!info.MainExtra.HasValue) info.MainExtra = hours;
            }
        }

        private static bool TryParseHours(string raw, out double hours)
        {
            raw = raw.Trim().TrimEnd('h', 'H').Trim().Replace(',', '.');
            return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out hours);
        }
    }
}
