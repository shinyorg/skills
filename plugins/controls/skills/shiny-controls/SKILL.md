---
name: shiny-controls
description: Generate UI for .NET MAUI (Shiny.Maui.Controls) and Blazor (Shiny.Blazor.Controls) - includes TableView with 14 cell types, TreeView with lazy loading, drag/drop reorder (above/below/into), and configurable expand/collapse icons, FloatingPanel/OverlayHost/ShinyContentPage (bottom/top overlay panels) with detents and header peek, DurationPicker (duration picker with FloatingPanel), FrostedGlassView (native blur/glass effect), Toast service (code-invoked toast notifications with queue/stack, auto-dismiss, spinner, progress bar, pill/fill modes), PillView status badges, BadgeView (content-wrapping corner badge with text/dot/count overflow and pulse), ImageViewer with pinch/pan/double-tap zoom, ImageEditor with crop/rotate/draw/text/undo/redo/export, ChatView with bubbles/typing/load-more/input-bar and custom MessageTemplate/MessageTemplateSelector for per-message rendering, SecurityPin entry, Fab and FabMenu (floating action button and expanding action menu), Scheduler views (calendar grid, agenda timeline, event list), Markdown controls (MarkdownView renderer, MarkdownEditor with toolbar), Barcodes & QR codes (separate Shiny.Maui.Controls.Barcodes / Shiny.Blazor.Controls.Barcodes packages — BarcodeView and QRCodeView with 13 symbologies including QR, Aztec, Data Matrix, PDF417, Code 128/39/93, Codabar, EAN-8/13, UPC-A/E, ITF — pure-managed ZXing.Net renderer, PNG output via built-in encoder on MAUI, SVG or PNG data-URI on Blazor, with a static BarcodeRenderer for raw bytes / SVG / data-URI from code), CameraView (separate Shiny.Maui.Controls.Camera / Shiny.Blazor.Controls.Camera packages — cross-platform camera preview on iOS, Android, Windows, macOS AppKit, and Blazor WASM with zoom, torch, lens/device selection, photo + video capture, and live color filters (Mono/Noir/Sepia/Vivid/Cool/Warm/Fade/Chrome/Instant/Tonal) applied to the live preview and captured photos; a pluggable IFrameAnalyzer pipeline where each analyzer raises its own strongly-typed event (or bindable Command), can be declared right in XAML, added/removed live or toggled on/off via IsEnabled, and draws styled OverlayBoxes via the built-in CameraOverlayView; modular analyzer add-ons Shiny.Maui.Controls.Camera.Barcode/.Face/.Motion/.Ocr/.Documents scan barcodes, detect faces and motion (a box per distinct moving region), run OCR, and extract structured documents — invoices with order lines, AAMVA driver's licenses, health cards, credit cards (brand via IIN+Luhn), and passports (deterministic MRZ) each as their own analyzer returning a strong record with nullable fields + a typed event; registered with .UseShinyCamera()), AutoCompleteEntry with debounced search and dropdown suggestions, CountryPicker with flag/dial code, AddressEntry with geocoding, SignaturePad for capturing signatures with canvas drawing and PNG export, TextEntry with animated floating placeholder/customizable border/tool slots/validation hints/character count, Slider with two-color gradient track and blended thumb, ProgressBar with gradient fill and Vista-style shimmer pulse sweep, ParallaxCollectionView (MAUI) / ParallaxList (Blazor) — a scrollable list with a hero header that translates at a configurable fraction of the scroll offset, with optional collapse-to-sticky and fade, Overlay/LoadingOverlay (full-screen overlay with configurable color/opacity, custom content template, and built-in loading mode with indeterminate spinner or determinate progress bar), SkeletonView (content-wrapping skeleton loader that shows animated shimmer placeholders while IsBusy is true, with built-in line placeholders or a custom placeholder template), Desktop add-on (separate Shiny.Maui.Controls.Desktop package) combining Tray Icon (cross-platform system tray / status-bar icon with context menus, click events, tooltips, and dynamic visibility on Windows/macOS AppKit/MacCatalyst/Linux), Docking (Visual-Studio-style dockable tool windows, tabbed groups, splitters, auto-hide rails, and tear-off floating windows), and an On-Screen Keyboard (touch / kiosk QWERTY soft keyboard with shift / numbers / symbols layers, bottom-docked auto-show on focus, dispatches into focused inputs without stealing focus, full a11y tree for switch input); companion Shiny.Blazor.Controls.Kiosk for Blazor packages docking + on-screen keyboard under one kiosk-shaped Blazor add-on, Feedback Service (extensible IFeedbackService with haptic default, replaceable with TTS/sound/analytics), and UseFeedback support across all interactive controls
auto_invoke: true
triggers:
  - tableview
  - table view
  - settings page
  - settings view
  - settingsview
  - treeview
  - tree view
  - tree control
  - hierarchical view
  - hierarchy
  - file browser
  - folder browser
  - folder picker
  - directory tree
  - lazy load tree
  - expandable list
  - nested list
  - tree node
  - org chart
  - blazor treeview
  - blazor tree view
  - sheet view
  - sheetview
  - bottom sheet
  - bottomsheet
  - floating panel
  - floatingpanel
  - overlay host
  - overlayhost
  - shiny content page
  - shinycontentpage
  - pill
  - badge
  - status badge
  - scheduler
  - calendar
  - agenda
  - event list
  - calendar view
  - timeline
  - image viewer
  - imageviewer
  - image zoom
  - pinch to zoom
  - photo viewer
  - image editor
  - imageeditor
  - image editing
  - crop image
  - draw on image
  - annotate image
  - image annotation
  - photo editor
  - media picker
  - mediapicker
  - media picker button
  - photo picker
  - pick photo
  - pick image
  - choose photo
  - take photo
  - add photos
  - image upload
  - photo upload
  - gallery picker
  - camera capture button
  - compress photo
  - image compression
  - photo carousel
  - markdown
  - markdown view
  - markdown editor
  - markdown preview
  - rich text
  - security pin
  - securitypin
  - pin code
  - pin entry
  - otp
  - one time password
  - pin control
  - fab
  - floating action button
  - floating button
  - fab menu
  - fabmenu
  - speed dial
  - action menu
  - action button
  - shiny blazor controls
  - blazor tableview
  - blazor sheetview
  - blazor bottomsheet
  - blazor floating panel
  - blazor fab
  - blazor pillview
  - blazor imageviewer
  - blazor imageeditor
  - blazor securitypin
  - blazor scheduler
  - blazor markdown
  - blazor mermaid
  - autocomplete
  - auto complete
  - autocompleteentry
  - auto complete entry
  - search input
  - typeahead
  - type ahead
  - country picker
  - countrypicker
  - country selector
  - country search
  - address entry
  - addressentry
  - address search
  - address lookup
  - geocoding
  - geocode
  - blazor autocomplete
  - blazor country picker
  - blazor address entry
  - chat
  - chatview
  - chat view
  - chat bubbles
  - messaging
  - chat control
  - typing indicator
  - blazor chatview
  - signature pad
  - signaturepad
  - signature
  - signature capture
  - sign here
  - e-signature
  - esignature
  - draw signature
  - blazor signaturepad
  - blazor signature
  - text entry
  - textentry
  - text field
  - text input
  - material entry
  - floating placeholder
  - floating label
  - blazor textentry
  - blazor text entry
  - stepper
  - stepper tool
  - textentrystepper
  - gradient slider
  - gradientslider
  - slider
  - range slider
  - rangeslider
  - two thumb slider
  - dual slider
  - min max slider
  - temperature slider
  - blazor slider
  - blazor gradient slider
  - duration picker
  - durationpicker
  - shinydurationpicker
  - frosted glass
  - frostedglass
  - glass effect
  - blur effect
  - acrylic
  - backdrop blur
  - glassmorphism
  - chat template
  - message template
  - chat button
  - action message
  - toast
  - toast notification
  - toast service
  - snackbar
  - show toast
  - blazor toast
  - dialog
  - dialogs
  - alert
  - confirm
  - prompt
  - datagrid
  - data grid
  - grid
  - table with sorting
  - sortable table
  - filterable grid
  - mudblazor datagrid
  - column grid
  - editable grid
  - alert confirm prompt
  - message box
  - blazor dialog
  - feedback service
  - ifeedbackservice
  - haptic
  - haptic feedback
  - custom feedback
  - usefeedback
  - progress bar
  - progressbar
  - progress indicator
  - loading bar
  - gradient progress
  - shimmer
  - vista progress
  - blazor progressbar
  - gradient slider
  - gradientslider
  - range slider
  - temperature slider
  - blazor gradient slider
  - overlay
  - loading overlay
  - loadingoverlay
  - busy overlay
  - spinner overlay
  - progress overlay
  - blazor overlay
  - carousel
  - carousel gallery
  - carouselgallery
  - netflix carousel
  - horizontal scroll
  - snap carousel
  - staggered grid
  - staggeredgrid
  - masonry
  - waterfall layout
  - pinterest grid
  - pinterest layout
  - virtualized grid
  - virtualizedgrid
  - grouped grid
  - sticky headers
  - load more
  - parallax
  - parallax collection view
  - parallaxcollectionview
  - parallax list
  - parallaxlist
  - parallax header
  - parallax scroll
  - hero header
  - collapsing header
  - sticky header collapse
  - blazor carousel
  - blazor staggered grid
  - blazor virtualized grid
  - blazor parallax
  - skeleton
  - skeleton loader
  - skeleton view
  - skeletonview
  - shimmer
  - shimmer loading
  - shimmer placeholder
  - loading placeholder
  - content placeholder
  - placeholder loading
  - badgeview
  - badge view
  - notification badge
  - notification dot
  - count badge
  - unread badge
  - unread count
  - inbox badge
  - cart badge
  - corner badge
  - blazor badge
  - blazor badgeview
  - tray icon
  - trayicon
  - system tray
  - system tray icon
  - status bar icon
  - status bar app
  - menu bar icon
  - menu bar app
  - menubar
  - notification area
  - taskbar icon
  - shell_notifyicon
  - notifyicon
  - nsstatusitem
  - nsstatusbar
  - app indicator
  - appindicator
  - libappindicator
  - libayatana-appindicator
  - statusnotifieritem
  - desktop tray
  - maui desktop tray
  - background app
  - tray menu
  - tray context menu
  - docking
  - dock host
  - dockhost
  - dockable
  - tool window
  - tool windows
  - tabbed panel
  - tabbed group
  - dock panel
  - dockpanel
  - visual studio docking
  - visual studio layout
  - vs docking
  - tear off
  - tear-off
  - floating window
  - floating panel docking
  - splitter
  - dock splitter
  - auto-hide rail
  - auto-hide panel
  - blazor docking
  - on-screen keyboard
  - onscreen keyboard
  - on screen keyboard
  - osk
  - virtual keyboard
  - soft keyboard
  - software keyboard
  - touch keyboard
  - kiosk keyboard
  - tablet keyboard
  - touchscreen keyboard
  - touch-screen keyboard
  - blazor keyboard
  - maui keyboard
  - bottom docked keyboard
  - qwerty
  - qwerty keyboard
  - kiosk mode
  - barcode
  - barcodes
  - barcode view
  - barcodeview
  - qr code
  - qrcode
  - qr code view
  - qrcodeview
  - qr generator
  - barcode generator
  - barcode renderer
  - ean
  - ean-13
  - ean13
  - upc
  - upc-a
  - upca
  - code128
  - code 128
  - code39
  - code 39
  - code93
  - pdf417
  - aztec
  - data matrix
  - datamatrix
  - codabar
  - itf
  - itf-14
  - zxing
  - render barcode
  - generate qr
  - generate qr code
  - blazor barcode
  - blazor qr code
  - blazor qrcode
  - camera
  - cameraview
  - camera view
  - camera preview
  - take photo
  - capture photo
  - record video
  - video recording
  - camera filter
  - scan barcode
  - barcode scanner
  - qr scanner
  - scan qr code
  - face detection
  - detect faces
  - motion detection
  - ocr
  - text recognition
  - read text from camera
  - invoice scanner
  - document scanner
  - scan drivers license
  - drivers license scanner
  - aamva
  - pdf417 scan
  - health card scanner
  - extract invoice line items
  - credit card scanner
  - scan credit card
  - card number ocr
  - passport scanner
  - scan passport
  - mrz reader
  - machine readable zone
  - frame analysis
  - getusermedia
  - webcam
  - torch
  - flashlight
  - flip camera
  - front camera
  - back camera
references:
  - tableview.md
  - datagrid.md
  - treeview.md
  - floating-panel.md
  - pillview.md
  - image-viewer.md
  - image-editor.md
  - media-picker-button.md
  - chatview.md
  - security-pin.md
  - fab.md
  - markdown.md
  - scheduler.md
  - autocomplete.md
  - country-picker.md
  - address-entry.md
  - signature-pad.md
  - pickers.md
  - frosted-glass.md
  - toast.md
  - dialogs.md
  - textentry.md
  - slider.md
  - range-slider.md
  - progressbar.md
  - overlay.md
  - skeleton.md
  - badge.md
  - tray-icon.md
  - docking.md
  - onscreen-keyboard.md
  - barcodes.md
  - camera.md
  - feedback-service.md
---

# Shiny Controls Skill

You are an expert in the Shiny Controls library, which ships a single shared control surface across two hosts:
- **.NET MAUI** — `Shiny.Maui.Controls` (plus `Shiny.Maui.Controls.Markdown`, `Shiny.Maui.Controls.MermaidDiagrams`)
- **Blazor** — `Shiny.Blazor.Controls` (plus `Shiny.Blazor.Controls.Markdown`, `Shiny.Blazor.Controls.MermaidDiagrams`)

Every control below is available on **both** MAUI and Blazor. The feature set (properties, events, behavior) is intentionally mirrored — the same concepts apply on either host; only the syntax differs (XAML + `BindableProperty` on MAUI, Razor markup + `[Parameter]` on Blazor).

The library contains:
- **TableView**: A pure MAUI settings-style TableView with 14 cell types, cascading styles, sections, drag-sort reordering, and full MVVM/binding support
- **DataGrid** (MAUI + Blazor): A feature-rich data grid modeled on MudBlazor. Blazor is a generic `DataGrid<TItem>` rendering an HTML `<table>` with child `PropertyColumn`/`TemplateColumn`; MAUI is a pure cross-platform composite (`shiny:DataGrid` + `DataGridColumn`/`DataGridTemplateColumn`, items as `object`) built on a `Grid` header + virtualized `CollectionView` (no native handlers). Sorting (single/multi), filtering (menu/row/toolbar), grouping + aggregates, single/multi selection w/ checkboxes, inline editing (cell/form), paging, virtualization, column resize/reorder, sticky header, `ServerData` delegate, density/striped/bordered/hover. See datagrid.md
- **TreeView**: Hierarchical tree with lazy-loaded branches (`ChildrenLoader` for per-node async, `RootLoader` for async root), `ChildrenSelector` for sync data, `HasChildrenSelector`/`CanExpandSelector`/`CanSelectSelector` predicates, configurable `ExpandedIcon`/`CollapsedIcon`/`RetryIcon` (ImageSource on MAUI, RenderFragment slots on Blazor), single/multi selection with two-way `SelectedItem`/`SelectedItems`, events + ICommand mirrors for `ItemSelected`/`ItemExpanded`/`ItemCollapsed`/`LoadFailed`/`ItemDropped`, indent + guide lines, drag/drop reorder with above/below/into drop positions and visual drop indicators (event-only — never mutates your data; native HTML5 drag via JS interop on Blazor for Safari/Firefox support, pan-gesture fallback on Catalyst/AppKit/GTK4), programmatic API (`ExpandAll`/`ExpandAllAsync`/`CollapseAll`/`Expand`/`Collapse`/`Refresh`/`ReloadAsync` with state preservation/`FindNode`), and keyboard navigation on Blazor
- **FloatingPanel + OverlayHost**: A floating panel overlay system (MAUI only). Panels slide from bottom or top with configurable detents, header peek when closed, backdrop dimming, and feedback. Multiple panels coexist without blocking touches. Use with `OverlayHost` (manual Grid setup) or `ShinyContentPage` (convenience ContentPage with built-in overlay). Blazor uses `SheetView` with CSS-based overlays instead
- **PillView**: A status badge/label control with 6 preset themes, custom colors, and WCAG-accessible contrast
- **BadgeView**: A content-wrapping overlay that pins a small badge to one of the four corners (`TopLeft`/`TopRight`/`BottomLeft`/`BottomRight`) of a wrapped view. Setting `Text` to an empty string auto-hides the badge — bind your unread/count value directly. Supports configurable `BadgeColor`/`BadgeTextColor`/`BadgeBorderColor`/`BadgeBorderThickness`, `IsDot` mode for simple notification indicators, `MaxCount` numeric overflow rendering ("99+"), per-corner `OffsetX`/`OffsetY` nudge (default hangs the badge slightly outside the corner), scale-in/out animation (`IsAnimated`), and optional continuous `IsPulsing` to draw attention. Blazor honors `prefers-reduced-motion`
- **ImageViewer**: A full-screen image overlay with pinch-to-zoom, pan when zoomed, double-tap to toggle zoom, animated open/close, and a close button
- **ImageEditor**: An inline image editor with cropping (drag-handle selection with dimmed overlay), rotation, freehand drawing with color, text annotations, undo/redo, reset, and export to PNG/JPEG/WEBP at configurable resolutions
- **MediaPickerButton**: A button that adds photos from the gallery and/or camera (built-in `MediaPicker` on MAUI; `<input type=file>`/`capture` on Blazor), compresses/re-encodes each to PNG or JPEG at a chosen quality (with optional max-dimension downscale), caps the count with `MaxPhotos` (added one at a time), and shows the collected photos inline as a tappable carousel (`ShowAsCarouselInView`, opening the ImageViewer with an optional Edit button that reuses the ImageEditor) or a compact pinch/zoom overlay. `AllowGallery`/`AllowCamera`/`AllowPhotoEdit` toggles, `PermissionDeniedText`, `NoImagesTemplate`, and a two-way `Photos` collection of `MediaPickerItem`. See media-picker-button.md
- **ChatView**: A modern chat UI with message bubbles, per-participant colors and avatars, visual grouping by sender/minute, typing indicators, virtualized message list with load-more, auto-link detection, image messages, and a bottom input bar with send/attach
- **SecurityPin**: A PIN/OTP entry control with individual cells, configurable length, keyboard, and optional character masking
- **Fab**: A Material-style floating action button with Icon, Text, Command, custom colors, border, and shadow
- **FabMenu**: A floating action menu with an expanding, animated child `FabMenuItem` stack and two-way `IsOpen`
- **SchedulerCalendarView**: Monthly calendar grid with swipe navigation, event display, and pinch-to-zoom
- **SchedulerAgendaView**: Day/multi-day timeline (`DaysToShow` 1–7) with overlap detection, switchable date picker modes (`DatePickerMode`: Carousel / Calendar / None), additional timezone columns, auto-updating current time marker, and 12/24-hour time — full feature parity on MAUI and Blazor
- **SchedulerCalendarListView**: Vertically scrolling event list grouped by day with infinite scroll and sticky day headers (`StickyDayHeaders`, default true, pins the current day header while scrolling)
- **MarkdownView**: A read-only markdown renderer that converts markdown text to native MAUI controls with theming and link handling
- **MarkdownEditor**: A markdown editor with formatting toolbar, live preview toggle, and customizable toolbar items
- **BarcodeView / QRCodeView** (separate `Shiny.Maui.Controls.Barcodes` / `Shiny.Blazor.Controls.Barcodes` packages): Pure-managed barcode rendering powered by ZXing.Net. Supports 13 symbologies (`QRCode`, `Aztec`, `DataMatrix`, `Pdf417`, `Code128`, `Code39`, `Code93`, `Codabar`, `Ean8`, `Ean13`, `UpcA`, `UpcE`, `Itf`). MAUI renders to PNG via a built-in pure-managed encoder (no SkiaSharp / `System.Drawing` dependency, AOT-safe) and feeds an `Image`. Blazor renders inline SVG by default (crisp at any size, single-path output with `shape-rendering="crispEdges"`) or a PNG `data:` URI. `QRCodeView` is a `BarcodeView` subclass that locks `Format = QRCode` and adds `Size` (square edge length) and `ErrorCorrection` (`Low`/`Medium`/`Quartile`/`High`). The static `BarcodeRenderer` exposes `RenderPng`, `RenderSvg`, and `RenderDataUri` for raw output without a view. XAML namespace `xmlns:bc="http://shiny.net/maui/barcodes"`
- **AutoCompleteEntry**: A text input with debounced search, dropdown suggestions, busy indicator, custom item templates, and full styling control via CSS custom properties (Blazor) or bindable properties (MAUI)
- **CountryPicker**: A country search control built on AutoCompleteEntry with flag emoji, country name, and dial code
- **AddressEntry**: An address search control built on AutoCompleteEntry with geocoding (Nominatim/OpenStreetMap by default) and structured address results
- **SignaturePad**: A signature capture control that opens in a FloatingPanel (MAUI) or SheetView (Blazor). Users draw on a canvas and export to PNG. Configurable stroke color/width, background, export dimensions, sign/cancel buttons, and panel styling. Like FloatingPanel, it must be placed inside an `OverlayHost` or `ShinyContentPage` (MAUI). The Sign button is disabled until the user draws something
- **Toast**: A service-first toast notification system invoked via DI-injected `IToaster` (registered by `UseShinyControls()`). Supports auto-dismiss with configurable duration, manual dismiss via `IDisposable`, pill or fill-horizontal display modes, top/bottom positioning, queue or stack mode for multiple toasts, indeterminate spinner, countdown progress bar, icon, tap command, feedback, and screen reader announcement. No XAML or OverlayHost required — the overlay auto-attaches to the current page. Blazor uses `IToastService` with `<ToastHost>` component
- **Dialogs** (MAUI + Blazor): A service-first dialog system that emulates `alert`/`confirm`/`prompt` with owned (non-native), animated, themeable dialogs. Inject `IDialogService` and await `Alert` (Task), `Confirm` (Task<bool>), or `Prompt` (Task<PromptResult>). Queued/modal, backdrop cancel (Escape/Enter on Blazor), theme-token colors. Per-call `configure` sets the `DialogAnimation` (None/Fade/SlideTop/SlideBottom/SlideLeft/SlideRight/Zoom/Pop) and styling; customize globally via `ConfigureDialogs` (MAUI) / `AddShinyDialogs(o => ...)` (Blazor) or fully replace the card with a `ContentTemplate` (MAUI `DataTemplate`) / `<DialogHost Template>` (Blazor `RenderFragment<DialogContext>`). MAUI auto-attaches (just `UseShinyControls()`); Blazor needs `AddShinyDialogs()` + a single `<DialogHost>`
- **TextEntry**: A Material Design-inspired text entry control with animated floating placeholder, customizable border, left/right tool slots, hint text for validation, character count, read-only/password modes, and reusable tools (ClearButtonTool, TextEntrySpeechToTextTool)
- **Slider**: A slider control with a two-color gradient track, blended thumb border that samples the gradient at the current position, tooltip with custom templates, and full drag/tap interaction
- **RangeSlider**: A two-thumb variant of Slider selecting a lower/upper value pair (`LowerValue`/`UpperValue`). Reuses the gradient (shown across the active segment between thumbs), blended thumb borders, and per-thumb tooltips, and adds `MinimumRange` (hard-stop gap) and `MaximumRange` (pushes the other thumb) constraints. See range-slider.md
- **ProgressBar**: A progress bar with gradient fill and a Vista-style shimmer pulse that sweeps left-to-right. Configurable `PulseLength` (width of sheen) and `PulseSpeed` (sweep duration). Triggers on value change or timed interval. Supports indeterminate mode and text overlay
- **Overlay & LoadingOverlay**: Full-screen overlay with configurable backdrop color and opacity, fade animation, and custom content via `DataTemplate` (MAUI) or `RenderFragment` (Blazor). `LoadingOverlay` extends it with built-in spinner (indeterminate) or progress bar (determinate) plus optional message text
- **SkeletonView**: A content-wrapping control (similar to `RefreshView`) that shows animated shimmer placeholders while `IsBusy` is true, then reveals the real content when loading finishes. Built-in line placeholders (configurable `ItemCount`/`ItemHeight`/`ItemSpacing`/`CornerRadius`/`BaseColor`/`ShimmerColor`) or a custom placeholder layout via `SkeletonTemplate` (MAUI) / `SkeletonContent` (Blazor). Shimmer is a sweeping `LinearGradientBrush` band on MAUI and an animated CSS gradient (honoring `prefers-reduced-motion`) on Blazor. Use it for inline content regions; use `LoadingOverlay` for whole-page loading
- **CarouselGallery**: Netflix-style horizontal carousel with snap-to-center, configurable scale transforms (FocusedItemScale/UnfocusedItemScale), peek area insets, infinite loop, two-way position tracking, and `SnapCount` (0=free scroll, 1+=snap to item). Uses native recycler views on MAUI and CSS scroll-snap on Blazor
- **Carousel** (Blazor only, `Carousel<TItem>`): An Embla-style, transform-based drag carousel. A pointer-drag engine (mouse click-drag + touch flick with momentum) translates the track — no native scroll-snap. Parameters: `Items`/`ItemTemplate` (+`PlaceholderTemplate`, `EmptyTemplate`, `ThumbnailTemplate`); layout `Align` (Start/Center/End), `SlidesPerView` (fill-to-fit sizing), `SlidesToScroll`, `VariableWidths` (content-sized slides), fixed `ItemWidth`/`ItemHeight`/`ItemSpacing`, `Orientation` (Horizontal/Vertical) with `ViewportHeight`, `Rtl`; interaction `Draggable`, `DragFree` (momentum, no snap), `Loop`; auto-motion `AutoPlay`+`AutoPlayInterval`, `AutoScroll`+`AutoScrollSpeed` (continuous marquee), `PauseOnHover`; scroll-linked `Effect` (None/Scale/Opacity/Parallax/Fade) tuned by `FocusedItemScale`/`UnfocusedItemScale`/`ParallaxFactor`/`MinOpacity`; chrome `ShowArrows`/`ShowIndicators` (dots = snap count)/`ShowCounter` ("n / total")/`ShowProgress` (scroll-position bar)/`ShowThumbnails`; `LazyLoad`+`LazyLoadBuffer`. Two-way `CurrentPosition` (selected snap index), `CurrentPositionChanged`, `ItemSelected`. Methods `NextAsync`/`PreviousAsync`/`GoToAsync(snap)`/`GoToSlideAsync(item)`. Keyboard arrows/Home/End. Effects animate live during drag. Use `CarouselGallery` for MAUI/Blazor parity; use `Carousel` for the richer Blazor-only drag experience
- **ParallaxCollectionView** (MAUI) / **ParallaxList** (Blazor): A scrollable list with a hero header that translates at a configurable fraction of the scroll offset (`ParallaxFactor`, default 0.5 = half speed). Optional `CollapseToSticky` clamps the header to a `MinHeaderHeight` minimum, and `FadeHeaderOnScroll` fades it out as it scrolls. MAUI wraps a real `CollectionView` (`ItemTemplate`, `EmptyView`, `SelectionMode`, `SelectedItem`, `ScrollTo`, custom `ItemsLayout`) in a `Grid` and drives the hero translation from `CollectionView.Scrolled`. Blazor uses a CSS-positioned hero plus a tiny JS scroll listener that mutates `transform`/`opacity` directly (rAF-throttled) so parallax runs at native scroll framerate without Razor re-renders. Both hosts fire a `Scrolled` event with `ParallaxScrollEventArgs(verticalOffset, headerTranslation, headerVisibleHeight)` for driving sticky titles, fading nav chrome, etc. No platform handlers
- **StaggeredGrid**: Pinterest-style masonry/waterfall layout with variable-height items in configurable columns. Items with HeightRequest on the root template view use that value directly for measurement. Uses native staggered layout managers on MAUI and CSS column-count on Blazor
- **VirtualizedGrid**: Full-featured grouped grid with sticky section headers, virtualization, orientation-aware column counts, cell padding, load-more button (renders as footer at end of data) with custom template support, and item visibility tracking. Uses native grid layouts on MAUI and CSS Grid on Blazor
- **Desktop (Tray Icon + Docking)**: A single desktop-only add-on package `Shiny.Maui.Controls.Desktop` bundles two features that share the desktop TFM matrix (Windows + macOS AppKit + MacCatalyst + Linux):
  - **Tray Icon** (`using Shiny.Maui.Controls.Desktop.TrayIcon;`) — cross-platform system tray / status-bar icon. Windows (`Shell_NotifyIcon`), macOS AppKit (`NSStatusItem`, native `net10.0-macos` build), MacCatalyst (AppKit bridged via the Objective-C runtime), Linux (`libayatana-appindicator3` + GTK 3 — requires the system library installed). API: `ITrayIconFactory` resolved from DI, then `ITrayIcon` with `SetIcon(Func<Stream>)` (PNG or ICO bytes — Windows auto-wraps PNG into ICO), `Tooltip`, `Title` (macOS/Linux label, ignored on Windows), `IsVisible`, `IsTemplateImage` (macOS auto-tint), `SetMenu(TrayMenu)`, `ShowMenu()`, and `PrimaryClick`/`SecondaryClick`/`DoubleClick` events. Menus are built fluently with `TrayMenu.Build(b => b.Item(...).Check(...).Separator().Submenu(...))` — `TrayMenuItem`, `TrayCheckMenuItem`, `TraySeparator`, and `TraySubmenu`. Mutating any item's `Label`/`IsEnabled`/`IsVisible` rebuilds the native menu automatically. No Blazor equivalent — tray icons are a desktop OS concept. Registered with `.UseTrayIcon()` in `MauiProgram.cs`
  - **Docking** (`using Shiny.Maui.Controls.Desktop.Docking;`) — Visual-Studio-style window docking for MAUI desktop, with a companion `Shiny.Blazor.Controls.Kiosk` package for Blazor (kiosk-shaped Blazor features — docking + on-screen keyboard; namespace `Shiny.Blazor.Controls.Kiosk.Docking`). `DockHostView` attaches to any existing `ContentPage` (not a `ContentPage` subclass) and orchestrates `DockGroupView`, `DockTabStrip`, and `DockSplitter` building blocks. Public surface includes `IDockHost` (per-window controller — `LoadAsync`, `Snapshot`, `ShowPanelAsync`/`HidePanelAsync`/`ActivatePanelAsync`, `ResetLayoutAsync`, `SetRailCollapsedAsync`, `IsLocked`, `Events`; implemented directly by `DockHostView` / `<DockHost>`), `IDockableContent` (optional interface on panel views — per-instance `Title`/`Icon`, `CanClose`/`CanFloat`, `OnActivated`/`OnDeactivated`, `WantsPointerDown` for embedded editors), `IDockableContentFactory` (async `Task<View> CreateAsync(string instanceId, ...)` + `DisplayName`/`Icon`, registered with `.AddDockPanel<TView>("panel-id", displayName: …, icon: …)`), `IDockLayoutStore` (bring-your-own persistence — no default ships; attach via the host's `LayoutStore` property for auto-load at startup + debounced auto-save), `IDockLayoutMigrator` (forward-only schema migrations), `IDockEvents` (`LayoutChanged`, `PanelActivated`, `DragStarted/Completed/Cancelled`), and `IDockCommandScope` (scopes Ctrl+W, Ctrl+Tab MRU, Ctrl+Alt+PgUp/Dn to the dock surface). The layout schema is a pure POCO tree (`DockRoot`, `DockWindowState`, `DockSplit`, `DockGroup`, `DockTab`, `DockEmpty`, `DockCollapsedPanel`) with a source-generated `System.Text.Json` context and `SchemaVersion` + `MinReadableVersion` for migration; `DockSerialization.Serialize`/`Deserialize` round-trip layouts to JSON. Fully interactive: tab drag to merge (drop center) / split (drop edge) / reorder (drop in tab strip) / tear off floating windows (drop outside the host — movable, resizable, re-dockable), draggable splitters with persisted clamped ratios, per-panel collapse to slim edge bars (restore on click; whole rails via `SetRailCollapsedAsync`), locked/read-only mode, and persisted floating-window bounds + collapsed state. Registered with `.UseShinyDocking()` + `.AddDockPanel<TView>("id")` on MAUI, `services.AddShinyDocking() + .AddDockPanel<TComponent>("id")` on Blazor; host controls accept `InitialLayout`, `LayoutStore`, and `IsLocked`
  - **On-Screen Keyboard** (`using Shiny.Maui.Controls.Desktop.OnScreenKeyboard;` / `using Shiny.Blazor.Controls.Kiosk.OnScreenKeyboard;`) — Touch / kiosk soft keyboard. US-QWERTY with three layers (lowercase / Shift / 123-symbols), bottom-docked, auto-shows when an `Entry` / `Editor` (MAUI) or `<input>` / `<textarea>` (Blazor) gains focus. Critically does NOT steal focus when keys are tapped — every key uses `pointerdown` + `preventDefault()` (Blazor) or `Focusable = false` + intercepted `PointerPressed` (MAUI). Press-and-hold autorepeat (400ms delay, 50ms interval, configurable). Dispatches via managed `Text` mutation at `CursorPosition` (MAUI) or `document.execCommand('insertText', char)` after focus restore (Blazor) — no native synthetic key events in v0.1, so no macOS Accessibility entitlement and Mac App Store distribution works. Full AutomationPeer (Windows / MAUI / GTK) and ARIA `role="button"` + `aria-keyshortcuts` (Blazor) for switch-input accessibility. Theme tokens mirror docking — `OnScreenKeyboardKeyBrush` / `--shiny-osk-key-bg` etc. Public surface: `IOnScreenKeyboard` (MAUI) / `IOnScreenKeyboardService` (Blazor) with `Show()` / `Hide()` / `Toggle()` / `IsVisible` / `VisibilityChanged`, plus `OnScreenKeyboardOptions` for auto-show-on-focus, push-content vs overlay, height, theme, and autorepeat timing. Register with `.UseOnScreenKeyboard(opts => ...)` on MAUI or `services.AddShinyOnScreenKeyboard(opts => ...)` + place `<OnScreenKeyboardHost />` once in `MainLayout.razor` on Blazor. v0.1 is planned but not yet implemented; limitations: MAUI inputs / DOM inputs only (no system-wide injection — opt-in pluggable `IKeyDispatcher` arrives in v0.4), no Shadow DOM, no IME / dead-key composition (v0.5), no language switching (v0.3 via JSON layout files)
- **Feedback Service**: All interactive controls fire events through `IFeedbackService`. Default `HapticFeedbackService` provides tactile feedback. Replace with `SetCustomFeedback<T>()` in `UseShinyControls()` for TTS, sounds, analytics, or custom responses. The `control` parameter is the actual control instance (use pattern matching like `control is ChatView`), and `args` carries context — `ChatMessage` for ChatView events, native `EventArgs` for standard MAUI controls. Standard MAUI control integration is pluggable and AOT-compatible via `MauiControlFeedbackBuilder` — use `AddDefaultMauiControlFeedback()` for all built-in hooks, add custom hooks with `Hook<TControl>(eventName, subscribe, unsubscribe)`, or use `AddMauiControlFeedback()` for only the hooks you configure

## When to Use This Skill

Invoke this skill when the user wants to:
- Create a settings page or preferences UI in .NET MAUI
- Add or configure TableView cells (switch, checkbox, entry, picker, command, etc.)
- Style a TableView with global cascading styles or per-cell overrides
- Use sections with headers, footers, and dynamic ItemTemplate cells
- Enable drag-to-reorder within a section
- Bind cell properties to a ViewModel using MVVM
- Create radio button groups, date/time pickers, number pickers, or multi-select pickers
- Build any form-like or list-based settings UI
- Build a tree/hierarchical view (file browser, folder picker, org chart, category tree)
- Lazy-load tree branches from a remote source on first expand
- Show a tree with single or multi-select and per-item `CanSelect`/`CanExpand` predicates
- Customize the tree's expand/collapse icons (font icons, custom images, render fragments)
- Enable drag-and-drop reordering within a tree
- Add a bottom sheet / sliding panel / floating panel to a page
- Show status badges, tags, or labels (pill views)
- Display categorized status indicators (success, warning, critical, etc.)
- Overlay a notification badge (count, dot, or label) on the corner of an icon, avatar, button, or card (BadgeView)
- Show an unread count or "99+" overflow indicator on inbox/cart/profile UI elements
- Display a pulsing "NEW"/"!" attention badge on a feature or menu item
- Add a zoomable image viewer / photo viewer overlay
- Display full-screen images with pinch-to-zoom, pan, and double-tap zoom
- Edit images with crop, rotate, draw, or text annotations
- Build an image editor with undo/redo and export
- Build a chat or messaging UI with bubbles, typing indicators, and message history
- Create a conversational interface with load-more pagination and auto-scroll
- Build a PIN entry / OTP / passcode input screen
- Capture numeric or alphanumeric codes in individual cells with optional masking
- Add a floating action button (FAB) to a page, or a speed-dial style multi-action menu
- Expose primary/contextual actions in the bottom corner with animated reveal
- Create scheduler/calendar views (monthly grid, day/week agenda, event list)
- Implement event providers for calendar data
- Customize event templates or day header templates for scheduler views
- Configure agenda timeline with overlap detection, timezone support, and time markers
- Set up infinite scrolling event lists grouped by day
- Build any calendar, appointment, or scheduling UI
- Render markdown text as native MAUI controls
- Build a markdown editor with formatting toolbar and live preview
- Display documentation, notes, or rich text content from markdown strings
- Render a QR code for a URL, vCard, Wi-Fi join code, or pairing token
- Render a 1D barcode (EAN-13, UPC-A, Code 128, etc.) for retail / shipping / IDs
- Render Aztec, Data Matrix, PDF417 codes for transit tickets, electronics labels, or driver's licenses
- Generate barcode PNG bytes, SVG markup, or `data:` URIs from code without a view (e.g., for PDF export, file save, email attachment)
- Pick a QR error-correction level (Low / Medium / Quartile / High) for printed labels or scuff-prone surfaces
- Configure colors, quiet-zone margin, or output size on a barcode / QR code view
- Add a search/autocomplete text input with dropdown suggestions
- Build a typeahead or search-as-you-type control with debounce
- Add a country picker or country selector with flag display
- Build an address search/lookup field with geocoding
- Implement a custom search provider for address or location queries
- Capture a signature or e-signature from the user
- Add a signature pad / drawing pad to a page
- Export a captured signature as a PNG image
- Show toast notifications, snackbar messages, or transient alerts from code
- Show an alert / confirm / prompt dialog from code on MAUI or Blazor (await a result via `IDialogService`), with custom in/out animations
- Build a data grid / data table with sortable, filterable, groupable, editable columns, paging, and selection (MAUI or Blazor) — MudBlazor-style
- Display progress/loading toasts with spinner while operations complete
- Queue or stack multiple notifications
- Replace haptic feedback with custom feedback (text-to-speech, sounds, analytics)
- Wire up IFeedbackService for control interaction events
- Enable text-to-speech on incoming chat messages via feedback service
- Create a text entry field with floating placeholder
- Add validation hints and error states to text inputs
- Build a text input with clear button, character count, or custom tools
- Create a form with styled text entry fields
- Add a gradient slider / temperature slider / range control
- Show a progress bar with gradient fill or shimmer animation
- Build a loading indicator with Vista-style pulse sweep
- Display determinate or indeterminate progress with configurable pulse
- Add a full-screen overlay / loading overlay to a page
- Show a busy/loading indicator over content (spinner or progress bar)
- Create a custom overlay with configurable color and content
- Build a horizontal carousel with snap-to-center or free-scroll behavior
- Create a Netflix-style browsing gallery
- Build a scrollable list with a parallax/hero header (App-Store-style header that translates as you scroll)
- Build a collapsing header that pins to a minimum height once scrolled
- Build a Pinterest-style masonry/waterfall grid with variable-height items
- Create a virtualized grid with grouping, sticky headers, and load-more
- Add load-more pagination (threshold or button) to a collection view
- Add a system tray / status-bar / menu-bar icon to a MAUI desktop app (Windows, macOS, MacCatalyst, Linux)
- Show a right-click context menu with submenus and checkmark items on a tray icon
- Handle left-click / right-click / double-click on a tray icon
- Build a "menu bar app" on macOS or a tray-resident background app on Windows
- Update tray menu items dynamically (e.g. "Pause" ↔ "Resume" labels, enable/disable states)
- Show or hide a tray icon at runtime without recreating it
- Make a tray icon auto-adapt to the macOS light/dark menu bar via template images
- Use Linux AppIndicator (`libayatana-appindicator`) for tray icons on GNOME/KDE
- Build a Visual-Studio-style dockable UI with tool windows, tabbed groups, splitters, auto-hide rails, and tear-off floating windows in a MAUI desktop app
- Add a docking host (`DockHostView` for MAUI / `<DockHost />` for Blazor) to an existing page without subclassing ContentPage
- Register dockable panels by stable string ID (`AddDockPanel<TView>("solution-explorer")`) so layout JSON can resolve them back to actual views
- Persist a dock layout to disk and reload it on app start, with schema versioning and forward migrations via `IDockLayoutMigrator`
- Implement a bring-your-own `IDockLayoutStore` (e.g. backed by Shiny.Stores, a file, or a remote service) to save/load `DockRoot` snapshots
- Observe dock layout/drag/activation events through `IDockEvents` for telemetry or undo-stack integration
- Lock the dock layout for read-only / kiosk modes via `IDockHost.IsLocked = true`
- Add a **touch / kiosk on-screen keyboard** to a MAUI desktop or Blazor app
- Show a **soft keyboard** that auto-appears when an `Entry`/`Editor`/`<input>`/`<textarea>` gains focus and types into it without stealing focus
- Build a bottom-docked QWERTY keyboard with shift / numbers / symbols layers
- Drive the on-screen keyboard's visibility from code via `IOnScreenKeyboard.Show()/Hide()/Toggle()`
- Ship a kiosk app on a touch tablet without relying on the OS on-screen keyboard
- Build a switch-input-accessible on-screen keyboard (full AutomationPeer / ARIA tree)
- Add an OSK that pushes page content up vs overlays above it (`PushContent` option)
- Distinguish from the OS on-screen keyboard (`osk.exe` / TabTip) — Shiny's OSK is an in-app control, not a wrapper around the OS one

## Library Overview

### .NET MAUI

**NuGet**: `Shiny.Maui.Controls` (+ `Shiny.Maui.Controls.Markdown`, `Shiny.Maui.Controls.MermaidDiagrams`, `Shiny.Maui.Controls.Barcodes`, `Shiny.Maui.Controls.Desktop` for tray icon + docking)
**Namespace**: `Shiny.Maui.Controls`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)
**Desktop add-on namespaces**: `Shiny.Maui.Controls.Desktop.TrayIcon`, `Shiny.Maui.Controls.Desktop.Docking`, `Shiny.Maui.Controls.Desktop.OnScreenKeyboard` (extension methods `UseTrayIcon`, `UseShinyDocking`, `AddDockPanel<T>`, `UseOnScreenKeyboard` live in `Shiny`)

### Blazor

**NuGet**: `Shiny.Blazor.Controls` (+ `Shiny.Blazor.Controls.Markdown`, `Shiny.Blazor.Controls.MermaidDiagrams`, `Shiny.Blazor.Controls.Barcodes`, `Shiny.Blazor.Controls.Kiosk` for the Blazor docking host + on-screen keyboard)
**Namespaces**: `Shiny.Blazor.Controls`, `Shiny.Blazor.Controls.Cells`, `Shiny.Blazor.Controls.Sections`, `Shiny.Blazor.Controls.Scheduler`, `Shiny.Blazor.Controls.Chat`, `Shiny.Blazor.Controls.Markdown`, `Shiny.Blazor.Controls.MermaidDiagrams`, `Shiny.Blazor.Controls.Kiosk.Docking`, `Shiny.Blazor.Controls.Kiosk.OnScreenKeyboard`

## Setup

### .NET MAUI

1. Install the NuGet package
   ```bash
   dotnet add package Shiny.Maui.Controls
   ```

2. Configure in `MauiProgram.cs`
   ```csharp
   using Shiny;

   var builder = MauiApp.CreateBuilder();
   builder
       .UseMauiApp<App>()
       .UseShinyControls();
   ```

3. Add the XAML namespace to your pages
   ```xml
   xmlns:shiny="http://shiny.net/maui/controls"
   ```

### Blazor

1. Install the NuGet package
   ```bash
   dotnet add package Shiny.Blazor.Controls
   ```

2. Add `@using` directives (typically in `_Imports.razor`)
   ```razor
   @using Shiny.Blazor.Controls
   @using Shiny.Blazor.Controls.Cells
   @using Shiny.Blazor.Controls.Sections
   @using Shiny.Blazor.Controls.Scheduler
   @using Shiny.Blazor.Controls.Chat
   @using Shiny.Blazor.Controls.Markdown
   @using Shiny.Blazor.Controls.MermaidDiagrams
   ```

No DI registration is required for Blazor — components are used directly in `.razor` pages.

## MAUI → Blazor Translation Cheat Sheet

All controls exist on both hosts, but the Blazor surface is idiomatic Razor, not a 1:1 XAML port. When generating Blazor code, translate with these rules:

### Component name differences

| MAUI (XAML)             | Blazor (Razor)    | Notes                                           |
|-------------------------|-------------------|-------------------------------------------------|
| `shiny:TableView`       | `<TableView>`     | No prefix on Blazor; `TableRoot` is not needed  |
| `shiny:TableRoot`       | *(omitted)*       | Sections go directly inside `<TableView>`       |
| `shiny:TreeView`        | `<TreeView TItem="…">` | Strongly typed on Blazor; `ExpandedIcon`/`CollapsedIcon`/`RetryIcon` are `RenderFragment` slots, not `ImageSource`; Blazor adds keyboard navigation (↑/↓/←/→/Enter/Home/End) and a `<LoadingTemplate>` slot for the root-load spinner |
| `shiny:TableSection`    | `<TableSection>`  |                                                 |
| `shiny:PillView`        | `<Pill>`          | Renamed to just `Pill` on Blazor                |
| `shiny:BadgeView`       | `<BadgeView>`     | Wraps `Content` (MAUI) / `ChildContent` (Blazor). Colors are CSS strings on Blazor; `Position` is the `BadgePosition` enum on both hosts; empty `Text` auto-hides unless `IsDot=true` |
| `shiny:FloatingPanel` in `shiny:OverlayHost` | `<SheetView>` | MAUI uses FloatingPanel+OverlayHost; Blazor uses SheetView with CSS overlay. Content goes in `<SheetContent>` named slot on Blazor |
| `shiny:Fab`             | `<Fab>`           | `Icon` takes inline SVG/text string, not `ImageSource` |
| `shiny:FabMenu`         | `<FabMenu>`       | Items passed via `Items` parameter (List<FabMenuItem>), not as children |
| `shiny:ImageViewer`     | `<ImageViewer>`   | `Source` is a URL string                        |
| `shiny:ImageEditor`     | `<ImageEditor>`   | `Source` is `byte[]` (MAUI) or URL string + `ImageData` byte[] (Blazor); colors are CSS strings on Blazor |
| `shiny:ChatView`        | `<ChatView>`      | Colors are CSS strings on Blazor; `SendCommand` is `EventCallback<string>` on Blazor; uses `@using Shiny.Blazor.Controls.Chat` |
| `shiny:SecurityPin`     | `<SecurityPin>`   |                                                 |
| `md:MarkdownView`       | `<MarkdownView>`  |                                                 |
| `md:MarkdownEditor`     | `<MarkdownEditor>`|                                                 |
| `diagram:MermaidDiagramControl` | `<MermaidDiagramControl>` |                                     |
| `shiny:AutoCompleteEntry` | `<AutoCompleteEntry>` | Colors are CSS strings; `SearchCommand` is `EventCallback<string>`; supports `CssClass`, `InputClass`, `DropDownClass`, and `AdditionalAttributes` on Blazor |
| `shiny:CountryPicker`  | `<CountryPicker>` | Colors are CSS strings on Blazor |
| `shiny:AddressEntry`   | `<AddressEntry>`  | Colors are CSS strings on Blazor; uses `IAddressSearchProvider` on both hosts |
| Scheduler views        | `<SchedulerCalendarView>`, `<SchedulerAgendaView>`, `<SchedulerCalendarListView>` | Same names |
| `IToaster.ShowAsync(text, cfg => {})` | `IToastService.ShowAsync(text, cfg => {})` | MAUI uses DI-injected `IToaster` (registered by `UseShinyControls()`); Blazor uses DI-injected `IToastService`. Blazor requires `AddShinyToast()` in DI and `<ToastHost />` in layout |
| `IDialogService.Confirm(...)` (DI) | `IDialogService.Confirm(...)` (DI) | Same interface on both hosts. MAUI auto-attaches via `UseShinyControls()`; Blazor needs `AddShinyDialogs()` + `<DialogHost />`. Colors are `Color` on MAUI, CSS strings on Blazor |
| `shiny:DataGrid` + `shiny:DataGridColumn PropertyName="Name"` (items as `object`) | `<DataGrid TItem="T">` + `<PropertyColumn Property="x => x.Name"/>` (generic; `RenderFragment` cell templates) | MAUI columns nest directly; Blazor columns go in `<Columns>`. Paging: MAUI `PageSize`, Blazor `<PagerContent><DataGridPager/></PagerContent>`. Reorder: MAUI `AllowColumnReorder` (arrows), Blazor `DragDropColumnReordering` (drag) |
| `shiny:TextEntry` | `<TextEntry>` | Colors are CSS strings on Blazor; tools are `List<TextEntryTool>` on both hosts. MAUI uses `ICommand`, Blazor uses `Action` callback. Blazor `TextEntryTool` has `Icon` as string (not ImageSource) |

### Binding, events, content

| MAUI                                                 | Blazor                                           |
|------------------------------------------------------|--------------------------------------------------|
| `IsOpen="{Binding IsOpen, Mode=TwoWay}"`             | `@bind-IsOpen="isOpen"`                          |
| `Value="{Binding Pin, Mode=TwoWay}"`                 | `@bind-Value="pin"`                              |
| `Command="{Binding AddCommand}"`                     | `Clicked="OnAdd"` / `OnClick="OnAdd"` (event callback) |
| `FontAttributes="Bold"` (PillView)                   | `Bold="true"`                                    |
| `Color="DodgerBlue"` (MAUI `Color`)                  | `Color="#1E90FF"` (CSS color strings)            |
| `ItemsSource` + `ItemTemplate` (DataTemplate)        | `ItemsSource` + `ItemTemplate` (`RenderFragment<object>`) |
| `<shiny:FloatingPanel>` content is `[ContentProperty]` PanelContent | `<SheetContent>…</SheetContent>` named slot (Blazor SheetView)    |
| `<shiny:FabMenu><shiny:FabMenuItem /></shiny:FabMenu>` | `Items="List<FabMenuItem>"` parameter         |

### Blazor-specific notes

- Use **CSS color strings** (`"#RRGGBB"`, `"rgb(...)"`, named colors) — there is no MAUI `Color` type on Blazor
- `Icon` on `Fab`/`FabMenuItem` is a string — pass an inline SVG, emoji, or single character
- `RenderFragment<object>` is the Blazor equivalent of `DataTemplate` for `ItemsSource`/`ItemTemplate`
- Event handlers take the event arg directly (e.g. `Completed="OnCompleted"` where `OnCompleted(SecurityPinCompletedEventArgs e)`), not ICommand
- Scheduler still uses `ISchedulerEventProvider` — the same interface and models work on both hosts

### Blazor quick examples

**TableView**
```razor
<TableView CellAccentColor="#10B981">
    <TableSection Title="Profile">
        <LabelCell Title="Name" ValueText="Allan Ritchie" />
        <LabelCell Title="Plan" ValueText="Pro" />
    </TableSection>
    <TableSection Title="Danger zone">
        <ButtonCell Title="Delete account"
                    ButtonTextColor="#DC2626"
                    OnClick="@(() => deleted = true)" />
    </TableSection>
</TableView>
```

**SheetView (Blazor only — MAUI uses FloatingPanel+OverlayHost)**
```razor
<button @onclick="() => isOpen = true">Open Sheet</button>

<SheetView @bind-IsOpen="isOpen"
                 Detents="detents"
                 SheetCornerRadius="20">
    <SheetContent>
        <h2>Hello from a sheet</h2>
        <button @onclick="() => isOpen = false">Close</button>
    </SheetContent>
</SheetView>

@code {
    bool isOpen;
    IList<DetentValue> detents = new List<DetentValue>
    {
        DetentValue.Quarter, DetentValue.Half, DetentValue.Full
    };
}
```

**Pill**
```razor
<Pill Text="Success" Type="PillType.Success" />
<Pill Text="Brand" PillColor="#312E81" PillTextColor="#E0E7FF" />
<Pill Text="Bold" Type="PillType.Info" Bold="true" />
```

**Fab / FabMenu**
```razor
<Fab Icon="+" FabBackgroundColor="#EC4899" Clicked="OnAdd" />

<FabMenu Items="items"
         Icon="+"
         FabBackgroundColor="#7C3AED"
         ItemTapped="OnItemTapped" />

@code {
    readonly List<FabMenuItem> items = new()
    {
        new FabMenuItem { Text = "New Note",  Icon = "📝", FabBackgroundColor = "#10B981", Tag = "note"  },
        new FabMenuItem { Text = "New Photo", Icon = "📷", FabBackgroundColor = "#F59E0B", Tag = "photo" }
    };
    void OnItemTapped(FabMenuItem item) { /* ... */ }
    void OnAdd() { /* ... */ }
}
```

**ImageViewer**
```razor
<img src="@url" @onclick="() => Open(url)" />

<ImageViewer Source="@current" @bind-IsOpen="isOpen" MaxZoom="6" />

@code {
    bool isOpen;
    string? current;
    void Open(string url) { current = url; isOpen = true; }
}
```

**ImageEditor**
```razor
<ImageEditor @ref="editor"
             Source="@imageUrl"
             ImageData="@imageData"
             AllowCrop="true"
             AllowDraw="true"
             AllowRotate="true"
             AllowTextAnnotation="true"
             DrawStrokeColor="#ff0000"
             DrawStrokeWidth="3"
             CanUndoChanged="v => canUndo = v"
             CanRedoChanged="v => canRedo = v" />

@code {
    ImageEditor? editor;
    string? imageUrl = "https://example.com/photo.jpg";
    byte[]? imageData;
    bool canUndo, canRedo;

    async Task Export()
    {
        var bytes = await editor!.ExportAsync("png");
    }
}
```

**ChatView**
```razor
@using Shiny.Blazor.Controls.Chat

<div style="height:600px;">
    <ChatView Messages="messages"
              Participants="participants"
              IsMultiPerson="true"
              TypingParticipants="typingParticipants"
              SendCommand="OnSend"
              AttachImageCommand="OnAttach"
              LoadMoreCommand="OnLoadMore"
              MyBubbleColor="#DCF8C6"
              OtherBubbleColor="#FFFFFF" />
</div>

@code {
    List<ChatMessage> messages = new();
    List<ChatParticipant> participants = new();
    List<ChatParticipant> typingParticipants = new();

    Task OnSend(string text)
    {
        messages.Add(new ChatMessage { Text = text, SenderId = "me", IsFromMe = true });
        StateHasChanged();
        return Task.CompletedTask;
    }

    Task OnAttach() => Task.CompletedTask;
    Task OnLoadMore() => Task.CompletedTask;
}
```

**SecurityPin**
```razor
<SecurityPin @bind-Value="pin"
             Length="6"
             HideCharacter="●"
             Completed="OnCompleted" />

@code {
    string pin = "";
    void OnCompleted(SecurityPinCompletedEventArgs e) { /* verify e.Value */ }
}
```

**Markdown**
```razor
<MarkdownView Markdown="@content" />
<MarkdownEditor @bind-Markdown="content" Placeholder="Write markdown…" />
```

---

# Code Generation Instructions

When generating code with Shiny.Maui.Controls:

### 1. Page Structure
- Always add `xmlns:shiny="http://shiny.net/maui/controls"` to the page
- For Markdown controls: add `xmlns:md="http://shiny.net/maui/markdown"` to the page
- For TableView: wrap content in `shiny:TableView > shiny:TableRoot > shiny:TableSection`
- For FloatingPanel (MAUI): use `shiny:ShinyContentPage` as the page base class with `PageContent` for main content and `Panels` for FloatingPanels. Alternatively, place `shiny:OverlayHost` with `shiny:FloatingPanel` children inside a Grid. Supports `Position="Bottom"` (default), `Position="Top"`, or `Position="BottomTabs"` (for use inside Shell TabBar)
- For SheetView (Blazor only): use `<SheetView>` with `<SheetContent>` child
- For ImageViewer: place `shiny:ImageViewer` inside a Grid that fills the page (it overlays on top, same pattern as SheetView)
- For ImageEditor: use `shiny:ImageEditor` with `Source` bound to `byte[]` image data. Set `AllowX` properties to toggle features. Use `CurrentToolMode` (TwoWay) to control the active tool. Use `CanUndo`/`CanRedo` (OneWayToSource) to observe undo state. Call `ExportAsync()` to save.
- For PillView: use inline within any layout
- For Scheduler views: use `shiny:SchedulerCalendarView`, `shiny:SchedulerAgendaView`, or `shiny:SchedulerCalendarListView` and bind `Provider` to an `ISchedulerEventProvider`
- For MarkdownView: use `md:MarkdownView` anywhere you need to render markdown content
- For MarkdownEditor: use `md:MarkdownEditor` for editable markdown with toolbar and preview

### 2. Cell Selection (TableView)
- Use `SwitchCell` for on/off toggles
- Use `CheckboxCell` for accept/agree checkboxes
- Use `SimpleCheckCell` for selection lists (shows/hides checkmark)
- Use `RadioCell` for mutually exclusive choices within a section
- Use `EntryCell` for text input
- Use `CommandCell` for navigation/action items with disclosure arrow
- Use `ButtonCell` for destructive or primary actions
- Use `LabelCell` for read-only display
- Use `DatePickerCell` / `TimePickerCell` for date/time selection
- Use `TextPickerCell` for dropdown selection from a list
- Use `NumberPickerCell` for numeric input with min/max
- Use `PickerCell` for full-page single or multi-select
- Use `CustomCell` for any custom MAUI view

### 3. Binding Patterns
- Always use `Mode=TwoWay` for editable properties (`On`, `Checked`, `ValueText`, `Date`, `Time`, `Number`, `SelectedIndex`, `SelectedItem`, `SelectedItems`, `IsOpen`, `IsViewerOpen`, `IsPreviewVisible`)
- Use `Mode=TwoWay` for `MarkdownEditor.Markdown` (editor content)
- Use `Mode=OneWay` (default) for display-only properties (`Title`, `Description`, `ValueText` on LabelCell, `Text` on PillView, `Source` on ImageViewer, `Markdown` on MarkdownView)
- Commands use default `Mode=OneWay`
- RadioCell selection binds at section level: `shiny:RadioCell.SelectedValue="{Binding Prop, Mode=TwoWay}"`

### 4. FloatingPanel Placement (MAUI)
Use `ShinyContentPage` for the simplest setup:

```xml
<shiny:ShinyContentPage xmlns:shiny="http://shiny.net/maui/controls">
    <shiny:ShinyContentPage.PageContent>
        <ScrollView>
            <VerticalStackLayout>
                <Button Text="Open Panel" Command="{Binding OpenCommand}" />
            </VerticalStackLayout>
        </ScrollView>
    </shiny:ShinyContentPage.PageContent>
    <shiny:ShinyContentPage.Panels>
        <shiny:FloatingPanel IsOpen="{Binding IsOpen, Mode=TwoWay}">
            <Label Text="Panel content" />
        </shiny:FloatingPanel>
    </shiny:ShinyContentPage.Panels>
</shiny:ShinyContentPage>
```

Or use `OverlayHost` manually in a Grid:

```xml
<ContentPage>
    <Grid>
        <ScrollView><!-- page content --></ScrollView>
        <shiny:OverlayHost>
            <shiny:FloatingPanel IsOpen="{Binding IsOpen, Mode=TwoWay}">
                <Label Text="Panel content" />
            </shiny:FloatingPanel>
        </shiny:OverlayHost>
    </Grid>
</ContentPage>
```

### 5. Dark Mode
- Do NOT hardcode colors. Leave color properties as `null` to inherit system defaults.
- Only set explicit colors when the design requires specific brand colors.
- The controls respect `Application.Current.UserAppTheme` automatically.

### 6. Styling Strategy
- Set global styles on `shiny:TableView` for consistent appearance
- Override at section level for section-specific header/footer styling
- Override at cell level only for individual cell emphasis
- Use `CellAccentColor` for switches, checkboxes, and radio buttons globally

## Complete TableView Example

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:shiny="http://shiny.net/maui/controls"
             x:Class="MyApp.SettingsPage"
             Title="Settings">

    <shiny:TableView CellSelectedColor="#E0E0E0" CellAccentColor="#007AFF">
        <shiny:TableRoot>
            <shiny:TableSection Title="General">
                <shiny:SwitchCell Title="Notifications" On="{Binding NotificationsOn, Mode=TwoWay}" />
                <shiny:SwitchCell Title="Sound" On="{Binding SoundOn, Mode=TwoWay}" />
                <shiny:CheckboxCell Title="Accept Analytics" Checked="{Binding AnalyticsAccepted, Mode=TwoWay}" />
            </shiny:TableSection>

            <shiny:TableSection Title="Account">
                <shiny:EntryCell Title="Name" ValueText="{Binding Name, Mode=TwoWay}" Placeholder="Your name" />
                <shiny:EntryCell Title="Email" ValueText="{Binding Email, Mode=TwoWay}" Keyboard="Email" />
                <shiny:CommandCell Title="Change Password" Command="{Binding ChangePasswordCommand}" />
            </shiny:TableSection>

            <shiny:TableSection Title="Theme" shiny:RadioCell.SelectedValue="{Binding Theme, Mode=TwoWay}">
                <shiny:RadioCell Title="Light" Value="Light" />
                <shiny:RadioCell Title="Dark" Value="Dark" />
                <shiny:RadioCell Title="System" Value="System" />
            </shiny:TableSection>

            <shiny:TableSection Title="Preferences">
                <shiny:DatePickerCell Title="Birthday" Date="{Binding Birthday, Mode=TwoWay}" Format="D" />
                <shiny:TimePickerCell Title="Daily Reminder" Time="{Binding ReminderTime, Mode=TwoWay}" />
                <shiny:NumberPickerCell Title="Font Size" Number="{Binding FontSize, Mode=TwoWay}"
                                      Min="10" Max="36" Unit="pt" />
            </shiny:TableSection>

            <shiny:TableSection Title="About">
                <shiny:LabelCell Title="Version" ValueText="1.0.0" />
                <shiny:CommandCell Title="Privacy Policy" Command="{Binding PrivacyCommand}" />
                <shiny:CommandCell Title="Terms of Service" Command="{Binding TermsCommand}" />
            </shiny:TableSection>

            <shiny:TableSection Title="Actions">
                <shiny:ButtonCell Title="Sign Out" Command="{Binding SignOutCommand}" ButtonTextColor="Red" />
            </shiny:TableSection>
        </shiny:TableRoot>
    </shiny:TableView>
</ContentPage>
```

## Complete FloatingPanel + PillView Example

```xml
<shiny:ShinyContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                         xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                         xmlns:shiny="http://shiny.net/maui/controls"
                         x:Class="MyApp.StatusPage"
                         Title="Status">

    <shiny:ShinyContentPage.PageContent>
        <ScrollView>
            <VerticalStackLayout Padding="20" Spacing="10">
                <Label Text="System Status" FontSize="24" FontAttributes="Bold" />

                <HorizontalStackLayout Spacing="8">
                    <shiny:PillView Text="API" Type="Success" />
                    <shiny:PillView Text="Database" Type="Warning" />
                    <shiny:PillView Text="Queue" Type="Critical" />
                </HorizontalStackLayout>

                <Button Text="View Details" Command="{Binding OpenDetailsCommand}" />
            </VerticalStackLayout>
        </ScrollView>
    </shiny:ShinyContentPage.PageContent>

    <shiny:ShinyContentPage.Panels>
        <shiny:FloatingPanel IsOpen="{Binding IsDetailsOpen, Mode=TwoWay}"
                             PanelCornerRadius="20">
            <VerticalStackLayout Padding="20" Spacing="12">
                <Label Text="Service Details" FontSize="18" FontAttributes="Bold" />

                <HorizontalStackLayout Spacing="6">
                    <shiny:PillView Text="Healthy" Type="Success" />
                    <Label Text="API Server" VerticalOptions="Center" />
                </HorizontalStackLayout>

                <HorizontalStackLayout Spacing="6">
                    <shiny:PillView Text="Degraded" Type="Warning" />
                    <Label Text="Database Cluster" VerticalOptions="Center" />
                </HorizontalStackLayout>

                <HorizontalStackLayout Spacing="6">
                    <shiny:PillView Text="Down" Type="Critical" />
                    <Label Text="Message Queue" VerticalOptions="Center" />
                </HorizontalStackLayout>
            </VerticalStackLayout>
        </shiny:FloatingPanel>
    </shiny:ShinyContentPage.Panels>
</shiny:ShinyContentPage>
```

## Best Practices

1. **Group logically** - Put related settings in the same section with clear headers
2. **Use FooterText** - Explain non-obvious settings in section footers
3. **Two-way bind editable values** - Always `Mode=TwoWay` for user-editable properties
4. **Leave colors null for dark mode** - Only set colors when brand-specific styling is needed
5. **Use CellAccentColor globally** - Set once on TableView instead of per-cell AccentColor
6. **Use CommandCell for navigation** - With `ShowArrow="True"` and `KeepSelectedUntilBack="True"`
7. **Use ButtonCell for destructive actions** - Red text, centered, at the bottom of the page
8. **Use RadioCell for exclusive choices** - Bind `SelectedValue` at the section level
9. **Use PickerCell for long lists** - Full-page picker is better than inline for more than 4-5 items
10. **Use ItemTemplate for dynamic content** - Bind `ItemsSource` on sections for data-driven cells
11. **Use ShinyContentPage or OverlayHost for FloatingPanels** - Use `ShinyContentPage` as the page base class, or place `OverlayHost` in a Grid for overlay panels. Place ImageViewer in a Grid as before
12. **Use PillView for status indicators** - Prefer preset types for consistency; use custom colors for brand-specific needs
13. **Use BadgeView for corner indicators** - Bind `Text` directly to your unread/count source (empty string auto-hides). Use `MaxCount` for numeric overflow ("99+") and `IsDot` for plain "has new" indicators. Reserve `IsPulsing` for genuinely important badges
13. **Use AOT-safe bindings for scheduler templates** - Always use `static (T item) => item.Property` lambda bindings, never string-based
14. **Leave MarkdownView/MarkdownEditor Theme as null** - It auto-resolves Light/Dark based on the app theme
15. **Use MarkdownView for read-only content** - Documentation, notes, changelogs; use MarkdownEditor only when the user needs to edit
16. **ImageViewer Source is set before IsOpen** - Always set the image source before opening the viewer. When Source is null, the viewer is automatically InputTransparent so it won't block touches. Use `OpenViewerOnTap="False"` when controlling the viewer programmatically
