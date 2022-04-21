using System;
using UnityEngine;

namespace DataManagement
{
    /// <copyright file="DataClassEditor.cs">
    /// Copyright (c) 2019 All Rights Reserved
    /// </copyright>
    /// <author>Kevin Hummel</author>
    /// <date>18/03/2019 21:41 PM </date>
    /// <summary>
    /// This class handles the information required to create data classes.
    /// </summary>
    [Serializable, CreateAssetMenu()]
    public class SaveData : ScriptableObject
    {
        [Header("Input what properties you want to save below.")]
        public Properties[] properties;

        [Serializable]
        public class Properties
        {
            public bool continuousSave;
            public string name;
            public enum Types { Int, String, Float, Sprite, Vector3, List, Mesh , Quaternion, Bool}
            public Types type;
        }
    }
}