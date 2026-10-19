# Avatar & AvatarGroup

`Avatar` = picture → initials → silhouette, with an optional presence dot. `AvatarGroup` = overlapping facepile that collapses everyone past `Max` into a "+N" chip. Core packages, **both hosts**, same names.

## Rules for generated code

- MAUI: `shiny:Avatar`, `shiny:AvatarGroup` (xmlns `http://shiny.net/maui/controls`). Blazor: `<Avatar>`, `<AvatarGroup>` (namespace `Shiny.Blazor.Controls`).
- Enums are `AvatarShape` (`Circle`, `Rounded`, `Square`) and `AvatarStatus` (`None`, `Online`, `Away`, `Busy`, `Offline`). In Razor write `Status="AvatarStatus.Online"`; in XAML `Status="Online"`.
- Always pass `Name` even when there is a picture — it is the fallback while loading/on failure and the accessible label.
- Picture is `Uri` (string) on both hosts. MAUI also has `Source` (`ImageSource`); **Blazor has no `Source`**.
- Colours: `Color` on MAUI, CSS strings on Blazor. Leave them unset to follow the theme. Use `ColorFromName="True"` for per-person colours instead of inventing a palette.
- Taps: MAUI `Command`/`CommandParameter` or `Clicked`; Blazor `OnClick`. Do not wrap an avatar in a TapGestureRecognizer/`<button>`.
- In a group, set `Size`/`Shape`/ring on the **group**, not the children (on MAUI the group overwrites them).
- `Max` counts avatars only (the chip is extra). Use `TotalCount` when only a sample of people is loaded.
- MAUI group binding: `ItemsSource` + `ItemTemplate` whose root **must be `shiny:Avatar`** (anything else throws). A list of strings needs no template.
- Blazor group: put avatars **directly** inside (`@foreach` with `@key` is fine); never wrap each one in another element, or `Max` cannot hide it. There is no `ItemsSource` on Blazor.
- Group "+N" tap: MAUI `OverflowCommand` (parameter = hidden count `int`) or `OverflowClicked`; Blazor `OnOverflowClick` (`EventCallback<int>`).

## MAUI

```xml
<shiny:Avatar Name="{Binding DisplayName}"
              Uri="{Binding PhotoUrl}"
              Size="48"
              Status="{Binding Presence}"
              Command="{Binding OpenProfileCommand}" />

<shiny:Avatar Name="Grace Hopper" ColorFromName="True" Shape="Rounded" />

<shiny:AvatarGroup Max="4" Size="32">
    <shiny:Avatar Name="Grace Hopper" Uri="https://…/grace.jpg" />
    <shiny:Avatar Name="Ada Lovelace" />
    <shiny:Avatar Name="Alan Turing" />
    <shiny:Avatar Name="Linus Torvalds" />
    <shiny:Avatar Name="Margaret Hamilton" />
</shiny:AvatarGroup>

<shiny:AvatarGroup ItemsSource="{Binding Members}"
                   TotalCount="{Binding MemberCount}"
                   Max="5"
                   OverflowCommand="{Binding ShowAllMembersCommand}">
    <shiny:AvatarGroup.ItemTemplate>
        <DataTemplate x:DataType="local:Member">
            <shiny:Avatar Name="{Binding Name}" Uri="{Binding PhotoUrl}" ColorFromName="True" />
        </DataTemplate>
    </shiny:AvatarGroup.ItemTemplate>
</shiny:AvatarGroup>
```

```csharp
[RelayCommand]
Task ShowAllMembers(int hiddenCount) => navigator.NavigateTo<MembersViewModel>();
```

## Blazor

```razor
<Avatar Name="@user.DisplayName" Uri="@user.PhotoUrl" Size="48"
        Status="AvatarStatus.Online" OnClick="OpenProfile" />

<AvatarGroup Max="5" TotalCount="@memberCount" Size="32" OnOverflowClick="ShowAll">
    @foreach (var m in members)
    {
        <Avatar @key="m.Id" Name="@m.Name" Uri="@m.PhotoUrl" ColorFromName="true" />
    }
</AvatarGroup>

@code {
    void ShowAll(int hiddenCount) => Nav.NavigateTo("/members");
}
```

## Properties

Avatar: `Name`, `Initials`, `Uri`, `Source` (MAUI), `Size` (40), `Shape`, `Status`, `StatusColor`, `FillColor`, `TextColor`, `ColorFromName`, `RingColor`, `RingThickness`, `FontSize`, `Command`/`CommandParameter`/`Clicked` (MAUI), `OnClick` (Blazor); read-only `DisplayInitials`, `IsShowingImage` (MAUI).

AvatarGroup: `Avatars`/`ChildContent`, `ItemsSource`/`ItemTemplate` (MAUI), `Max`, `TotalCount`, `Size`, `Shape`, `Overlap` (-1 = 25% of size), `RingColor`, `RingThickness` (2), `OverflowFormat` ("+{0}"), `OverflowCommand`/`OverflowClicked` (MAUI), `OnOverflowClick` (Blazor); read-only `OverflowCount`, `VisibleAvatars` (MAUI) / `VisibleCount` (Blazor).

Blazor CSS hooks: `--shiny-avatar-size`, `--shiny-avatar-radius`, `--shiny-avatar-fill`, `--shiny-avatar-ink`, `--shiny-avatar-ring-width`, `--shiny-avatar-ring-color`, `--shiny-avatar-overlap`, `--shiny-avatar-font-size`; classes `.shiny-avatar`, `.shiny-avatar-face`, `.shiny-avatar-status--online|away|busy|offline`, `.shiny-avatar-overflow`.

### Automation ids (MAUI)

Set `AutomationId` on an `AvatarGroup` and its parts are named after it, for UI tests. A single `Avatar` is one tap target and needs nothing extra.

| Part | Id |
|---|---|
| Avatars generated from `ItemsSource` | `<id>_Avatar_<index>` |
| "+N" overflow chip | `<id>_Overflow` |

Avatars you add to `Avatars` yourself, or that your `ItemTemplate` already gives an id, keep their own.
