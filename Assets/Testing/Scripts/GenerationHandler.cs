using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DataManagement;
using System.Linq;
using System;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[RequireComponent(typeof(Terrain))]
public class GenerationHandler : MonoBehaviour 
{
	#region Save implementation
	// A reference too all currently saved data.
	private DataReferences _dataReferences = null;

	// A reference too the data being saved in this class.
	private GenerationHandlerData _data = null;

	// The ID under wich this data will be saved.
	[SerializeField] private string _id = "none";

	[ContextMenu("Generate ID")]
	public void GenerateID()
	{
		if (!Application.isPlaying)
		#if UNITY_EDITOR
			EditorSceneManager.MarkSceneDirty(gameObject.scene);
		#endif

		_id = "";
		System.Random t_random = new System.Random();
		const string t_chars = "AaBbCcDdErFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
		_id = new string( Enumerable.Repeat(t_chars, 8).Select(s => s[t_random.Next(s.Length)]).ToArray());
	}

	public bool Setup(string p_id)
	{
		if (_id == "none") return false;

		_id = p_id;

		StartCoroutine(ContinuousSave());

		_dataReferences = SceneManager.Instance.DataReferences;

		_data = _dataReferences.FindElement<GenerationHandlerData>(_id);
		if (_data == null) {
			_data = _dataReferences.AddElement<GenerationHandlerData>(_id);
			return false;
		}

		LoadObjects();

		return true;
	}
	public void SaveObjects()
	{
		_data.Objects = _objects;
		_data.Save();
	}
	public void LoadObjects()
	{
		_objects = _data.Objects;
    }

    private IEnumerator ContinuousSave()
	{
		while (true)
		{
			yield return new WaitForSeconds(.5f);

		}
	}

    #endregion

    #region Structs
    [Serializable] public struct TerrainObject
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;

        public TerrainObject(GameObject prefab, Quaternion rotation, Vector3 position)
        {
            this.prefab = prefab;
            this.rotation = rotation;
            this.position = position;
        }
    }
    [Serializable] private struct GenerationProperties
    {
        public string name;
        public GameObject[] prefabs;
        public int amount;
        [Range(0, 500)] public float minAngle, maxAngle, minDistance, maxHeight;

        public List<GameObject> objects;

        public GameObject GetPrefab(int p_index) 
        {
            for (int i = 0; i < prefabs.Length; i++) 
            {
                if (objects[p_index].name.Replace("(Clone)", "").Equals(prefabs[i].name)) 
                {
                    return prefabs[i];
                }
            }
            return null;
        }
    }
    #endregion

    [SerializeField] private GenerationProperties[] _generationProperties;

    private Terrain _terrain = null;
    private List<TerrainObject> _objects = new List<TerrainObject>();

    private void Start()
    {
        _terrain = GetComponent<Terrain>();

        if (!Setup(_id)) 
        {
            StartCoroutine(StartGeneration());
            return;
        }

        StartCoroutine(InstantiateFromDisk());
    }

    private IEnumerator StartGeneration()
    {
        for (int i = 0; i < _generationProperties.Length; i++)
        {
            yield return Generate(_generationProperties[i].prefabs, _generationProperties[i].name, _generationProperties[i].amount, _generationProperties[i].minAngle, _generationProperties[i].maxAngle, _generationProperties[i].minDistance, _generationProperties[i].maxHeight);
        }
        SaveTerrain();
    }

    private IEnumerator Generate(GameObject[] p_objects, string p_name, int p_amount, float p_minAngle, float p_maxAngle, float p_minDistance, float p_maxHeight)
    {
        LoadingscreenManager t_loadingscreenManager = LoadingscreenManager.Instance;

        for (int i = 0; i < p_amount; i++)
        {
            float t_x = Random.Range(transform.position.x, transform.position.x + _terrain.terrainData.size.x);
            float t_z = Random.Range(transform.position.z, transform.position.z + _terrain.terrainData.size.z);
            float t_y = _terrain.SampleHeight(new Vector3(t_x, transform.position.y, t_z));
            Vector3 t_randomSpawn = new Vector3(t_x, t_y, t_z);

            if (CheckAngle(t_randomSpawn, p_minAngle, p_maxAngle) && CheckDistance(p_name, t_randomSpawn, p_minDistance) && t_randomSpawn.y < p_maxHeight)
            {
                GameObject t_objectToSpawn = p_objects[Random.Range(0, p_objects.Length)];

                t_loadingscreenManager.OpenLoadingscreen(i, p_amount - 1, "Instantiating " + p_name);

                GameObject t_object = Instantiate(t_objectToSpawn, t_randomSpawn, new Quaternion(), transform);
                t_object.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));

                for (int a = 0; a < _generationProperties.Length; a++)
                {
                    if (_generationProperties[a].name == p_name)
                        _generationProperties[a].objects.Add(t_object);

                }

                if (i % 100 == 0) yield return new WaitForEndOfFrame();
            }
            else
            {
                i--;
                continue;
            }
        }
        yield return null;
    }

    private bool CheckDistance(string p_name, Vector3 p_origin, float p_distance)
    {
        for (int i = 0; i < _generationProperties.Length; i++)
        {
            if (_generationProperties[i].name == p_name)
            {
                for (int a = 0; a < _generationProperties[i].objects.Count; a++)
                {
                    if (Vector3.Distance(p_origin, _generationProperties[i].objects[a].transform.position) > p_distance) continue;
                    else return false;
                }
                return true;
            }
        }
        return false;
    }

    private bool CheckAngle(Vector3 p_origin, float p_minAngle, float p_maxAngle)
    {
        float t_angle;
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(p_origin.x, p_origin.y + 10, p_origin.z), Vector3.down, out hit) && p_origin.y != 0)
        {
            if (!hit.collider.CompareTag("Terrain")) return false;

            t_angle = Vector3.Angle(hit.normal, Vector3.up);

            if (t_angle >= p_minAngle && t_angle <= p_maxAngle) return true;
            else return false;
        }
        else return true;
    }

    private void SaveTerrain() 
    {
        if (_generationProperties.Length == 0)
            return;

        for (int i = 0; i < _generationProperties.Length; i++) 
        {
            for (int a = 0; a < _generationProperties[i].objects.Count; a++) 
            {
                _objects.Add(new TerrainObject(_generationProperties[i].GetPrefab(a), _generationProperties[i].objects[a].transform.rotation, _generationProperties[i].objects[a].transform.position));
            }
        }
        SaveObjects();
    }

    private IEnumerator InstantiateFromDisk()
    {
        LoadingscreenManager t_loadingscreenManager = LoadingscreenManager.Instance;

        for (int i = 0; i < _objects.Count; i++) 
        {
            t_loadingscreenManager.OpenLoadingscreen(i, _objects.Count - 1, "Loading files from disk");

            Instantiate(_objects[i].prefab, _objects[i].position, _objects[i].rotation, transform);

            if (i % 100 == 0) yield return new WaitForEndOfFrame();
        }
    }
}