using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DataManagement;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class Item : MonoBehaviour {

	#region Save implementation
	// A reference too all currently saved data.
	private DataReferences _dataReferences = null;

	// A reference too the data being saved in this class.
	private ItemData _data = null;

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

		_data = _dataReferences.FindElement<ItemData>(_id);
		if (_data == null) {
			_data = _dataReferences.AddElement<ItemData>(_id);
			return false;
		}

		LoadPosition();
		LoadRotation();
		LoadIsAlive();

		return true;
	}
	public void SavePosition()
	{
		_data.Position = _position;
		_data.Save();
	}
	public void LoadPosition()
	{
		_position = _data.Position;
	}

	public void SaveRotation()
	{
		_data.Rotation = _rotation;
		_data.Save();
	}
	public void LoadRotation()
	{
		_rotation = _data.Rotation;
	}
	public void SaveIsAlive()
	{
		_data.IsAlive = _isAlive;
		_data.Save();
	}
	public void LoadIsAlive()
	{
		_isAlive = _data.IsAlive;
	}

	private IEnumerator ContinuousSave()
	{
		while (true)
		{
			yield return new WaitForSeconds(.5f);

			SavePosition();
			SaveRotation();
		}
	}

	#endregion

	[Serializable] public struct ItemProperties
	{
		public string name;
		public bool savable;
		public int health;
		public float lifetime;
	}
	public ItemProperties itemProperties;

	private Vector3 _position;
	private Quaternion _rotation;
	private bool _isAlive = true;

    private void Start()
	{
		StartCoroutine(LifeTimeHandler());

		if (!Setup(itemProperties.name + "_" + _id))
			return;

		if (!_isAlive)
			Destroy(gameObject);

		transform.position = _position;
		transform.rotation = _rotation;
	}

    private IEnumerator LifeTimeHandler()
    {
		if (itemProperties.lifetime == 0)
			yield return null;

		yield return new WaitForSeconds(itemProperties.lifetime);

		_isAlive = false;
		SaveIsAlive();

		Destroy(gameObject);
	}

    private void OnDestroy()
    {
		Debug.Log("Item: " + itemProperties.name + " has been Destroyed");
    }

    private void Update()
    {
		_position = transform.position;
		_rotation = transform.rotation;
    }
}