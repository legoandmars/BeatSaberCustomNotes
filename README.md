# BeatSaberCustomNotes
CustomNotes adds the ability to swap out the default notes with custom ones in Beat Saber.

# Installation Instructions
Download the latest version of CustomNotes from any current mod manager or the [Github releases page](https://github.com/legoandmars/BeatSaberCustomNotes/releases/).

You can install custom note files (`.bloq` files) in the `CustomNotes` folder found in your game's directory. If this folder doesn't exist, make sure to run the game with the mod installed so it can be automatically generated.

To change which note model is used, simply select a different note in the CustomNotes mod tab in-game.

## Notice
While a custom note is being used AND you are either playing with the "Ghost Notes" or "Disappearing Arrows" modifier turned on, score submission will be temporarily disabled.

By switching to the default notes or turning off these modifiers, score submission will be re-enabled. No need for a game restart!

## For developers

### Contributing to CustomNotes
In order to build this project, please create the file `CustomNotes.csproj.user` and add your Beat Saber directory path to it in the project directory.
This file should not be uploaded to GitHub and is in the .gitignore.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Set "YOUR OWN" Beat Saber folder here to resolve most of the dependency paths! -->
    <BeatSaberDir>E:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

If you plan on adding any new dependencies which are located in the Beat Saber directory, please edit the paths to use `$(BeatSaberDir)` in `CustomNotes.csproj`

```xml
...
<Reference Include="SiraUtil">
  <HintPath>$(BeatSaberDir)\Plugins\SiraUtil.dll</HintPath>
</Reference>
<Reference Include="IPA.Loader">
  <HintPath>$(BeatSaberDir)\Beat Saber_Data\Managed\IPA.Loader.dll</HintPath>
</Reference>
...
```
