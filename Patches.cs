using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using VLB;
using UnityEngine;
using Steamworks;

namespace MusicManager
{
    [HarmonyPatch(typeof(PLUIMainMenu), "Start")]
    class CreateMusicManager
    {
        static void Postfix()
        {
            Debug.Log("Ran MusicManager adder");
            PLNetworkManager.Instance.gameObject.AddComponent<MusicManager>();
        }
    }
    [HarmonyPatch(typeof(PLMusic), "PlayMusic")]
    class MusicPatch
    {
        internal static bool[] CurrentModeStorage = new bool[5];
        static bool Prefix(string inMusicString, bool isCombatTrack, bool isPlanetTrack, bool isSpecialTrack, bool isLoopingTrack)
        {
            Debug.Log($"Ran PlayMusic: {MusicManager.Instance.PlayingVanillaMusic}");
            if (!Settings.Enabled)
            {
                MusicManager.Instance.PlayingVanillaMusic = true;
                return true;
            }
            isLoopingTrack = false;

            if (PLMusic.Instance.m_CurrentPlayingMusicEventString != inMusicString)
            {
                PLMusic.Instance.m_CurrentPlayingMusicEventString = inMusicString;
            }
            PLMusic.Instance.m_CombatMusicPlaying = isCombatTrack;
            PLMusic.Instance.m_PlanetMusicPlaying = isPlanetTrack;
            PLMusic.Instance.m_LoopingMusicPlaying = false;
            PLMusic.Instance.m_SpecialMusicPlaying = isSpecialTrack;
            if (Settings.CategoriesMode)
            {
                bool[] bools = new bool[5];
                bools[0] = isCombatTrack && !isSpecialTrack;//combat
                bools[1] = !isCombatTrack && !isSpecialTrack && !isPlanetTrack;//ambient
                bools[2] = isCombatTrack && isSpecialTrack;//boss
                bools[4] = isPlanetTrack;//planet

                //bools[0] = PLEncounterManager.Instance.PlayerShip != null && PLEncounterManager.Instance.PlayerShip.AlertLevel > 0;
                //PLSectorInfo sector = PLServer.GetCurrentSector();
                ////bools[2] = BossSectors.Contains(sector.m_VisualIndication) || (PLAbyssShipInfo.Instance != null && AbyssBossTypes.Contains(PLInGameUI.Instance.BossUI_SpaceTarget.ShipTypeID) || (sector.m_VisualIndication == ESectorVisualIndication.WASTEDWING && );
                //bools[2] = PLInGameUI.Instance.BossUI
                if (PLEncounterManager.Instance.PlayerShip != null)
                {
                    bools[3] = PLEncounterManager.Instance.PlayerShip.InWarp;
                }
                if (bools[3])
                {
                    bools[0] = false;
                    bools[1] = false;
                    bools[2] = false;
                    bools[4] = false;
                }
                if (CurrentModeStorage[0] != bools[0] || CurrentModeStorage[1] != bools[1] || CurrentModeStorage[2] != bools[2] || CurrentModeStorage[3] != bools[3] || CurrentModeStorage[4] != bools[4])
                {
                    CurrentModeStorage = bools;
                    if (!Settings.LetSongsPlayOut)
                    {
                        MusicManager.Instance.StartCoroutine(MusicManager.Instance.PlayNext());
                    }

                }
                bools = null;
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PLMusic), "OnMusicCallback")]
    class MusicEndPatch
    {
        internal static bool HoldStopForForcePlay;
        static void Prefix(object in_cookie, AkCallbackType in_type, object in_info)
        {
            if (!HoldStopForForcePlay)
            {
                MusicManager.Instance.VanillaMusicHasEnded = true;
            }
            HoldStopForForcePlay = false;
            Debug.Log($"Ran Callback: {MusicManager.Instance.VanillaMusicHasEnded}, {MusicManager.Instance.PlayingVanillaMusic}");
        }
    }
    [HarmonyPatch(typeof(PLMusic), "StopCurrentMusic")]
    class StopMusicPatch
    {
        
        static bool Prefix()
        {
            Debug.Log($"Ran Stop: {MusicManager.Instance.PlayingVanillaMusic}");
            if (!Settings.Enabled)
            {
                return true;
            }
            bool enabled = true;
            if (MusicManager.Instance.PlayingVanillaMusic && MusicManager.Instance.CurrentlyPlayingVanillaSong.GetName().Equals(PLMusic.Instance.m_CurrentPlayingMusicEventString))
            {
                enabled = false;
            }
            return enabled;
        }
    }
    [HarmonyPatch(typeof(PLGlobal), "OnApplicationQuit")]
    class QuitSavePatch
    {
        static void Prefix()
        {
            MusicManager.Instance.OutputAllJson();
        }
    }
    [HarmonyPatch(typeof(PLServer), "CPEI_HandleActivateWarpDrive")]
    class UnloadingPatch
    { 
        static void Postfix()
        {
            MusicManager.Instance.ClearAllAudioClips();
        }
    }
    [HarmonyPatch(typeof(PLServer), "Start")]
    class StartServerUnloadingPatch
    {
        static void Postfix()
        {
            MusicManager.Instance.ClearAllAudioClips();
        }
    }
}
