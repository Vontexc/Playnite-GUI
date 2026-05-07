# Playnite Theme „SteamLike" – Konzept

## 1. Projekt-Übersicht

- **Name:** SteamLike
- **Typ:** Playnite Desktop Theme (WPF / XAML)
- **Ziel:** Steam-ähnliches UI mit fokussiertem Game-Detail-View inkl. HowLongToBeat-Daten, animiertem Cover/Hintergrund und Trailer-Video-Playback
- **Zielversion:** Playnite 10+ (.NET 8)
- **Sprache:** XAML (deklarativ, kein Code-Behind möglich)

---

## 2. Design-System (Steam-Anlehnung)

### 2.1 Farbpalette

| Token | Hex | Verwendung |
|---|---|---|
| `BgDeep` | `#1B2838` | Haupt-Hintergrund |
| `BgPanel` | `#2A475E` | Panels, Cards |
| `BgElevated` | `#316282` | Hover, Sidebar |
| `AccentBlue` | `#66C0F4` | Primär-Akzent, Links |
| `AccentGreen` | `#A4D007` | Install-Button, „Play" |
| `TextPrimary` | `#C7D5E0` | Standardtext |
| `TextMuted` | `#8F98A0` | Meta-Infos |
| `GradientTop` | `#1B2838` | Header-Verlauf oben |
| `GradientBottom` | `#0E1822` | Header-Verlauf unten |

### 2.2 Typografie

- **Headline:** Motiva Sans (Fallback: Arial Bold)
- **Body:** Segoe UI / system default
- Großzügige Letter-Spacing bei Game-Titeln (Steam-typisch)

### 2.3 Layout-Prinzipien

- Diagonale Gradient-Overlays auf Hero-Bildern
- Abgerundete Ecken nur dezent (2–4 px)
- Buttons mit Linear-Gradient (Steam-Green-Button als Signature)
- Hover-States mit sanftem Glow (`AccentBlue`-Schatten)

---

## 3. Views & Layout

### 3.1 Library View (Grid)

- Cover-Grid wie Steam-Bibliothek (vertikales Capsule-Format 2:3)
- Hover: leichtes Scale + Glow + Spielname-Overlay
- Linke Sidebar: Plattformen, Kategorien, Tags (collapsible)
- Top-Bar: Suche, Sortierung, Filter, View-Switcher

### 3.2 Game Detail View (Kernstück)

Aufbau von oben nach unten:

1. **Hero-Bereich** (volle Breite, ~480 px Höhe)
   - Background: Game-Background-Image mit dunklem Gradient unten
   - Links: großes Cover (vertikal)
   - Rechts: Titel, Kurzinfo, Release-Datum, Entwickler, Tags
   - Trailer-Video-Player (autoplay, muted, loop) als Alternative zum Background

2. **Action-Bar**
   - Großer grüner „Play"-Button (Steam-Style)
   - Sekundär: Install/Uninstall, Edit, More

3. **HowLongToBeat-Panel** (Plugin-Integration)
   - 3 Karten nebeneinander: Main Story | Main + Extras | Completionist
   - Jede Karte: Stunden-Zahl groß, Label klein
   - Quelle: HowLongToBeat Plugin (Jeshibu) – Extra-Metadata-Felder

4. **Beschreibung** (collapsible, „Read more")

5. **Media-Galerie**
   - Screenshots-Carousel (aus `Game.BackgroundImage` + Extra-Images)
   - Trailer-Thumbnails klickbar → Vollbild-Player

6. **Details-Grid**
   - Genres, Features, Plattform, Spielzeit (Playnite), letzte Sitzung
   - Achievements (falls Plugin aktiv)

### 3.3 Top Panel

- Steam-typische horizontale Navigation
- Gradient-Background, weiche Trennlinien

### 3.4 Sidebar

- Dunkler als Hauptbereich
- Icons + Labels, aktiver Eintrag mit `AccentBlue`-Leftborder

---

## 4. Datenquellen & Bindings

### 4.1 Playnite-Bindings (Standard)

```xml
{Binding Game.Name}
{Binding Game.CoverImage}
{Binding Game.BackgroundImage}
{Binding Game.ReleaseDate}
{Binding Game.Developers}
{Binding Game.Genres}
{Binding Game.Playtime}
{Binding Game.Description}
```

### 4.2 HowLongToBeat (via Plugin-Felder)

HLTB-Daten landen in `Game.GameExtraProperties` oder als benannte Links/Tags. Beide Quellen unterstützen:

1. **HowLongToBeat Plugin (Jeshibu)** – speichert in custom fields
2. **Fallback:** parse aus `Game.Links` (Name = „HowLongToBeat")

Converter erstellen: `HltbDataConverter`, der ein `HltbInfo`-Objekt mit `MainStory`, `MainExtra`, `Completionist` zurückgibt.

### 4.3 Video-Trailer

- Quelle 1: `Game.GameExtraProperties` mit Trailer-URL
- Quelle 2: lokales File via Extra-Metadata-Plugin (`extramedia\trailer.mp4`)
- Player: `MediaElement` mit `LoadedBehavior=Manual`, AutoPlay konfigurierbar

---

## 5. Projektstruktur

```
SteamLike/
├── theme.yaml
├── theme.xaml                  # Globale Ressourcen
├── Styles/
│   ├── Colors.xaml
│   ├── Buttons.xaml
│   ├── TextStyles.xaml
│   └── Controls.xaml
├── Views/
│   ├── LibraryView.xaml
│   ├── GameDetails.xaml        # Hauptarbeit
│   ├── TopPanel.xaml
│   ├── Sidebar.xaml
│   └── Statusbar.xaml
├── Controls/
│   ├── HltbPanel.xaml          # 3-Karten-Komponente
│   ├── TrailerPlayer.xaml      # MediaElement + Controls
│   ├── ScreenshotCarousel.xaml
│   └── GameCoverCard.xaml
├── Converters/
│   ├── HltbDataConverter.cs
│   ├── PlaytimeFormatter.xaml
│   └── NullToVisibilityConverter.xaml
├── Fonts/
│   └── MotivaSans.ttf
└── Images/
    ├── logo.png
    └── placeholder_cover.png
```

> **Hinweis:** Playnite-Themes sind rein deklarativ (XAML) – kein Code-Behind. Logik muss in `IValueConverter`-Klassen oder XAML-Triggern/DataTemplates gelöst werden. Falls C#-Logik nötig: als kleines Companion-Plugin auslagern.

---

## 6. Implementierungs-Tasks (Phasenplan)

### Phase 1 – Setup
1. Projekt-Skeleton via `Toolbox.exe new DesktopTheme SteamLike` erzeugen
2. `theme.yaml` mit Manifest füllen (Name, Author, Version, PlayniteVersion)
3. Default-Theme als Vergleichsreferenz in `_reference/` ablegen

### Phase 2 – Design-System
4. `Styles/Colors.xaml` mit allen Farb-Tokens als `SolidColorBrush`
5. `Styles/Buttons.xaml` – Primary (grün), Secondary (blau), Ghost
6. `Styles/TextStyles.xaml` – H1, H2, Body, Caption
7. Motiva-Sans-Font einbinden, `FontFamily`-Resource definieren

### Phase 3 – Library
8. `LibraryView.xaml` – Cover-Grid mit `ItemsControl` + `WrapPanel`
9. `GameCoverCard.xaml` – Hover-Animationen via `VisualStateManager`
10. Sidebar mit Filter-Listen
11. Top-Panel mit Such- und Sortier-Bindings

### Phase 4 – Game Details (Kern)
12. `GameDetails.xaml` Grundlayout (Hero + Sections)
13. `TrailerPlayer.xaml` – `MediaElement` mit Mute/Play-Toggle, Fallback auf BackgroundImage
14. `HltbPanel.xaml` – 3-Karten-Layout, Bindings + Converter
15. `HltbDataConverter.cs` – liest aus Extra-Properties / Links / Tags
16. `ScreenshotCarousel.xaml` – horizontaler Scroller mit Thumbnails
17. Action-Bar mit Play-Button (Steam-Green-Gradient)
18. Details-Grid (Genre, Plattform, Playtime, …)

### Phase 5 – Polishing
19. Hover- und Transition-Animationen (`Storyboard`)
20. Placeholder-Bilder für fehlende Cover/Backgrounds
21. Dark/Light Toggle (optional, Steam ist primär dark)
22. Responsivität testen (kleine Fenster, 4K)

### Phase 6 – Release
23. Screenshots + README
24. `Toolbox.exe pack` → `.pthm`-Datei
25. Optional: Submission ins offizielle Playnite-Theme-Repo

---

## 7. Risiken & offene Fragen

- **HLTB-Daten-Format:** plugin-abhängig → Phase 4 mit echtem Beispiel-Game testen
- **Video-Codecs:** `MediaElement` braucht Windows Media Foundation; MP4 (H.264) sicher, WebM eher nicht
- **Performance:** Autoplay-Trailer im Detail-View pausieren beim Verlassen der View (`DataTrigger` auf `IsVisible`)
- **Logik ohne Code-Behind:** komplexere Fälle ggf. als Companion-Extension lösen

---

## 8. Arbeitsanweisung für Claude Code

> Lies dieses Konzept (`KONZEPT.md`) und beginne mit **Phase 1 + 2**. Erzeuge das Theme-Skeleton im aktuellen Verzeichnis, lege `theme.yaml`, `theme.xaml` und alle Styles-Dateien aus Phase 2 an. Verwende die Farb-Tokens aus Abschnitt 2. Halte dich strikt an die Projektstruktur in Abschnitt 5. Schreibe keinen Code-Behind. Frage nach, bevor du Phase 3 beginnst.

### Verbindliche Regeln

- Nur XAML, keine `.cs`-Code-Behind-Files (außer `Converters/*.cs`)
- Alle Farben/Fonts nur über Ressourcen aus `Styles/Colors.xaml` und `Styles/TextStyles.xaml` referenzieren
- Keine Hardcoded-Hex-Werte in Views
- Bindings müssen Null-safe sein (Fallback-Werte)
- Jede neue Datei am Ende der Phase committen
