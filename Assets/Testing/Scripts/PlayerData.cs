using UnityEngine;
using System.Collections.Generic;
using DataManagement;

public class PlayerData : DataElement {

	public Vector3 Position { get => _position; set => _position = value; }
	[SerializeField] private Vector3 _position;
	public int Health { get => _health; set => _health = value; }
	[SerializeField] private int _health;

	public PlayerData(string p_id) : base(p_id)
	{
		ID = p_id;
	}
}