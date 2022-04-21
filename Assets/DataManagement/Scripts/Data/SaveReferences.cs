using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DataManagement
{
    /// <copyright file="SaveReferences.cs">
    /// Copyright (c) 2019 All Rights Reserved
    /// </copyright>
    /// <author>Kevin Hummel</author>
    /// <date>18/03/2019 21:41 PM </date>
    /// <summary>
    /// This class is used too show saved data in the Load dropdown.
    /// </summary>
    [Serializable]
    public class SaveReferences
    {
        public Dropdown load;
        public Dropdown save;

        public Button saveButton;
        public Button overrideButton;

        public List<string> saveData = new List<string>();

        public void Init()
        {
            if (saveData != null) saveData.Clear();
            if (load.options != null) load.options.Clear();
            if (save.options != null) save.options.Clear();

            saveData.Add("");
            load.options.Add(new Dropdown.OptionData());


            overrideButton.gameObject.SetActive(false);

            string t_path = Application.persistentDataPath + "/";
            List<string> t_data = new List<string>(Directory.GetDirectories(t_path));
            t_data.Reverse();

            for (int i = 0; i < t_data.Count; i++)
            {
                t_data[i] = t_data[i].Replace(t_path, "");

                if (!t_data[i].Contains("Unity") && !t_data[i].Contains("temp"))
                {
                    saveData.Add(t_data[i]);

                    if (t_data[i] != DataManager.Instance.ID)
                        load.options.Add(new Dropdown.OptionData(t_data[i]));

                    else load.options.Add(new Dropdown.OptionData(DataManager.Instance.ID));

                    load.RefreshShownValue();
                }
            }


            for (int i = 0; i < t_data.Count; i++)
            {
                t_data[i] = t_data[i].Replace(t_path, "");

                if (!t_data[i].Contains("Unity") && !t_data[i].Contains("temp"))
                {
                    if (t_data[i] != DataManager.Instance.ID)
                        save.options.Add(new Dropdown.OptionData(t_data[i]));

                    else save.options.Add(new Dropdown.OptionData(DataManager.Instance.ID));

                    save.RefreshShownValue();

                    overrideButton.gameObject.SetActive(true);
                }
            }
        }
    }
}