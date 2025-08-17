using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

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
        [JsonIgnore]
        internal AudioClip audio = null;
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
            if (PLMusic.WwiseLibCallSuccessful)
            {
                AkSoundEngine.PostEvent(string.Concat("play_",Prefix, Name), PLMusic.Instance.gameObject, 1U, new AkCallbackManager.EventCallback(PLMusic.Instance.OnMusicCallback), null);
            }
        }
        internal void StopSong()
        {
            if (PLMusic.WwiseLibCallSuccessful)
            {
                PLMusic.PostEvent(string.Concat("stop_",Prefix,Name), PLMusic.Instance.gameObject);
            }
        }
        internal string GetName()
        {
            return string.Concat(Prefix, Name);
        }
        internal static List<VanillaSongInfo> VanillaSongInfos = new List<VanillaSongInfo>()
        {
             VanillaSongInfo.CreateVanillaSong("mx_corrupteddrone_lp2", true, false, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_finalstand", true, false, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_boarders", true, false, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_lasttogo", true, false, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_infected_commander", true, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_lostcolony_theme_two", true, true, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_corrupteddrone_lp2", true, false, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_colunion_v4", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_wd_corp_v3", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_AllGent_Heist", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_cu_commander", true, true, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_infected_ambient", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_WDCommander_lp", true, true, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_ambient_3_full", false, false, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_lostcolony_theme_three", true, true, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_lostcolony_theme_one", true, true, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_ivm_genamb01", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_ivm_genamb02", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_CUAmbient_lp", false, true, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_CUExplore_lp", false, true, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_abyss_ambient_1", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_abyss_ambient_2", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_abyss_ambient_3", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_abyss_ambient_4", false, false, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_Tavern", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_ThrivingStation", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_Caverns", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_Disaster", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_CorneliaStation", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_AlienRuins", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_Lost", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_ivm_wreck", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_wdce_explore_lp2", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_Desert", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_ambient_1"),
             VanillaSongInfo.CreateVanillaSong("mx_ambient_2"),
             VanillaSongInfo.CreateVanillaSong("mx_ambient_3_loop"),
             VanillaSongInfo.CreateVanillaSong("mx_ivm_genamb01"),
             VanillaSongInfo.CreateVanillaSong("mx_ivm_genamb02"),
             VanillaSongInfo.CreateVanillaSong("mx_unseen_amb"),
             VanillaSongInfo.CreateVanillaSong("mx_finalstand"),
             VanillaSongInfo.CreateVanillaSong("mx_gap"),
             VanillaSongInfo.CreateVanillaSong("mx_boarders"),
             VanillaSongInfo.CreateVanillaSong("mx_lasttogo"),
             VanillaSongInfo.CreateVanillaSong("mx_CUAttack", true),
             VanillaSongInfo.CreateVanillaSong("mx_CUAttackAlt", true),
             VanillaSongInfo.CreateVanillaSong("mx_drone_attack_01_lp", true),
             VanillaSongInfo.CreateVanillaSong("mx_drone_attack_02_lp", true),
             VanillaSongInfo.CreateVanillaSong("mx_wd_attack_lp", true),
             VanillaSongInfo.CreateVanillaSong("mx_corrupteddrone_lp2", true),
             VanillaSongInfo.CreateVanillaSong("mx_AllGent_Attack", true),
             VanillaSongInfo.CreateVanillaSong("mx_AllGent_Heist", true),
             VanillaSongInfo.CreateVanillaSong("mx_infected_attack", true),
             VanillaSongInfo.CreateVanillaSong("mx_infected_commander", true),
             VanillaSongInfo.CreateVanillaSong("mx_AllGent_Commander", true),
             VanillaSongInfo.CreateVanillaSong("mx_FluffyBiscuit_Ambient", true),
             VanillaSongInfo.CreateVanillaSong("mx_FluffyBiscuitTheme_FullLength", true),
             VanillaSongInfo.CreateVanillaSong("mx_Polytechnic_Attack", true),
             VanillaSongInfo.CreateVanillaSong("mx_Polytechnic_MainTheme", true),
             VanillaSongInfo.CreateVanillaSong("mx_Polytechnic_SectorCommander", true),
             VanillaSongInfo.CreateVanillaSong("mx_unseen_action1", true),
             VanillaSongInfo.CreateVanillaSong("mx_unseen_action1", true),
             VanillaSongInfo.CreateVanillaSong("mx_colunion_v4"),
             VanillaSongInfo.CreateVanillaSong("mx_CUExplore_lp"),
             VanillaSongInfo.CreateVanillaSong("mx_drone_ambient_01_lp"),
             VanillaSongInfo.CreateVanillaSong("mx_drone_ambient_02_lp"),
             VanillaSongInfo.CreateVanillaSong("mx_AllGent_ExploreLP"),
             VanillaSongInfo.CreateVanillaSong("mx_AllGent_v4"),
             VanillaSongInfo.CreateVanillaSong("mx_infected_ambient"),
             VanillaSongInfo.CreateVanillaSong("mx_infected_commander"),
             VanillaSongInfo.CreateVanillaSong("mx_wdce_explore_lp2"),
             VanillaSongInfo.CreateVanillaSong("mx_unseen_amb"),
             VanillaSongInfo.CreateVanillaSong("mx_wd_corp_v3"),
             VanillaSongInfo.CreateVanillaSong("mx_Polytechnic_Ambient"),
             VanillaSongInfo.CreateVanillaSong("mx_Polytechnic_Exploration"),
             VanillaSongInfo.CreateVanillaSong("mx_unseen_amb"),
             VanillaSongInfo.CreateVanillaSong("mx_FluffyBiscuitTheme_FullLength"),
             VanillaSongInfo.CreateVanillaSong("mx_CUAttackAlt", true, true, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_ivm_darkness", false, true, false, false),
             VanillaSongInfo.CreateVanillaSong("mx_unseen_action2", true, false, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_unseen_action1", false, false, false, true),
             VanillaSongInfo.CreateVanillaSong("mx_AllGent_Commander", true, true, true, false),
             VanillaSongInfo.CreateVanillaSong("mx_warpguardian_theme_one", true, false, true, true),
             VanillaSongInfo.CreateVanillaSong("mx_warpguardian_theme_two", true, false, true, true)
        };
    }
}
