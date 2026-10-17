# NotchView

A Dynamic Island / camera-notch style surface that holds **any** content and morphs its width, height and corner radius to fit it. Both hosts, **core** packages (`Shiny.Maui.Controls`, `Shiny.Blazor.Controls`). The macOS hardware-notch window lives in `Shiny.Maui.Controls.Desktop` and is `net10.0-macos` only.

**Rules for generated code**

- There are **no** compact/expanded states, `IsExpanded`, `CompactContent` or `ExpandedContent`. To "expand", swap `Content` (MAUI) or what `ChildContent` renders (Blazor) for bigger content. To collapse, swap back or set `Content = null` (that is the `MinNotchWidth`×`MinNotchHeight` size).
- Giving wrapping content a fixed `WidthRequest` (e.g. a message stack at 280) is fine and is the usual way to make text wrap inside the notch: the notch measures it at that width.
- Content that changes its own size needs no call; the notch follows it. On MAUI, `Refresh()` only covers changes that don't invalidate measure.
- The default surface is **black on every theme**, so give the content light text (`TextColor="White"`). Don't bind it to a theme surface token unless asked.
- Place it beside the page's scroller (same `Grid` cell) to float it over the page. Never put it inside a `ScrollView` of its own; it never scrolls.
- On Blazor, set `ContentKey` when the content is a different "state" so the new content fades in. A `@switch` on a state string with `ContentKey="@state"` is the idiomatic pattern.
- MAUI tap: `Tapped` event or `Command`/`CommandParameter`. Blazor: `OnClick`, and inner buttons need `@onclick:stopPropagation` if they should not count as a notch click.
- `Shape="Notch"` squares the top corners (flush with the top edge); `Island` rounds every corner. Default is `Island` on MAUI, `Notch` on Blazor.
- **Blazor overlays the top of the screen by default.** It opens in the browser top layer (manual popover), flush with the viewport's top edge, wherever the component is placed — so put it once in a page or layout, don't wrap it in a positioned container, and don't add your own `position: fixed`/`z-index`. Use `Inline="true"` only for a notch shown inside the page flow (a card, a device mock-up).

## MAUI

```xml
<Grid>
    <ScrollView>
        <VerticalStackLayout Padding="16,120,16,16"><!-- page --></VerticalStackLayout>
    </ScrollView>

    <shiny:NotchView x:Name="Notch"
                     Margin="0,12,0,0"
                     MaxNotchWidth="420"
                     Command="{Binding ExpandCommand}">
        <HorizontalStackLayout Spacing="8">
            <Ellipse Fill="Red" WidthRequest="10" HeightRequest="10" VerticalOptions="Center" />
            <Label Text="{Binding Elapsed}" TextColor="White" FontAttributes="Bold" />
        </HorizontalStackLayout>
    </shiny:NotchView>
</Grid>
```

```csharp
Notch.Content = new NowPlayingView();   // grows to fit, new content fades in
Notch.Content = null;                   // shrinks to MinNotchWidth x MinNotchHeight
Notch.NotchSizeChanged += (_, e) => Debug.WriteLine($"{e.OldSize} -> {e.NewSize}");
```

Properties: `Content`, `Shape` (Island/Notch), `SurfaceColor` (Black), `Stroke`, `StrokeThickness`, `ContentPadding` (14,8), `MinNotchWidth` (120), `MinNotchHeight` (36), `MaxNotchWidth`/`MaxNotchHeight` (∞, and the parent's width also caps it), `CornerRadius` (-1 = auto pill capped at 28), `MorphDuration` (420ms), `MorphEasing` (null = soft spring), `HasShadow` (true), `Command`, `CommandParameter`, read-only `NotchSize`. Events: `Tapped`, `NotchSizeChanged` (when a morph starts), `MorphCompleted`.

## Blazor

```razor
<NotchView ContentKey="@state" MaxNotchWidth="420" OnClick="Toggle"
           OnNotchSizeChanged="s => size = s">
    @switch (state)
    {
        case "recording":
            <span style="font-weight:700">● 0:42</span>
            break;
        case "playing":
            <NowPlaying />
            break;
    }
</NotchView>
```

Parameters: `ChildContent`, `ContentKey`, `Inline` (false = top of the screen), `Shape` (Notch), `SurfaceColor` (string?, null = black via `--shiny-notch-surface`), `Stroke`, `StrokeThickness`, `ContentPadding` ("8px 14px"), `MinNotchWidth`, `MinNotchHeight`, `MaxNotchWidth?`, `MaxNotchHeight?`, `CornerRadius`, `MorphDuration` (int ms), `HasShadow`, `OnClick`, `OnNotchSizeChanged` (`EventCallback<NotchSize>`), `CssClass`, plus splatted attributes (a `style` is merged, not swapped). Read-only `Size`. Honours `prefers-reduced-motion`.

## macOS hardware notch (Desktop add-on)

`INotchWindowService` puts a `NotchView` in the MacBook camera notch, above the menu bar and all apps. It only exists on `net10.0-macos`, so **always wrap it in `#if MACOS`**. Don't call it from shared code without the guard, and don't invent an `IsSupported` check; there isn't one.

```csharp
#if MACOS
builder
    .UseShinyControls()
    .UseDesktopNotch(o =>
    {
        o.MaxWidth = 520;                 // points
        o.UseHardwareNotch = true;        // false = floating island under the menu bar
        o.Configure = notch => notch.ContentPadding = new Thickness(18, notch.ContentPadding.Top, 18, 16);
    });

// inject INotchWindowService
await notchWindow.ShowAsync(new Label { Text = "Recording 0:42", TextColor = Colors.White });
await notchWindow.SetContentAsync(new NowPlayingView());  // grows out of the notch
await notchWindow.SetContentAsync(null);                  // back into the notch
notchWindow.Hide();
notchWindow.Tapped += (_, _) => { /* ... */ };
bool notched = notchWindow.HasHardwareNotch;
Size cutout = notchWindow.HardwareNotchSize;   // Size.Zero without a notch

// per-content styling (runs after options.Configure; each content gets a fresh NotchView)
await notchWindow.SetContentAsync(new PeekView(cutout.Width), n =>
{
    n.ContentPadding = new Thickness(10, 0);   // beside the camera, not below it
    n.MinNotchHeight = cutout.Height;
});

// hover-to-expand: mouse monitors, no accessibility permission needed
notchWindow.PointerEntered += (_, _) => Expand();
notchWindow.PointerExited += (_, _) => Collapse();   // also bool notchWindow.IsPointerOver
#endif
```

`NotchWindowOptions`: `UseHardwareNotch` (true), `TopMargin` (8, floating island only), `MaxWidth` (640), `MaxHeight` (480), `MorphDuration` (300ms), `JoinAllSpaces` (true), `Configure` (`Action<NotchView>`, runs after the service's own geometry so it wins). With a hardware notch the service sets `Shape = Notch`, sizes the empty notch to the cut-out, and pads the top by the cut-out's height so content sits below the camera. The window never takes keyboard focus, so content should be for glancing and clicking, not text entry.
