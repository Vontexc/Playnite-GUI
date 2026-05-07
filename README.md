# SteamLike — Playnite Desktop Theme

Steam-inspired desktop theme for [Playnite](https://playnite.link/) 10+ (.NET 8).
Focused on a hero-driven game detail view with HowLongToBeat integration,
trailer playback and the Steam-typical dark palette.

> Status: **v0.1.0** — feature-complete for first private use. No screenshots
> yet (need a live Playnite install to capture).

---

## Highlights

- **Cover-grid library** with hover scale, glow, and game-name overlay.
- **Game detail view** with hero, action bar, HLTB 3-card panel, description,
  screenshot carousel, details grid.
- **Trailer playback** via `MediaElement` with mute toggle (no code-behind).
- **Vector placeholder** for missing covers — no bitmap dependency.
- **HowLongToBeat integration** via the optional companion plugin (see below).
- Pure XAML — only the HLTB converter ships as C#.

---

## Install (manual)

1. Drop the theme folder into Playnite's themes directory:
   ```
   %AppData%\Playnite\Themes\Desktop\SteamLike\
   ```
2. (Optional) Install the companion plugin for HLTB data — see below.
3. In Playnite: **Settings → Appearance → Theme → SteamLike**.

Or pack it as a `.pthm` and double-click — see *Packing* below.

---

## Optional: HowLongToBeat companion plugin

The HLTB 3-card panel needs `SteamLike.Converters.HltbDataConverter`. Playnite
themes do not compile their own `.cs` files, so the converter is shipped as a
tiny extension that just exposes the type to XAML.

Build it once with the Playnite SDK:

```
cd Companion
dotnet build -c Release
```

Then copy the build output to:

```
%AppData%\Playnite\Extensions\SteamLike.Companion\
```

(The Companion project references `Playnite.SDK` from NuGet. Adjust the SDK
version in `SteamLike.Companion.csproj` if you target a different Playnite
build.)

Without the companion plugin the panel still renders — every card just shows
`—` because every binding has `FallbackValue='—'`.

---

## Customization

All theme tokens live in `Styles/Colors.xaml`. The most useful overrides:

| Token | Default | Effect |
|---|---|---|
| `BgDeepBrush` | `#1B2838` | Main app background |
| `BgPanelBrush` | `#2A475E` | Cards, panels, action bar |
| `AccentBlueBrush` | `#66C0F4` | Links, hover glow, focus |
| `AccentGreenBrush` | `#A4D007` | Play button gradient |
| `HeadlineFontFamily` | Motiva Sans → Arial → Segoe UI | Game titles, section headers |

To swap to a custom font, drop a `.ttf` into `Fonts/` and update the
`HeadlineFontFamily` resource in `Styles/TextStyles.xaml`.

---

## Project layout

```
SteamLike/
├── theme.yaml                  # Playnite manifest
├── theme.xaml                  # global resource merge
├── Styles/
│   ├── Colors.xaml             # palette + gradients
│   ├── TextStyles.xaml         # H1/H2/Body/HLTB type styles
│   ├── Buttons.xaml            # PrimaryPlay / Secondary / Ghost / Icon
│   ├── Controls.xaml           # search box, list items, combo, chips
│   ├── Placeholders.xaml       # vector controller icon
│   └── Animations.xaml         # FadeIn / SlideUpFadeIn
├── Views/
│   ├── LibraryView.xaml        # cover grid + sidebar + top panel
│   ├── GameDetails.xaml        # hero · actions · HLTB · about · media · details
│   ├── TopPanel.xaml           # nav · search · sort · view switcher
│   ├── Sidebar.xaml            # libraries / categories / tags
│   └── Statusbar.xaml          # footer rail
├── Controls/
│   ├── GameCoverCard.xaml      # 2:3 capsule with hover VSM
│   ├── HltbPanel.xaml          # 3-card HLTB readout
│   ├── TrailerPlayer.xaml      # MediaElement + mute toggle
│   └── ScreenshotCarousel.xaml # horizontal thumbnails
├── Converters/
│   ├── HltbDataConverter.cs        # built into Companion plugin
│   ├── PlaytimeFormatter.xaml      # format-string resources
│   └── NullToVisibilityConverter.xaml  # pure-XAML null helpers
├── Companion/                  # optional Playnite extension (HLTB)
└── pack.ps1                    # wrapper around Toolbox.exe pack
```

---

## Packing

Playnite ships `Toolbox.exe`. To produce a `.pthm` file from this checkout:

```
.\pack.ps1 -PlayniteRoot "C:\Program Files\Playnite" -Output ".\dist"
```

Or directly:

```
& "C:\Program Files\Playnite\Toolbox.exe" pack . dist
```

The result is `dist\SteamLike_<version>.pthm` — drop it on a running Playnite
window to install.

---

## Known limitations

- **Trailer looping** is unsupported because WPF's `MediaElement` cannot loop
  declaratively. The trailer plays once and stops. A future Companion attached
  behavior can rewind on `MediaEnded`.
- **Bindings target Playnite 10's `MainModel.GamesView.CollectionView` /
  `MainModel.SearchText` shape.** All of them carry `FallbackValue` so the
  view renders empty if a path is wrong on your build, but you may need to
  adapt paths for older Playnite versions.
- **`Game.TrailerVideoPath` and `Game.Screenshots` are not Playnite core
  properties.** They come from the Extra-Metadata-Loader plugin or similar.
  Without it the trailer slot falls back to the still background image.

---

## License

Not yet decided — pick before public submission to the Playnite theme repo.
