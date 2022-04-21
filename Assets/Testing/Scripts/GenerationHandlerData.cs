using UnityEngine;
using System.Collections.Generic;
using DataManagement;

public class GenerationHandlerData : DataElement {

	public List<GenerationHandler.TerrainObject> Objects { get => _objects; set => _objects = value; }
	[SerializeField] private List<GenerationHandler.TerrainObject> _objects;

	public GenerationHandlerData(string p_id) : base(p_id)
	{
		ID = p_id;
	}
}