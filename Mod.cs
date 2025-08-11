﻿
using PulsarModLoader;
using PulsarModLoader.MPModChecks;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using PulsarModLoader.Keybinds;
namespace MusicManager
{
    public sealed class Mod : PulsarMod, IKeybind
    {
        public static Mod Instance;
        internal DirectoryInfo MusicDirectory;
        internal bool Enabled = false;
        internal List<FileInfo> Songs = new List<FileInfo>();
        internal Assembly assembly = Assembly.GetExecutingAssembly();
        public Mod()
        {
            Instance = this;
            string PathInfo = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Mods\\Music"));
            if (!Directory.Exists(PathInfo))
            {
                Debug.Log("Creating Directory");
                Directory.CreateDirectory(PathInfo);
            }
            MusicDirectory = new DirectoryInfo(PathInfo);
            if (MusicDirectory == null || !MusicDirectory.Exists)
            {
                Debug.Log("Music Directory Doesn't Exist");
                return;
            }
            Songs = MusicDirectory.GetFiles("*.*").ToList<FileInfo>();
            if (Songs.Count < 1)
            {
                Debug.Log("No Music Found");
            }
        }
        public override string Version => "1.0.0";
        public override string Name => "Music Manager";
        public override string Author => "OnHyex";
        public override string ShortDescription => "A brief description";
        public override string LongDescription => "All the details";
        public override int MPRequirements => (int)MPRequirement.None;
        public override string HarmonyIdentifier() => $"{Author}.{Name}";
        public void RegisterBinds(KeybindManager manager)
        {
            manager.NewBind("Music Menu", "MusicMenu", "Basics", ",");
        }
    }
}
