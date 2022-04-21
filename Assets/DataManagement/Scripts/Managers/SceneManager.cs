using System.IO;
using UnityEngine;



namespace DataManagement
{
    /// <copyright file="SceneManager.cs">
    /// Copyright (c) 2019 All Rights Reserved
    /// </copyright>
    /// <author>Kevin Hummel</author>
    /// <date>18/03/2019 21:41 PM </date>
    /// <summary>
    /// This class handles a scene and load data from the corresponding scene into the DataReferences.
    /// When making a new scene where data needs too be saved and loaded make sure this class is added into the hierarchy.
    /// </summary>
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance { get; set; }

        [SerializeField] private string _sceneID = null;

        public DataReferences DataReferences => _dataReferences;
        [SerializeField] private DataReferences _dataReferences = null;

        private void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);

            Instance = this;

            if (DataManager.Instance == null) return;

            _dataReferences.ID = _sceneID;

            string t_path = Application.persistentDataPath + "/" + DataManager.Instance.ID + "/" + _dataReferences.ID + "/";
            if (!Directory.Exists(t_path))
                Directory.CreateDirectory(t_path);

            Build();
        }

        public void Reload()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            ClearAllData();
        }

        public void ManualSave(string p_input)
        {
            DataManager.Instance.OverideSave(p_input);
        }

        public void ClearAllData()
        {
            _dataReferences.SaveData.ids.Clear();
            _dataReferences.SaveData.info.Clear();
            _dataReferences.SaveData.types.Clear();
        }

        private void Build()
        {
            DataManager t_dataManager = DataManager.Instance;
            if (t_dataManager != null) t_dataManager.Build();
        }

        public void LoadScene(string p_input)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(p_input);
        }
    }
}
