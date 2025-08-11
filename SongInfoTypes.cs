using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace MusicManager
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class SongInfo
    {
        public SongInfo(string name, bool combat = false, bool ambient = false, bool boss = false, bool warp = false, bool planet = false)
        {
            Name = name;
            IsCombatTrack = combat;
            IsAmbientMusic = ambient;
            IsBossMusic = boss;
            IsWarpMusic = warp;
            IsPlanetMusic = planet;
        }
        [JsonProperty]
        internal string Name;
        [JsonProperty]
        internal bool IsCombatTrack;
        [JsonProperty]
        internal bool IsAmbientMusic;
        [JsonProperty]
        internal bool IsBossMusic;
        [JsonProperty]
        internal bool IsWarpMusic;
        [JsonProperty]
        internal bool IsPlanetMusic;
        [JsonIgnore]
        internal FileInfo song = null;
    }
    internal sealed class VanillaSongInfo
    {
        internal static VanillaSongInfo CreateVanillaSong(string name, bool combat = false, bool planet = false, bool special = false, bool looping = false)
        {
            VanillaSongInfo song = new VanillaSongInfo(name.Substring(3));
            song.IsCombatTrack = combat;
            song.IsPlanetMusic = planet;
            song.IsSpecialMusic = special;
            return song;
        }
        internal VanillaSongInfo(string name, bool combat = false, bool planet = false, bool special = false, bool looping = false)
        {
            this.Name = name;
            IsCombatTrack = combat;
            IsPlanetMusic = planet;
            IsSpecialMusic = special;
        }
        internal string Name;
        internal bool IsCombatTrack;
        internal bool IsPlanetMusic;
        internal bool IsSpecialMusic;
        private static readonly string Prefix = "mx_";
        internal void PlaySong()
        {
            PLMusic.Instance.PlayMusic(string.Concat(Prefix, Name), false, false, false, false);
        }
    }
}
