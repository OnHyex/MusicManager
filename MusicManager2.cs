using Crosstales.BWF.Data;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using static RootMotion.Demos.AnimationWarping;

namespace MusicManager
{
    public sealed class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;
        internal bool VanillaMusicHasEnded = true;
        internal bool PlayingVanillaMusic;
        internal VanillaSongInfo CurrentlyPlayingVanillaSong;
        internal SongInfo CurrentlyPlayingModdedSong;
        internal AudioSource Source;
        internal List<SongCategoryData> SongData = new List<SongCategoryData>();
        internal List<SongInfo> AllSongs = new List<SongInfo>();
        internal bool FinishedLoading;
        internal bool IsNextModdedSongFinishedLoading;
        internal bool CurrentlyPreparingNextSong;
        internal bool ForcePlayingNextSong;
        internal bool StoredVanillaMusicState;
        internal bool CurrentlyInPulsar;
        internal string CurrentlyPlayingSongName = "";
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;
            Source = gameObject.AddComponent<UnityEngine.AudioSource>();
            Source.loop = false;
            SongData.Add(new SongCategoryData(Mod.Instance.MusicDirectory));
            foreach (DirectoryInfo directory in Mod.Instance.MusicSubDirectories)
            {
                SongData.Add(new SongCategoryData(directory));
            }
            _ = GetSongsFromFolder();
        }
        private async Task GetSongsFromFolder()
        {
            await Task.Yield();
            Task[] SongFileInitializers = new Task[SongData.Count];
            for (int i = 0; i < SongData.Count; i++)
            {
                int temp = i;
                SongFileInitializers[i] = Task.Run(async () => await GetSongsFromDirectory(SongData[temp]));
            }
            await Task.WhenAll(SongFileInitializers);
            for (int i = 0; i < SongData.Count; i++)
            {
                SongFileInitializers[i].Dispose();
                AllSongs.AddRange(SongData[i].AddedSongs.Where(song => song.song != null));
            }
            AllSongs.Do(song =>
            {
                if (song.Name.Contains('.'))
                {
                    song.Name = song.Name.Split(new char[] { '.' }, 2)[0];
                }
            });
            FinishedLoading = true;
        }
        private async Task GetSongsFromDirectory(SongCategoryData data)
        {
            await Task.Yield();
            FileInfo[] songFiles = data.Directory.GetFiles("*.*");
            for (int i = 0; i < songFiles.Length; i++)
            {
                if (!songFiles[i].Name.Contains(".json"))
                {
                    data.AddSong(songFiles[i]);
                }
                await Task.Yield();
            }
        }
        void Update()
        {
            if (FinishedLoading && Settings.Enabled)
            {
                if (Application.isFocused && CurrentlyInPulsar && Source != null && !CurrentlyPreparingNextSong)
                {
                            if (VanillaMusicHasEnded && !Source.isPlaying)
                            {
                                this.StartCoroutine(PlayNext());
                            }
                    Source.volume = Settings.Volume.Value;
                        }
                //Pausing and Unpausing Music when application is tabbed out
                if (Application.isFocused != CurrentlyInPulsar)
                {
                    if (Source != null && Source.isPlaying)
                    {
                        Source.Pause();
                    }
                    else if (!Source.isPlaying && !PlayingVanillaMusic)
                    {
                        Source.Play();
                    }
                    CurrentlyInPulsar = Application.isFocused;
                }
            }
                    //if (!PLNetworkManager.Instance.IsTyping && PLInput.Instance.GetButtonDown("MusicMenu"))
                    //{
                    //    if (songs.Count > 0)
                    //    {
                    //        source.clip = songs[UnityEngine.Random.Range(0, songs.Count - 1)];
                    //    }
                    //}
                }
        internal IEnumerator PlayNext()
        {
            CurrentlyPreparingNextSong = true;
            if (CurrentlyPlayingVanillaSong != null)
            {
                CurrentlyPlayingVanillaSong.StopSong();
            }
            yield return null;
            if (!ForcePlayingNextSong)
            {
                //if (CurrentlyPlayingModdedSong != null && CurrentlyPlayingModdedSong.audio != null)
                //{
                //    Destroy(CurrentlyPlayingModdedSong.audio);
                //}
                StoredVanillaMusicState = PlayingVanillaMusic;
                if (AllSongs.Count < 1 || UnityEngine.Random.Range(0f, 1f) < Settings.ChanceOfVanillaMusic)
                {
                    PlayingVanillaMusic = true;
                    CurrentlyPlayingVanillaSong = PickNextVanillaSong();
                }
                else
                {
                    PlayingVanillaMusic = false;
                    CurrentlyPlayingModdedSong = PickNextModdedSong();
                    this.StartCoroutine(LoadModdedSong());
                }
            }
            else
            {
                ForcePlayingNextSong = false;
                if (!PlayingVanillaMusic)
                {
                    IsNextModdedSongFinishedLoading = false;
                    this.StartCoroutine(LoadModdedSong());
                }
            }
            int i = 0;
            while (i < 10)
            {
                if (Source != null)
                {
                    Source.volume = Settings.Volume - ((Settings.Volume / 10) * i);
                }
                yield return null;
                i++;
            }
            if (Source != null && Source.isPlaying)
            {
                Source.Stop();
            }
            if (PlayingVanillaMusic)
            {
                if (StoredVanillaMusicState)
                {
                    MusicEndPatch.HoldStopForForcePlay = true;
                }
                VanillaMusicHasEnded = false;
                CurrentlyPlayingVanillaSong.PlaySong();
                CurrentlyPlayingSongName = CurrentlyPlayingVanillaSong.Name;
            }
            else
            {
                if (Source != null)
                {
                    yield return new WaitUntil(() => IsNextModdedSongFinishedLoading);
                    yield return null;
                    CurrentlyPlayingSongName = CurrentlyPlayingModdedSong.Name;
                    Source.clip = CurrentlyPlayingModdedSong.audio;
                    Source.volume = Settings.Volume;
                    Source.Play();
                }
            }
            CurrentlyPreparingNextSong = false;
        }
        internal void ForcePlaySong(VanillaSongInfo vanillaSong, SongInfo moddedSong)
        {
            if (!CurrentlyPreparingNextSong)
            {
                //if (CurrentlyPlayingModdedSong != null && CurrentlyPlayingModdedSong.audio != null)
                //{
                //    Destroy(CurrentlyPlayingModdedSong.audio);
                //}
                if (vanillaSong == null)
                {
                    CurrentlyPlayingModdedSong = moddedSong;
                    ForcePlayingNextSong = true;
                    PlayingVanillaMusic = false;
                }
                else
                {
                    if (CurrentlyPlayingVanillaSong != null)
                    {
                        CurrentlyPlayingVanillaSong.StopSong();
                    }
                    StoredVanillaMusicState = PlayingVanillaMusic;
                    CurrentlyPlayingVanillaSong = vanillaSong;
                    ForcePlayingNextSong = true;
                    PlayingVanillaMusic = true;
                }
                this.StartCoroutine(PlayNext());
            }
            
        }
        private VanillaSongInfo PickNextVanillaSong()
        {
            if (Settings.CategoriesMode)
            {
                List<VanillaSongInfo> list = VanillaSongInfo.VanillaSongInfos.FindAll(song => ((song.IsCombatTrack == PLMusic.Instance.m_CombatMusicPlaying) && (song.IsSpecialMusic == PLMusic.Instance.m_SpecialMusicPlaying) && (song.IsPlanetMusic == PLMusic.Instance.m_PlanetMusicPlaying)));
                if (list.Count > 0)
                {
                    return list[UnityEngine.Random.Range(0, list.Count - 1)];
                }
                else
                {
#if DEBUG
                    Debug.Log("No Vanilla Music Found");
#endif
                    return VanillaSongInfo.VanillaSongInfos[UnityEngine.Random.Range(0, VanillaSongInfo.VanillaSongInfos.Count - 1)];
                }
            }
            else
            {
                return VanillaSongInfo.VanillaSongInfos[UnityEngine.Random.Range(0, VanillaSongInfo.VanillaSongInfos.Count - 1)];
            }
        }
        private SongInfo PickNextModdedSong()
        {
            //MusicPatch.CurrentModeStorage[0], MusicPatch.CurrentModeStorage[1], MusicPatch.CurrentModeStorage[2], MusicPatch.CurrentModeStorage[3], MusicPatch.CurrentModeStorage[4]
            //bool combat, bool ambient, bool boss, bool warp, bool planet
            List<SongInfo> songInfos = AllSongs;
            if (Settings.CategoriesMode)
            {
                //Debug.Log("Sorting Categories");
                List<SongInfo> tempSongInfos = AllSongs.Where(song => (song.IsCombatTrack & MusicPatch.CurrentModeStorage[0]) | (song.IsBossMusic & MusicPatch.CurrentModeStorage[2]) | (song.IsAmbientMusic & MusicPatch.CurrentModeStorage[1]) | (song.IsPlanetMusic & MusicPatch.CurrentModeStorage[4]) | (song.IsWarpMusic & MusicPatch.CurrentModeStorage[3])).ToList<SongInfo>();
                if (tempSongInfos.Count > 0)
                {
                    songInfos = tempSongInfos;
                }
                //Debug.Log($"{songInfos.Count}");
            }
            return songInfos[UnityEngine.Random.Range(0,songInfos.Count)];
        }
        private IEnumerator LoadModdedSong()
        {
            if (CurrentlyPlayingModdedSong.audio != null)
            {
                IsNextModdedSongFinishedLoading = true;
                yield break;
            }
            string[] songInfo = CurrentlyPlayingModdedSong.song.Name.Split(new char[] { '.' }, 2);
            string songName = songInfo[0];
            string fileType = songInfo[1];
            AudioType type = (AudioType)0;
            switch (fileType)
            {
                case "mp3":
                case "mp2":
                    type = AudioType.MPEG;
                    break;
                case "wav":
                    type = AudioType.WAV;
                    break;
                case "ogg":
                    type = AudioType.OGGVORBIS;
                    break;
                case "aiff":
                    type = AudioType.AIFF;
                    break;
                case "aac":
                    type = AudioType.ACC;
                    break;
                default:
                    //Debug.Log("Not Supported AudioFile type");
                    break;
            }
            if (type != 0)
            {
                //string url = string.Format("file://{0}", songFile.Name);
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(CurrentlyPlayingModdedSong.song.FullName, type))
                {
                    TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
                    yield return www.SendWebRequest();
                    //Debug.Log("Awaiting Completion of load");
                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        //Debug.Log(www.error);
                    }
                    else
                    {
                        AudioClip addedSong = DownloadHandlerAudioClip.GetContent(www);
                        addedSong.name = CurrentlyPlayingModdedSong.Name;
                        CurrentlyPlayingModdedSong.audio = addedSong;
                    }

                }
            }
            IsNextModdedSongFinishedLoading = true;
        }
        internal async Task ReloadSongs()
        {
            FinishedLoading = false;
            while (CurrentlyPreparingNextSong)
            {
                await Task.Delay(100);
            }
            ClearAllAudioClips();
            DirectoryInfo musicDirectory = new DirectoryInfo(Mod.Instance.MusicDirectory.FullName);
            if (!musicDirectory.Exists)
            {
                musicDirectory.Create();
                Mod.Instance.MusicDirectory = musicDirectory;
            }
            OutputAllJson();
            AllSongs.Clear();
            ReloadDirectories();
            await GetSongsFromFolder();
            FinishedLoading = true;
        }
        internal void OutputAllJson()
        {
            for (int i = 0; i < MusicManager.Instance.SongData.Count; i++)
            {
                MusicManager.Instance.SongData[i].OutputJson();
            }
        }
        private void ReloadDirectories()
        {
            //SongData.AsParallel().Do(input => input.AddedSongs.Do(song => song = null));
            SongData.Clear();
            SongData.Add(new SongCategoryData(Mod.Instance.MusicDirectory));
            Mod.Instance.MusicSubDirectories = Mod.Instance.MusicDirectory.GetDirectories();
            foreach (DirectoryInfo directory in Mod.Instance.MusicSubDirectories)
            {
                SongData.Add(new SongCategoryData(directory));
            }
        }
        internal void ClearAllAudioClips()
        {
            _ = SafeClearAllAudioClips();
        }
        private async Task SafeClearAllAudioClips()
        {
            await Task.Yield();
            while (CurrentlyPreparingNextSong)
            {
                await Task.Delay(100);
            }
            for (int i = 0; i < AllSongs.Count; i++)
            {
                if (AllSongs[i].audio != null)
                {
                    Destroy(AllSongs[i].audio);
                }
            }
        }
    }
}
