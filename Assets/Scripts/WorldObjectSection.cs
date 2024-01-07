using System.Collections.Generic;
using UnityEngine;

public class WorldObjectSection : MonoBehaviour
{
	[SerializeField]
	private string _plantPrefabName;

	[SerializeField]
	private Vector2 _sectionArea;

	[SerializeField]
	private float _xOffset;

	[SerializeField]
	private float _yOffset;

	[SerializeField]
	private float _zOffset;

	[SerializeField]
	private int _rowsCount;

	[SerializeField]
	private int _columnCount;

	[SerializeField]
	private bool _setByHand;

	[SerializeField]
	private List<Transform> _itemsSections;

	private List<Vector3> _sections = new List<Vector3>();

	public int SectionsCount
	{
		get
		{
			return _rowsCount * _columnCount;
		}
	}

	public string PlantPrefabName
	{
		get
		{
			return _plantPrefabName;
		}
	}

	private void Awake()
	{
		InitializeSections();
	}

	private void InitializeSections()
	{
		if (_setByHand)
		{
			return;
		}
		float num = _sectionArea.x / (float)_columnCount;
		float num2 = _sectionArea.y / (float)_rowsCount;
		for (int i = 0; i < _columnCount; i++)
		{
			for (int j = 0; j < _rowsCount; j++)
			{
				float x = _xOffset + (float)i * num - _sectionArea.x / 2f + num / 2f;
				float yOffset = _yOffset;
				float z = _zOffset + (float)j * num2 - _sectionArea.y / 2f + num2 / 2f;
				_sections.Add(new Vector3(x, yOffset, z));
			}
		}
	}

	public Vector3? GetSection(int index)
	{
		if (_setByHand)
		{
			if (index < _itemsSections.Count)
			{
				return _itemsSections[index].position;
			}
		}
		else if (index < _sections.Count)
		{
			return _sections[index];
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
		Gizmos.DrawCube(new Vector3(base.transform.position.x + _xOffset, base.transform.position.y + _yOffset, base.transform.position.z + _zOffset), new Vector3(_sectionArea.x, 0.01f, _sectionArea.y));
		float num = _sectionArea.x / (float)_columnCount;
		float num2 = _sectionArea.y / (float)_rowsCount;
		Gizmos.color = new Color(0f, 1f, 0f, 1f);
		for (int i = 1; i < _columnCount; i++)
		{
			Vector3 vector = new Vector3(base.transform.position.x + _xOffset + num * (float)i - _sectionArea.x / 2f, base.transform.position.y + _yOffset, base.transform.position.z + _zOffset - _sectionArea.y / 2f);
			Vector3 to = vector + new Vector3(0f, 0f, _sectionArea.y);
			Gizmos.DrawLine(vector, to);
		}
		for (int j = 1; j < _rowsCount; j++)
		{
			Vector3 vector2 = new Vector3(base.transform.position.x + _xOffset - _sectionArea.x / 2f, base.transform.position.y + _yOffset, base.transform.position.z + _zOffset + num2 * (float)j - _sectionArea.y / 2f);
			Vector3 to2 = vector2 + new Vector3(_sectionArea.x, 0f, 0f);
			Gizmos.DrawLine(vector2, to2);
		}
		for (int k = 0; k < _columnCount; k++)
		{
			for (int l = 0; l < _rowsCount; l++)
			{
				float x = base.transform.position.x + _xOffset + (float)k * num - _sectionArea.x / 2f + num / 2f;
				float y = base.transform.position.y + _yOffset;
				float z = base.transform.position.z + _zOffset + (float)l * num2 - _sectionArea.y / 2f + num2 / 2f;
				Gizmos.DrawSphere(new Vector3(x, y, z), 0.01f);
			}
		}
	}
}
