using UnityEngine;
using System.Collections.Generic;
using DataManagement;

public class ItemData : DataElement {

	public Vector3 Position { get => _position; set => _position = value; }
	[SerializeField] private Vector3 _position;
	public Quaternion Rotation { get => _rotation; set => _rotation = value; }
    [SerializeField] private Quaternion _rotation;
	public bool IsAlive { get => _isAlive; set => _isAlive = value; }
	[SerializeField] private bool _isAlive;

	public ItemData(string p_id) : base(p_id)
	{
		ID = p_id;
	}
}