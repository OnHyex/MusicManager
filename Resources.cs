using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using System.IO;
//using System.Reflection;

//namespace MusicManager
//{
//    sealed class MusicResources
//    {
//        public static Texture2D LoadResourcePNG(string name)
//        {
//            Texture2D tex = null;
//            //Properties.Resources.arrow_down;
//            //typeof(Properties.Resources).GetProperty();
//            Stream stream = Mod.Instance.assembly.GetManifestResourceStream(string.Concat("MusicManager.Resources.",name));
//            byte[] fileData;
//            using (MemoryStream memoryStream = new MemoryStream())
//            {
//                stream.CopyTo(memoryStream);
//                fileData = memoryStream.ToArray();
//            }
//            if (fileData != null && fileData.Length > 0)
//            {
//                tex = new Texture2D(2, 2);
//                tex.LoadImage(fileData);
//            }
//            return tex;
//        }
//        private static Texture2D BuildTexFrom1Color(Color color)
//        {
//            Texture2D texture2D = new Texture2D(1, 1);
//            texture2D.SetPixel(0, 0, color);
//            texture2D.Apply();
//            return texture2D;
//        }
//        public static Texture2D BackGroundTexture = BuildTexFrom1Color(new Color(0, 0, 0, 0.3f));
//        public static Texture2D UpArrow = LoadResourcePNG("arrow-up.png");
//        public static Texture2D DownArrow = LoadResourcePNG("arrow-down.png");
//        public static Texture2D settingscog = LoadResourcePNG("cog.png");

//    }
//}
