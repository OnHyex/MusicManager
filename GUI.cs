using PulsarModLoader.CustomGUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

namespace MusicManager
{
    internal sealed class Settings : ModSettingsMenu
    {
        internal static float ChanceOfVanillaMusic = 0.1f;
        internal static bool Enabled = true;
        internal static bool CategoriesMode = false;
        internal static bool VanillaMusicEnabled = true;
        internal static Vector2 AllSongsScroll = new Vector2(0, 0);
        internal static bool CategoryOrganizationMode = false;
        internal static float Volume = 1f;
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
        public override string Name()
        {
            return "Music Manager";
        }
        public override void OnOpen()
        {
            base.OnOpen();
            if (NonDuplicateVanillaSongs.Count == 0)
            {
                foreach (VanillaSongInfo song in MusicManager.Instance.VanillaSongInfos)
                {
                    if (!NonDuplicateVanillaSongs.Exists(s => s.Name.Equals(song.Name)))
                    {
                        NonDuplicateVanillaSongs.Add(song);
                    }
                }
            }
        }
        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            Enabled = GUILayout.Toggle(Enabled, "Enabled");
            CategoriesMode = GUILayout.Toggle(CategoriesMode, "Situational Music Enabled");
            VanillaMusicEnabled = GUILayout.Toggle(VanillaMusicEnabled, "Vanilla Music Enabled");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Volume: {Volume * 100}%", GUILayout.MinWidth(100),GUILayout.MaxWidth(100), GUILayout.MaxHeight(25));
            Volume = GUILayout.HorizontalSlider(Volume, 0f, 1f);
            GUILayout.EndHorizontal();
            
            if (MusicManager.Instance.FinishedLoading)
            {
                GUILayout.BeginHorizontal();
                if (ReloadDirectories != null && ReloadDirectories.IsCompleted) { ReloadDirectories.Dispose(); }
                if(GUILayout.Button("Reload Song List"))
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
                GUILayout.BeginHorizontal();
                if (CategoryOrganizationMode)
                {
                    GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.245f));
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                }
                //if (width < Mathf.Epsilon)
                //{
                //    width = GUILayoutUtility.GetLastRect().width;
                //}
                
                if (ModdedSongDisplay)
                {
                    GUILayout.Box("Modded Songs");
                    AllSongsScroll = GUILayout.BeginScrollView(AllSongsScroll, false, true);
                    for (int i = 0; i < MusicManager.Instance.AllSongs.Count; i++)
                    {
                        if (GUILayout.Button($"{MusicManager.Instance.AllSongs[i].Name}"))
                        {
                            if (!CategoryOrganizationMode)
                            {
                                //Force Play a song
                                MusicManager.Instance.ForcePlaySong(i);
                            }
                            else
                            {
                                switch (CurrentCategory)
                                {
                                    default:
                                        MusicManager.Instance.AllSongs[i].IsCombatTrack = !MusicManager.Instance.AllSongs[i].IsCombatTrack;
                                        break;
                                    case 1:
                                        MusicManager.Instance.AllSongs[i].IsAmbientMusic = !MusicManager.Instance.AllSongs[i].IsAmbientMusic;
                                        break;
                                    case 2:
                                        MusicManager.Instance.AllSongs[i].IsBossMusic = !MusicManager.Instance.AllSongs[i].IsBossMusic;
                                        break;
                                    case 3:
                                        MusicManager.Instance.AllSongs[i].IsWarpMusic = !MusicManager.Instance.AllSongs[i].IsWarpMusic;
                                        break;
                                    case 4:
                                        MusicManager.Instance.AllSongs[i].IsPlanetMusic = !MusicManager.Instance.AllSongs[i].IsPlanetMusic;
                                        break;
                                }
                                //[JsonProperty]
                                //internal bool IsCombatTrack;
                                //[JsonProperty]
                                //internal bool IsAmbientMusic;
                                //[JsonProperty]
                                //internal bool IsBossMusic;
                                //[JsonProperty]
                                //internal bool IsWarpMusic;
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
                            MusicManager.Instance.PlayVanillaMusic(NonDuplicateVanillaSongs[i]);
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                if (CategoryOrganizationMode)
                {
                    GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.245f));
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

