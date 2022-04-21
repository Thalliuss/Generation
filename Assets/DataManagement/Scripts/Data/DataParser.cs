using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace DataManagement
{
    /// <copyright file="DataParser.cs">
    /// Copyright (c) 2019 All Rights Reserved
    /// </copyright>
    /// <author>Kevin Hummel</author>
    /// <date>18/03/2019 21:41 PM </date>
    /// <summary>
    /// This class handles getting the data in Unity and saving it too disk.
    /// </summary>
    public static class DataParser
    {
        public static string Encrypt(string p_input)
        {
            if (DataManager.Instance.encrypt)
            {
                byte[] t_inputbuffer = Encoding.Unicode.GetBytes(p_input);
                byte[] t_outputBuffer = DES.Create().CreateEncryptor(DataReferences.key, DataReferences.iv).TransformFinalBlock(t_inputbuffer, 0, t_inputbuffer.Length);
                return System.Convert.ToBase64String(t_outputBuffer);
            }
            else return p_input;
        }

        public static ScriptableObject CreateAsset<T>(string p_name) where T : ScriptableObject
        {
            T t_asset = ScriptableObject.CreateInstance<T>() as T;

            return t_asset;
        }

        public static void SaveJSON(string p_name, string p_info)
        {
            if (DataManager.Instance == null) return;
            string t_path = Application.persistentDataPath + "/" + DataManager.Instance.ID + "/" + SceneManager.Instance.DataReferences.ID + "/" + p_name + ".json";
            if (!File.Exists(t_path)) File.Delete(t_path);

            using (FileStream fs = new FileStream(t_path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                    writer.Write(Encrypt(p_info));
            }
        }
    }
}

