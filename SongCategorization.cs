using Beebyte.Obfuscator;
using Crosstales.BWF.Data;
using Newtonsoft.Json;
using Oculus.Platform.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace MusicManager
{
    internal sealed class SongCategoryData
    {

        internal DirectoryInfo Directory;
        internal List<SongInfo> AddedSongs = new List<SongInfo>();
        internal SongCategoryData(DirectoryInfo directoryInput)
        {
            Directory = directoryInput;
            FileInfo[] files = Directory.GetFiles("*.json");
            FileInfo jsonFile = null;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Exists && files[i].Name.StartsWith("SongCategoryInfo"))
                {
                    jsonFile = files[i];
                }
            }

            if (jsonFile != null && jsonFile.Exists)
            {
                using (StreamReader r = new StreamReader(jsonFile.FullName))
                {
                    string json = r.ReadToEnd();
                    AddedSongs = JsonConvert.DeserializeObject<List<SongInfo>>(json, JsonSettings);
                }
            }
        }
        internal static JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            Formatting = (Formatting)1
        };
        internal void AddSong(FileInfo file)
        {
            string fileNameNoExtension = file.Name.Split(new char[] { '.' }, 2)[0];
            if (AddedSongs.Exists(song => song.Name.Equals(fileNameNoExtension)))
            {
                this.AddedSongs.Find(song => song.Name.Equals(fileNameNoExtension)).song = file;
            }
            else
            {
                SongInfo song = new SongInfo(fileNameNoExtension, false, true, false, false, false);
                song.song = file;
                this.AddedSongs.Add(new SongInfo(fileNameNoExtension, false, true, false, false, false));
            }
            fileNameNoExtension = null;
        }
        internal void OutputJson()
        {
            //string jsonString = JsonConvert.SerializeObject(AddedSongs);
            //= JsonUtility.ToJson(AddedSongs);
            //JObject json = new JObject();
            //List<JToken> jsonSongs = new List<JToken>(); 
            //foreach (SongInfo song in this.AddedSongs)
            //{
            //    json.Add(song.Name.Split(new char[1] { '.' })[0],JToken.FromObject(song));
            //    //jsonSongs.Add(JToken.FromObject(song));
            //}
            //StringBuilder jsonString = new StringBuilder("");
            //foreach (JToken jsonToken in jsonSongs)
            //{
            //    jsonString.Append(JsonConvert.SerializeObject(jsonToken));
            //    jsonString.Append("\n");
            //}
            string jsonString = JsonConvert.SerializeObject(AddedSongs, JsonSettings);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.FullName, "SongCategoryInfo.json"), false))
            {
                outputFile.WriteLine(jsonString);
            }
            jsonString = null;
        }
    }

}
