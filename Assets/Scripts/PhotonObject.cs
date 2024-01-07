using System.Collections.Generic;
using UnityEngine;


public class PhotonObject : MonoBehaviour
{
	[SerializeField]
	private string _worldObjId;

	[SerializeField]
	private GameObject _normalModel;

	[SerializeField]
	private GameObject _destructedModel;

	[SerializeField]
	private WorldObjectActionType _actionType;

	private DestructibleObject _info;

	private Vector3 _destructedModelBoundsSize;

	private short _index;

	private int _currentHp;

	public WorldObjectActionType ActionType
	{
		get
		{
			return _actionType;
		}
	}

	public DestructibleObject Info
	{
		get
		{
			return _info;
		}
	}

	public Vector3 DestructedModelBoundsSize
	{
		get
		{
			return _destructedModelBoundsSize;
		}
	}

	public Animation Animation { get; private set; }

	public bool IsActivated { get; set; }

	public int CurrentHp
	{
		get
		{
			return _currentHp;
		}
		set
		{
			_currentHp = value;
			if (_currentHp <= 0 && _normalModel.activeSelf)
			{
				_normalModel.SetActive(false);
				if (_destructedModel != null)
				{
					_destructedModel.SetActive(true);
					Renderer componentInChildren = _destructedModel.GetComponentInChildren<Renderer>();
					_destructedModelBoundsSize = ((!componentInChildren) ? Vector3.zero : componentInChildren.bounds.size);
				}
				else
				{
					_destructedModelBoundsSize = Vector3.zero;
				}
			}
		}
	}

	private void Start()
	{
		_info = DataKeeper.Info.GetDestructibleObjectInfo(_worldObjId);
		if (_info != null)
		{
			Animation = _normalModel.GetComponent<Animation>();
			_index = (short)WorldController.I.ObjectsManager.Objects.Count;
			WorldController.I.ObjectsManager.Objects.Add(this);
			CurrentHp = _info.HealthPoint;
		}
	}

	public void Hit(short damage, List<DestructibleObjectType> type)
	{
		if (_info.Type == DestructibleObjectType.Other || type.Contains(_info.Type))
		{
			WorldController.I.ObjectsManager.Hit(_index, damage);
		}
	}

	public void Use()
	{
		WorldObjectActionType actionType = ActionType;
		if (actionType == WorldObjectActionType.Activate)
		{
			WorldController.I.ObjectsManager.Activate(_index, !IsActivated);
		}
	}
}
