using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DataManagement
{  
    /// <copyright file="DataManager.cs">
    /// Copyright (c) 2019 All Rights Reserved
    /// </copyright>
    /// <author>Kevin Hummel</author>
    /// <date>18/03/2019 21:41 PM </date>
    /// <summary>
    /// This class is the brain behind it all and handles building all saved data back into Unity on loading into a scene.
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance { get; set; }
        public string ID { get => _id; set => _id = value; }
        [SerializeField] private string _id;

        [Header("Enable/Disable Encryption.")]
        public bool encrypt = false;

        public SaveReferences SaveReferences => _saveReferences;


        [SerializeField] private SaveReferences _saveReferences = null;

        public string TempID { get => _tempID; }
        private const string _tempID = "temp";

        private void Awake()
        {
            DontDestroyOnLoad(this);
            ID = (CheckForLastFile() ?? TempID);

            Reset();

            string t_path = Application.persistentDataPath + "/" + ID + "/";
            if (!Directory.Exists(t_path))
                Directory.CreateDirectory(t_path);

            if (Instance != null)
                Destroy(gameObject);

            Instance = this;

            SaveReferences.Init();

            Debug.Log($"{Application.persistentDataPath}/");
        }

        private string CheckForLastFile()
        {
            string t_path = $"{Application.persistentDataPath}/";

            var t_root = new DirectoryInfo(t_path);
            var t_dir = t_root.GetDirectories().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();

            if (t_dir != null && t_dir.Name != "Unity") return t_dir.Name;
            else return null;
        }

        // Add DataElement children down here too make sure they are correctly loaded in on loading a scene.
        public void Build()
        {
            SceneManager t_sceneManager = SceneManager.Instance;

            DataBuilder.BuildDataReferences();

            //Build data down here

		    DataBuilder.BuildElementsOfType<GenerationHandlerData>(t_sceneManager.DataReferences.SaveData);
		    DataBuilder.BuildElementsOfType<ItemData>(t_sceneManager.DataReferences.SaveData);
		    DataBuilder.BuildElementsOfType<PlayerData>(t_sceneManager.DataReferences.SaveData);
        }

        public void Save()
        {
            SceneManager t_sceneManager = SceneManager.Instance;

            string t_time = DateTime.Now.ToString();

            t_time = t_time.Replace("/", "-");
            t_time = t_time.Replace(" ", "_");
            t_time = t_time.Replace(":", "-");

            string _path = Application.persistentDataPath + "/";
            if (Directory.Exists(_path + ID + "/"))
            {
                string t_temp = _path + (ID == TempID || ID == "Autosave" ? "SAVE" : ID);

                t_temp = t_temp.Replace(_path, "");
                t_temp = t_temp.Replace("-", "");
                t_temp = t_temp.Replace("_", "");
                t_temp = t_temp.Replace(":", "");
                t_temp = t_temp.Replace("PM", "");
                t_temp = t_temp.Replace("AM", "");

                for (int i = 0; i < t_temp.Length; i++)
                {
                    if (char.IsDigit(t_temp[i]))
                        t_temp = t_temp.Replace(t_temp[i], ' ');
                }
                t_temp = t_temp.TrimEnd();

                string t_newPath = t_temp + "_" + t_time;
                t_newPath = t_newPath.Replace(" ", "_");

                Directory.CreateDirectory(_path + t_newPath);

                for (uint i = 0; i < Directory.GetDirectories(_path + ID).Length; i++)
                {
                    string t_name = Directory.GetDirectories(_path + ID)[i];
                    Directory.CreateDirectory(t_name.Replace(ID, t_newPath));

                    for (uint a = 0; a < Directory.GetFiles(t_name).Length; a++)
                        File.Copy(Directory.GetFiles(t_name)[a], Directory.GetFiles(t_name)[a].Replace(ID, t_newPath));
                }

                Debug.Log("Saving Data to: " + t_newPath);

                ID = t_newPath;
                SaveReferences.Init();

                for (int i = 0; i < t_sceneManager.DataReferences.SaveData.info.Count; i++)
                    t_sceneManager.DataReferences.SaveData.info[i].Save();

                Reset();
            }
        }
        public void OverideSave(string p_input = null)
        {
            SceneManager t_sceneManager = SceneManager.Instance;
            if (t_sceneManager != null)
            {
                if (p_input != null && p_input != "")
                {
                    ID = p_input;
                }
                else ID = SaveReferences.saveData[SaveReferences.save.value + 1];

                string _path = Application.persistentDataPath + "/";

                Directory.Delete(_path + ID, true);

                for (uint i = 0; i < Directory.GetDirectories(_path + _tempID).Length; i++)
                {
                    string t_name = Directory.GetDirectories(_path + _tempID)[i];
                    Directory.CreateDirectory(t_name.Replace(_tempID, ID));

                    for (uint a = 0; a < Directory.GetFiles(t_name).Length; a++)
                        File.Copy(Directory.GetFiles(t_name)[a], Directory.GetFiles(t_name)[a].Replace(_tempID, ID));
                }

                SaveReferences.Init();
                Debug.Log("Saving Data to: " + ID);

                for (int i = 0; i < t_sceneManager.DataReferences.SaveData.info.Count; i++)
                    t_sceneManager.DataReferences.SaveData.info[i].Save();
            }

            Reset();
        }

        public void Load()
        {
            ID = SaveReferences.saveData[SaveReferences.load.value];

            SceneManager t_sceneManager = SceneManager.Instance;
            if (t_sceneManager != null)
                Build();

            t_sceneManager.Reload();

            Reset();
        }

        private void Reset()
        {
            if (ID != TempID)
            {
                Directory.CreateDirectory(TempID);

                for (uint i = 0; i < Directory.GetDirectories(Application.persistentDataPath + "/" + ID).Length; i++)
                {
                    string t_name = Directory.GetDirectories(Application.persistentDataPath + "/" + ID)[i];
                    Directory.CreateDirectory(t_name.Replace(ID, TempID));

                    for (uint a = 0; a < Directory.GetFiles(t_name).Length; a++)
                        File.Copy(Directory.GetFiles(t_name)[a], Directory.GetFiles(t_name)[a].Replace(ID, TempID), true);
                }

                ID = TempID;
            }
        }

        [ContextMenu("Clear All Data.")]
        public void ClearAllData()
        {
            string t_path = Application.persistentDataPath + "/";
            string[] t_data = Directory.GetDirectories(t_path);
            for (uint i = 0; i < t_data.Length; i++)
            {
                if (!t_data[i].Contains("Unity"))
                {
                    Directory.Delete(t_data[i], true);
                    Debug.Log("Cleaning Data from: " + t_data[i]);
                }
            }
        }

        public void ClearData(string p_input)
        {
            string t_path = Application.persistentDataPath + "/";
            string[] t_data = Directory.GetFiles(t_path + "Autosave/GameData/");

            for (uint i = 0; i < t_data.Length; i++)
            {
                if (t_data[i].Contains(p_input))
                    File.Delete(t_data[i]);
            }
        }

        private void OnDestroy()
        {
            string t_temp = Application.persistentDataPath + "/" + TempID + "/";
            if (Directory.Exists(t_temp))
                Directory.Delete(t_temp, true);
        }
    }
}
