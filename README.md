# BeatSaberCustomNotes
CustomNotes adds the ability to swap out the default notes with custom ones in Beat Saber.

# Installation Instructions
* Download the latest version of CustomNotes from any current mod manager.
* You can also manually install the mod by downloading the (latest?) prebuilt "CustomNotes.dll" from the "Releases Tab" and copying it into your Beat Saber plugins folder.

To change which note model is used, simply select a different note in the Mod settings for CustomNotes in-game, or press the "N" key.

## Notice
While a custom note is being used AND you are either playing with the "Ghost Notes" or "Disappearing Arrows" modifier turned on, high-score submission will be temporarily disabled.

By switching to the default notes or turning off these modifiers, score submission will be re-enabled. No need for a game restart!

## For developers

### Contributing to CustomNotes
In order to build this project, please add your Beat Saber directory path to the `CustomNotes.csproj.user` file located in the project directory like for example this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Set "YOUR OWN" Beat Saber folder here to resolve most of the dependency paths! -->
    <BeatSaberDir>E:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

If you plan on adding any new dependencies which are located in the Beat Saber directory, it would be nice if you edited the paths to use `$(BeatSaberDir)` in `CustomNotes.csproj` like so to keep some consistency

```xml
...
<Reference Include="BS_Utils">
  <HintPath>$(BeatSaberDir)\Plugins\BS_Utils.dll</HintPath>
</Reference>
<Reference Include="IPA.Loader">
  <HintPath>$(BeatSaberDir)\Beat Saber_Data\Managed\IPA.Loader.dll</HintPath>
</Reference>
...
```