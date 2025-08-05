using System.IO;
using UnityEngine;

namespace SoulGames.EasyGridBuilderPro
{
    public static class GridSaveSystem
    {
        private const string SAVE_EXTENSION = "txt";
        private static string saveFolder => Application.dataPath + GetSaveLocation();
        private static bool isInit = false;

        private static string GetSaveLocation()
        {
            if (EasyGridBuilderPro.Instance != null)
                return EasyGridBuilderPro.Instance.saveLocation;

            Debug.LogWarning("EasyGridBuilderPro.Instance is null. Using fallback save location.");
            return "/DefaultSaves"; // fallback path
        }

        public static void Init()
        {
            if (!isInit)
            {
                isInit = true;
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);
            }
        }

        public static void Save(string fileName, string saveString, bool overwrite)
        {
            Init();
            File.WriteAllText(Path.Combine(saveFolder, $"{fileName}.{SAVE_EXTENSION}"), saveString);
        }

        public static string Load(string fileName)
        {
            Init();
            string fullPath = Path.Combine(saveFolder, $"{fileName}.{SAVE_EXTENSION}");

            if (File.Exists(fullPath))
                return File.ReadAllText(fullPath);
            else
                return null;
        }
    }

    
    /*
    public static class GridSaveSystem
    {
        private const string SAVE_EXTENSION = "txt"; //Save file extension
        private static readonly string saveFolder = Application.dataPath + EasyGridBuilderPro.Instance.saveLocation; //Save file location
        private static bool isInit = false; //Is this initialized

        public static void Init() //Initializing savesystem
        {
            if (!isInit) //Check if already initialized
            {
                isInit = true; //Set to initialized
                if (!Directory.Exists(saveFolder)) //Check if save location exist
                {
                    Directory.CreateDirectory(saveFolder); //Create savefile
                }
            }
        }

        public static void Save(string fileName, string saveString, bool overwrite) //Main save function
        {
            Init(); //Call function 'Init()'
            string saveFileName = fileName;
            File.WriteAllText(saveFolder + saveFileName + "." + SAVE_EXTENSION, saveString); //Write data to the save file
        }

        public static string Load(string fileName) //Main load function
        {
            Init(); //Call function 'Init()'
            if (File.Exists(saveFolder + fileName + "." + SAVE_EXTENSION)) //Check if save file exist
            {
                string saveString = File.ReadAllText(saveFolder + fileName + "." + SAVE_EXTENSION); //Load saved data and return
                return saveString;
            }
            else
            {
                return null;
            }
        }

    }*/
}
