//TODO: Add time scrubbing for modded songs, Potentially make it so that directory names are listed prior to song names for organization

Music Manager:

INSTALL INSTRUCTIONS:

1. Download current version from releases
2. Extract the dll and put it in the mods folder
3. Run Pulsar once to create the Music folder in your Mods folder

USAGE INFORMATION:
- Once Music folder is created you can directly drop in songs or folders with songs into the folder. (Please note do not put folders inside of folders inside the Music the manager only checks 1 layer deep)
- There is an in game settings menu through PML that allows a lot of control:
    - Enabled/Disable
    - Enabling/Disabling
    - Enabling/Disabling Integration with the situational music system so that songs that you have categorized play according to the situation in these categories: Combat, Boss Fight, Space Ambience, Planetary, and Warp
        - Enabling/Disabling letting songs always play out when integrating with the situational music system
    - Controlling Chance of Vanilla Music to occur with the Music Manager
    - Controlling the Volume of Vanilla Music
    - Controlling the Volume of Modded Music
    - Displaying Currently Playing Song
    - Reloading List of Modded Songs from Music folder and direct sub folders
    - Displaying list of all Modded Songs or all Vanilla Songs
        - From list of all songs of either type songs can be forcibly played by clicking on them
    - Switching to a song organization mode which lets you:
        - From the list of all Modded Songs add and remove modded songs from categories to integrate with the situational music system
        - View the current list of modded songs a part of each category

Important Notes:
- Please do not Alt-f4 or task manager your game if you want to keep any of your song categorization that you did during that session
- Only the Music folder and folders inside the Music directory are checked for songs not folders inside of folders in the Music folder or any deeper
- Premade music packs can be installed just by dropping the entire folder extracted into your Music folder for ease of organization and allowing the songcategoryinfo.json file from overwriting the base Music folder one so that the situational music meta data isn't accidentally destroyed
- If you delete or overwrite the SongCategoryInfo.json from the Music folder or sub folders, info about when songs should play according to the situational music system will be lost
- All songs lacking prior information on when it should be played in the situational music system default to Space Ambience
- Only put files of the types .json, .mp2, .mp3, .wav, .aac, .ogg, and .aiff in the music folders otherwise unexpected behaviour could occur
