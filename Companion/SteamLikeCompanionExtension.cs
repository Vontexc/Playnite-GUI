using System;
using Playnite.SDK;
using Playnite.SDK.Plugins;

namespace SteamLike.Companion
{
    /// <summary>
    /// Minimal Playnite extension whose only job is to be loaded by Playnite
    /// at startup so the SteamLike theme can resolve types from this
    /// assembly via XAML xmlns:
    ///   xmlns:conv="clr-namespace:SteamLike.Converters;assembly=SteamLike.Companion"
    ///
    /// No UI, no commands, no settings.
    /// </summary>
    public class SteamLikeCompanionExtension : GenericPlugin
    {
        // Stable, randomly chosen GUID. Must not change between releases.
        public override Guid Id { get; } = Guid.Parse("5E7D8C1A-7F0A-4E5C-9C2B-1B2D7C8A4F31");

        public SteamLikeCompanionExtension(IPlayniteAPI api) : base(api)
        {
            Properties = new GenericPluginProperties
            {
                HasSettings = false
            };
        }
    }
}
