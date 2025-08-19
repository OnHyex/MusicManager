using PulsarModLoader.CustomGUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using Crosstales.BWF.Data;
using PulsarModLoader;

namespace MusicManager
{
    internal sealed class Settings : ModSettingsMenu
    {
        internal static SaveValue<float> ChanceOfVanillaMusic = new SaveValue<float>("ChanceOfVanillaMusic", 1f);
        internal static bool Enabled = true;
        private static bool EnabledStateStorage = true;
        internal static bool CategoriesMode = false;
        internal static bool VanillaMusicEnabled = true;
        internal static bool LetSongsPlayOut = true;
        internal static Vector2 AllSongsScroll = new Vector2(0, 0);
        internal static bool CategoryOrganizationMode = false;
        internal static SaveValue<float> Volume = new SaveValue<float>("Volume", 1f);
        private static int CurrentCategory = 0;
        //private static float width = 0;
        private static readonly string[] CategoryNames = new string[] 
        { 
            "Combat Music",
            "Ambient Music",
            "Boss Music",
            "Warp Music",
            "Planet Music"
        };
        internal static Vector2 CategoriesScroll = new Vector2(0, 0);
        private static Task ReloadDirectories;
        private static List<VanillaSongInfo> NonDuplicateVanillaSongs = new List<VanillaSongInfo>();
        private static bool ModdedSongDisplay = true;
        internal static bool ForceLaunchedSong = false;
        public override string Name()
        {
            return "Music Manager";
        }
        internal static bool IsOpen = false;
        public override void OnOpen()
        {
            base.OnOpen();
            IsOpen = true;
            if (NonDuplicateVanillaSongs.Count == 0)
            {
                foreach (VanillaSongInfo song in VanillaSongInfo.VanillaSongInfos)
                {
                    if (!NonDuplicateVanillaSongs.Exists(s => s.Name.Equals(song.Name)))
                    {
                        NonDuplicateVanillaSongs.Add(song);
                    }
                }
            }
            tempVanillaMusicVolume = PLXMLOptionsIO.Instance.CurrentOptions.GetFloatValue("VolumeMusic");
        }
        public override void OnClose()
        {
            base.OnClose();
            IsOpen = false;
        }
        float tempVanillaMusicVolume;
        public override void Draw()
        {
            if (GUI.changed)
            {
                if (!Enabled && EnabledStateStorage)
                {
                    //Turning off vanilla music before fully reenabling
                    PLMusic.Instance.StopCurrentMusic();
                }
                if (Enabled && !EnabledStateStorage)
                {
                    //turning off Music Manager vanilla / moddded music before disabling
                    if (MusicManager.Instance.Source != null && MusicManager.Instance.Source.isPlaying)
                    {
                        MusicManager.Instance.Source.Stop();
                    }
                    if (MusicManager.Instance.CurrentlyPlayingVanillaSong != null && MusicManager.Instance.PlayingVanillaMusic)
                    {
                        MusicManager.Instance.CurrentlyPlayingVanillaSong.StopSong();
                    }
                    Enabled = EnabledStateStorage;
                    //Starting Up what should be the normal track before turning it off
                    PLMusic.Instance.PlayMusic(PLMusic.Instance.m_CurrentPlayingMusicEventString, PLMusic.Instance.m_CombatMusicPlaying, PLMusic.Instance.m_PlanetMusicPlaying, PLMusic.Instance.m_SpecialMusicPlaying, PLMusic.Instance.m_LoopingMusicPlaying);
                }
                Enabled = EnabledStateStorage;
            }
            GUILayout.BeginHorizontal();
            EnabledStateStorage = GUILayout.Toggle(EnabledStateStorage, "Enabled");
            CategoriesMode = GUILayout.Toggle(CategoriesMode, "Situational Music Enabled");
            VanillaMusicEnabled = GUILayout.Toggle(VanillaMusicEnabled, "Vanilla Music Enabled");
            LetSongsPlayOut = GUILayout.Toggle(LetSongsPlayOut, "Songs Play Till End");

            GUILayout.EndHorizontal();
            if (VanillaMusicEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Vanilla Music Chance: {(ChanceOfVanillaMusic * 100).ToString("0.0")}%");
                ChanceOfVanillaMusic.Value = GUILayout.HorizontalSlider(ChanceOfVanillaMusic.Value, 0f, 1f, GUILayout.Width(Screen.width * 0.246f));
                GUILayout.EndHorizontal();
            }

            //Volume Control
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Vanilla Volume: {(tempVanillaMusicVolume * 100).ToString("0.0")}%");
            tempVanillaMusicVolume = GUILayout.HorizontalSlider(tempVanillaMusicVolume, 0f, 1f, GUILayout.Width(Screen.width * 0.246f));
            PLXMLOptionsIO.Instance.CurrentOptions.SetFloatValue("VolumeMusic", tempVanillaMusicVolume);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Custom Volume: {(Volume * 100).ToString("0.0")}%");
            Volume.Value = GUILayout.HorizontalSlider(Volume.Value, 0f, 1f, GUILayout.Width(Screen.width * 0.246f));
            if (MusicManager.Instance.Source != null)
            {
                MusicManager.Instance.Source.volume = Volume;
            }
            GUILayout.EndHorizontal();

            //Show name of currently playing song
            if (MusicManager.Instance.PlayingVanillaMusic)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Box($"Now Playing Vanilla Music: {MusicManager.Instance.CurrentlyPlayingSongName}");

                GUILayout.EndHorizontal();
            }
            else
            {
                if (MusicManager.Instance.Source != null && MusicManager.Instance.Source.isPlaying)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Box($"Now Playing Modded Music: {MusicManager.Instance.CurrentlyPlayingSongName}");

                    GUILayout.EndHorizontal();
                }
            }
                GUILayout.BeginHorizontal();
                if (ReloadDirectories != null && ReloadDirectories.IsCompleted) { ReloadDirectories.Dispose(); }
            if (GUILayout.Button("Reload Song List") && (ReloadDirectories == null || ReloadDirectories.IsCompleted))
                {
                    ReloadDirectories = MusicManager.Instance.ReloadSongs();
                }
                if (GUILayout.Button("Modded Songs"))
                {
                    ModdedSongDisplay = true;
                }
                if (GUILayout.Button("Vanilla Songs"))
                {
                    ModdedSongDisplay = false;
                    CategoryOrganizationMode = false;
                }
                CategoryOrganizationMode = GUILayout.Toggle(CategoryOrganizationMode, "Organize Songs");
                CategoryOrganizationMode = ModdedSongDisplay && CategoryOrganizationMode;
                GUILayout.EndHorizontal();
            if (MusicManager.Instance.FinishedLoading)
            {
                GUILayout.BeginHorizontal();
                if (CategoryOrganizationMode)
                {
                    GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.246f));
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                }
                
                if (ModdedSongDisplay)
                {
                    //Display Modded Songs
                    GUILayout.Box("Modded Songs");
                    AllSongsScroll = GUILayout.BeginScrollView(AllSongsScroll, false, true);
                    for (int i = 0; i < MusicManager.Instance.AllSongs.Count; i++)
                    {
                        SongInfo song = MusicManager.Instance.AllSongs[i];
                        if (GUILayout.Button($"{song.Name}"))
                        {
                            if (!CategoryOrganizationMode)
                            {
                                //Force Play a song
                                ForceLaunchedSong = true;
                                MusicManager.Instance.ForcePlaySong(null, song);
                            }
                            else
                            {
                                switch (CurrentCategory)
                                {
                                    //Flip song category info to either be a part of or no longer a part of the currently selected category
                                    default:
                                        song.IsCombatTrack = !song.IsCombatTrack;
                                        break;
                                    case 1:
                                        song.IsAmbientMusic = !song.IsAmbientMusic;
                                        break;
                                    case 2:
                                        song.IsBossMusic = !song.IsBossMusic;
                                        break;
                                    case 3:
                                        song.IsWarpMusic = !song.IsWarpMusic;
                                        break;
                                    case 4:
                                        song.IsPlanetMusic = !song.IsPlanetMusic;
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    GUILayout.Box("Vanilla Songs");
                    AllSongsScroll = GUILayout.BeginScrollView(AllSongsScroll, false, true);
                    for (int i = 0; i < NonDuplicateVanillaSongs.Count; i++)
                    {
                        if (GUILayout.Button($"{NonDuplicateVanillaSongs[i].Name}"))
                        {
                            MusicEndPatch.HoldStopForForcePlay = true;
                            MusicManager.Instance.ForcePlaySong(NonDuplicateVanillaSongs[i], null);
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                //Display secondary box for showing what songs are in the currently selected category
                if (CategoryOrganizationMode)
                {
                    //Change Selected Category
                    GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.246f));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("<-"))
                    {
                        CategoriesScroll = new Vector2(0, 0);
                        if (CurrentCategory == 0)
                        {
                            CurrentCategory = 4;
                        }
                        CurrentCategory--;
                    }
                    GUILayout.Box($"{CategoryNames[CurrentCategory]}");
                    if (GUILayout.Button("->"))
                    {
                        CategoriesScroll = new Vector2(0, 0);
                        if (CurrentCategory == 4)
                        {
                            CurrentCategory = 0;
                        }
                        CurrentCategory++;
                    }
                    GUILayout.EndHorizontal();

                    CategoriesScroll = GUILayout.BeginScrollView(CategoriesScroll, false, true);
                    //Get list of songs that are in the category
                    List<SongInfo> categorySongs = MusicManager.Instance.AllSongs.FindAll(song =>
                    {
                        switch (CurrentCategory)
                        {
                            default:
                                return song.IsCombatTrack;
                            case 1:
                                return song.IsAmbientMusic;
                            case 2:
                                return song.IsBossMusic;
                            case 3:
                                return song.IsWarpMusic;
                            case 4:
                                return song.IsPlanetMusic;
                        }
                    });
                    //Display list of songs in category and be able to remove them if clicked
                    for (int i = 0; i < categorySongs.Count; i++)
                    {
                        if (GUILayout.Button($"{categorySongs[i].Name}"))
                        {
                            switch (CurrentCategory)
                            {
                                case 0:
                                    categorySongs[i].IsCombatTrack = !categorySongs[i].IsCombatTrack;
                                    break;
                                case 1:
                                    categorySongs[i].IsAmbientMusic = !categorySongs[i].IsAmbientMusic;
                                    break;
                                case 2:
                                    categorySongs[i].IsBossMusic = !categorySongs[i].IsBossMusic;
                                    break;
                                case 3:
                                    categorySongs[i].IsWarpMusic = !categorySongs[i].IsWarpMusic;
                                    break;
                                case 4:
                                    categorySongs[i].IsPlanetMusic = !categorySongs[i].IsPlanetMusic;
                                    break;
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            
        }
    }
}

