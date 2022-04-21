using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DataManagement;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class Player : MonoBehaviour {

	#region Save implementation
	// A reference too all currently saved data.
	private DataReferences _dataReferences = null;

	// A reference too the data being saved in this class.
	private PlayerData _data = null;

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

		_data = _dataReferences.FindElement<PlayerData>(_id);
		if (_data == null) {
			_data = _dataReferences.AddElement<PlayerData>(_id);
			return false;
		}

		LoadPosition();
		LoadHealth();

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

	public void SaveHealth()
	{
		_data.Health = _health;
		_data.Save();
	}
	public void LoadHealth()
	{
		_health = _data.Health;
	}

	private IEnumerator ContinuousSave()
	{
		while (true)
		{
			yield return new WaitForSeconds(.5f);

			SavePosition();
		}
	}

	#endregion

	[SerializeField] private GameObject _bulletPrefab;

	private Vector3 _position;
	private int _health;

	private void Start()
	{
		if(!Setup(_id))
			return;

		transform.position = _position;
	}

    private void Update()
    {
		_position = transform.position;

		if (Input.GetKeyDown(KeyCode.Mouse0))
			Shoot();
	}

	private void Shoot() 
	{
		GameObject t_bullet = Instantiate(_bulletPrefab);

		t_bullet.transform.position = transform.position + transform.forward;
		t_bullet.transform.rotation = transform.rotation;
	}
}