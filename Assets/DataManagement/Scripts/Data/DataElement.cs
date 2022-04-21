using System.IO;
using UnityEngine;

namespace DataManagement
{
    /// <summary>
    /// This class makes sure that all subclass deriving from DataElement have a constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Constructor<T> : ScriptableObject
    {
        public Constructor(T id) { }
    }

    /// <copyright file="DataElement.cs">
    /// Copyright (c) 2019 All Rights Reserved
    /// </copyright>
    /// <author>Kevin Hummel</author>
    /// <date>18/03/2019 21:41 PM </date>
    /// <summary>
    /// A data class that you want too save too disk needs too derive from this base class.
    /// </summary>
    public class DataElement : Constructor<string>
    {
        public string ID { get => _id; set => _id = value; }
        [Header("Element's ID:"), SerializeField]
        private string _id;

        public DataElement(string p_id) : base(p_id)
        {
            _id = ID;
        }

        public void Save()
        {
            DataParser.SaveJSON(_id.ToString(), JsonUtility.ToJson(this, true));
            JsonUtility.FromJsonOverwrite(DataBuilder.Decrypt(File.ReadAllText(Application.persistentDataPath + "/" + DataManager.Instance.ID + "/" + SceneManager.Instance.DataReferences.ID + "/" + _id.ToString() + ".json")), this);
        }
    }
}
