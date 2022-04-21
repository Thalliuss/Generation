using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DataManagement
{
    /// <copyright file="DataClassEditor.cs">
    /// Copyright (c) 2019 All Rights Reserved
    /// </copyright>
    /// <author>Kevin Hummel</author>
    /// <date>18/03/2019 21:41 PM </date>
    /// <summary>
    /// This class handles the easy creation of data classes trough the use of a custom Editor.
    /// </summary>
    [CustomEditor(typeof(SaveData))]
    public class SaveDataEditor : Editor
    {
        private string _path = "";

        public SaveData classReferences;

        private enum OutputType { Main, Data, Initialize, Values, Continuous}
        private string PropertyHandler(OutputType p_input)
        {
            if (p_input == OutputType.Initialize)
            {
                List<string> t_temp = new List<string>();
                for (int i = 0; i < classReferences.properties.Length; i++)
                {
                    string t_data = "\t\tLoad" + char.ToUpper(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + "();\n";

                    t_temp.Add(t_data);
                }
                return string.Join("", t_temp.ToArray());
            }
            if (p_input == OutputType.Main)
            {
                List<string> t_temp = new List<string>();
                for (int i = 0; i < classReferences.properties.Length; i++)
                {
                    string t_data = "\tpublic void Save" + char.ToUpper(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + "()\n" + "\t{\n" + "\t\t_data." + char.ToUpper(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + " = _" + char.ToLower(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + ";\n" + "\t\t_data.Save();\n" + "\t}\n" +
                                    "\tpublic void Load" + char.ToUpper(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + "()\n" + "\t{\n" + "\t\t_" + char.ToLower(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + " = _data." + char.ToUpper(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + ";\n" + "\t}\n\n";

                    t_temp.Add(t_data);
                }
                return string.Join("", t_temp.ToArray());
            }
            if (p_input == OutputType.Data)
            {
                List<string> t_temp = new List<string>();
                for (int i = 0; i < classReferences.properties.Length; i++)
                {
                    string t_data = "\tpublic " + TypeCheck(classReferences.properties[i].type.ToString()) + " " + char.ToUpper(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + " { get => _" + classReferences.properties[i].name.ToLower() + "; set => _" + classReferences.properties[i].name.ToLower() + " = value; }\n" +
                                    "\t[SerializeField] private " + TypeCheck(classReferences.properties[i].type.ToString()) + " _" + classReferences.properties[i].name.ToLower() + ";\n";

                    t_temp.Add(t_data);
                }
                return string.Join("", t_temp.ToArray()); ;
            }
            if (p_input == OutputType.Values)
            {
                List<string> t_temp = new List<string>();
                for (int i = 0; i < classReferences.properties.Length; i++)
                {
                    string t_data = "\tprivate " + TypeCheck(classReferences.properties[i].type.ToString()) + " _" + classReferences.properties[i].name.ToLower() + ";\n";

                    t_temp.Add(t_data);
                }
                return string.Join("", t_temp.ToArray()); ;
            }
            if (p_input == OutputType.Continuous)
            {
                List<string> t_temp = new List<string>();

                for (int i = 0; i < classReferences.properties.Length; i++)
                {
                    if (classReferences.properties[i].continuousSave) 
                    {
                        string t_data = "\t\t\tSave" + char.ToUpper(classReferences.properties[i].name[0]) + classReferences.properties[i].name.Substring(1) + "();\n";

                        t_temp.Add(t_data);
                    }
                }
                return string.Join("", t_temp.ToArray());
            }
            return null;
        }

        private string TypeCheck(string p_input)
        {
            if (p_input.Contains("Int")) return p_input.ToLower();
            if (p_input.Contains("String")) return p_input.ToLower();
            if (p_input.Contains("Float")) return p_input.ToLower();
            if (p_input.Contains("List")) return p_input + "<T>";
            if (p_input.Contains("Bool")) return p_input.ToLower();

            return p_input;
        }

        public void CreateClass(string p_input)
        {
            _path = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(classReferences).Replace(classReferences.name + ".asset", "");

            if (p_input.Length > 0 && File.Exists(_path + "/" + p_input + ".cs") == false && classReferences != null)
            {
                using (StreamWriter t_dataclass = new StreamWriter(_path + "/" + p_input + "Data.cs"))
                {
                    string t_input = ("using UnityEngine;\n") +
                                    ("using System.Collections.Generic;\n") +
                                    ("using DataManagement;\n") +
                                    ("\n") +
                                    ("public class " + p_input + "Data" + " : DataElement {\n") +
                                    ("\n") +
                                    (PropertyHandler(OutputType.Data)) +
                                    ("\n") +
                                    ("\tpublic " + p_input + "Data" + "(string p_id) : base(p_id)\n") +
                                    ("\t{\n") +
                                    ("\t\tID = p_id;\n") +
                                    ("\t}\n") +
                                    ("}");

                    t_dataclass.Write(t_input);

                    File.Create(t_dataclass.ToString()).Dispose();
                }

                using (StreamWriter t_class = new StreamWriter(_path + "/" + p_input + ".cs"))
                {
                    string t_input = ("using UnityEngine;\n") +
                                    ("using System.Collections.Generic;\n") +
                                    ("using System.Collections;\n") +
                                    ("using DataManagement;\n") +
                                    ("using System.Linq;\n") +
                                    ("\n") +
                                    ("#if UNITY_EDITOR\n") +
                                    ("using UnityEditor.SceneManagement;\n") +
                                    ("#endif\n") +
                                    ("\n") +
                                    ("public class " + p_input + " : MonoBehaviour {\n") +
                                    ("\n") +
                                    ("\t#region Save implementation\n") +
                                    ("\t// A reference too all currently saved data.\n\tprivate DataReferences _dataReferences = null;\n") +
                                    ("\n") +
                                    ("\t// A reference too the data being saved in this class.\n\tprivate " + p_input + "Data" + " _data = null;\n") +
                                    ("\n") +
                                    ("\t// The ID under wich this data will be saved.\n\t[SerializeField] private string _id = " + '"' + "none" + '"' + ";\n") +
                                    ("\n") +
                                    ("\t[ContextMenu(" + '"' + "Generate ID" + '"' + ")]\n") +
                                    ("\tpublic void GenerateID()\n") +
                                    ("\t{\n") +
                                    ("\t\tif (!Application.isPlaying)\n") +
                                    ("\t\t#if UNITY_EDITOR\n") +
                                    ("\t\t\tEditorSceneManager.MarkSceneDirty(gameObject.scene);\n") +
                                    ("\t\t#endif\n") +
                                    ("\n") +
                                    ("\t\t_id = " + '"' + '"' + ";\n") +
                                    ("\t\tSystem.Random t_random = new System.Random();\n") +
                                    ("\t\tconst string t_chars = " + '"' + "AaBbCcDdErFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789" + '"' + ";\n") +
                                    ("\t\t_id = new string( Enumerable.Repeat(t_chars, 8).Select(s => s[t_random.Next(s.Length)]).ToArray());\n") +
                                    ("\t}\n") +
                                    ("\n") +
                                    ("\tpublic bool Setup(string p_id)\n") +
                                    ("\t{\n") +
                                    ("\t\tif (_id == " + '"' + "none" + '"' + ") return false;\n") +
                                    ("\n") +
                                    ("\t\t_id = p_id;\n") +
                                    ("\n") +
                                    ("\t\tStartCoroutine(ContinuousSave());\n") +
                                    ("\n") +
                                    ("\t\t_dataReferences = SceneManager.Instance.DataReferences;\n") +
                                    ("\n") +
                                    ("\t\t_data = _dataReferences.FindElement<" + p_input + "Data" + ">(_id);\n") +
                                    ("\t\tif (_data == null) {\n") +
                                    ("\t\t\t_data = _dataReferences.AddElement<" + p_input + "Data" + ">(_id);\n") +
                                    ("\t\t\treturn false;\n") +
                                    ("\t\t}\n") +
                                    ("\n") +
                                    (PropertyHandler(OutputType.Initialize)) +
                                    ("\n") +
                                    ("\t\treturn true;\n") +
                                    ("\t}\n") +
                                    (PropertyHandler(OutputType.Main)) +
                                    ("\tprivate IEnumerator ContinuousSave()\n") +
                                    ("\t{\n") +
                                    ("\t\twhile (true)\n") +
                                    ("\t\t{\n") +
                                    ("\t\t\tyield return new WaitForSeconds(.5f);\n") +
                                    ("\n") +
                                    (PropertyHandler(OutputType.Continuous)) +
                                    ("\t\t}\n") +
                                    ("\t}\n") +
                                    ("\n") +
                                    ("\t#endregion\n") +
                                    ("\n") +
                                    (PropertyHandler(OutputType.Values)) +
                                    ("\n") +
                                    ("\tprivate void Start()\n") +
                                    ("\t{\n") +
                                    ("\t\tif(!Setup(_id))\n") +
                                    ("\t\t\treturn;\n") +
                                    ("\t}\n") +
                                    ("}");

                    t_class.Write(t_input);

                    File.CreateText(t_class.ToString()).Dispose();
                }

                string[] lines = File.ReadAllLines(Application.dataPath + "/DataManagement/Scripts/Managers/DataManager.cs");
                string allString = "";

                lines[73] = "\n\t\t    DataBuilder.BuildElementsOfType<" + p_input + "Data" + ">(t_sceneManager.DataReferences.SaveData);";

                for (int i = 0; i < lines.Length; i++)
                {
                    allString += lines[i] + "\n";
                }

                File.WriteAllText(Application.dataPath + "/DataManagement/Scripts/Managers/DataManager.cs", allString);

                AssetDatabase.Refresh();
            }
        }
        public override void OnInspectorGUI()
        {
            GUILayout.Space(20f);

            if (GUILayout.Button("Create required classes")) 
            {
                classReferences = (SaveData)target;
                CreateClass(classReferences.name);
            }

            GUILayout.Space(20f);


            base.OnInspectorGUI();
        }
    }
}