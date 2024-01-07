using System.Collections.Generic;
using UnityEngine;

public class PhotonSectorService : MonoBehaviour
{
	public static PhotonSectorService I;

	[SerializeField]
	private Vector2 _area;

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
	private bool _drawSphereInFirstSector;

	private PhotonSectorInfo[,] _sectors;

	private Vector2 _size;

	private Vector3 _transformPosition;

	private void Awake()
	{
		I = this;
		InitializeSectors();
	}

	private void InitializeSectors()
	{
		_sectors = new PhotonSectorInfo[_rowsCount, _columnCount];
		float num = _area.x / (float)_columnCount;
		float num2 = _area.y / (float)_rowsCount;
		_transformPosition = base.transform.position;
		_size = new Vector2(num, num2);
		for (byte b = 0; b < _rowsCount; b++)
		{
			for (byte b2 = 0; b2 < _columnCount; b2++)
			{
				float x = _transformPosition.x + _xOffset + (float)(int)b2 * num - _area.x / 2f + num / 2f;
				float y = _transformPosition.y + _yOffset;
				float z = _transformPosition.z + _zOffset + (float)(int)b * num2 - _area.y / 2f + num2 / 2f;
				_sectors[b, b2] = new PhotonSectorInfo(new Vector3(x, y, z), b, b2);
			}
		}
		PhotonSectorInfo[,] sectors = _sectors;
		int length = sectors.GetLength(0);
		int length2 = sectors.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				PhotonSectorInfo photonSectorInfo = sectors[i, j];
				photonSectorInfo.SetNeighbors(GetNeighbors(photonSectorInfo));
			}
		}
	}

	public void InitializeSpawnInfo(List<PhotonSpawnMobInfo> spawnInfos)
	{
		foreach (PhotonSpawnMobInfo spawnInfo in spawnInfos)
		{
			PhotonSectorInfo sector = GetSector(spawnInfo.Position);
			if (sector != null)
			{
				sector.AddSpawnInfo(spawnInfo);
			}
		}
	}

	public PhotonSectorInfo GetSector(Vector3 position)
	{
		byte b = (byte)((position.z - (_transformPosition.z + _zOffset - _area.y / 2f)) / _size.y);
		byte b2 = (byte)((position.x - (_transformPosition.x + _xOffset - _area.x / 2f)) / _size.x);
		if (b < _rowsCount && b2 < _columnCount)
		{
			return _sectors[b, b2];
		}
		return null;
	}

	public PhotonSectorInfo GetSector(byte row, byte column)
	{
		if (row < _rowsCount && column < _columnCount)
		{
			return _sectors[row, column];
		}
		return null;
	}

	public void OnPlayerChangeSector(PhotonSectorInfo oldSector, PhotonSectorInfo newSector, bool playerIsMine)
	{
		List<PhotonSectorInfo> sectorWithNeighbors = GetSectorWithNeighbors(oldSector);
		List<PhotonSectorInfo> sectorWithNeighbors2 = GetSectorWithNeighbors(newSector);
		foreach (PhotonSectorInfo item in sectorWithNeighbors2)
		{
			if (!sectorWithNeighbors.Contains(item))
			{
				sectorWithNeighbors.Add(item);
			}
			else
			{
				sectorWithNeighbors.Remove(item);
			}
		}
		foreach (PhotonSectorInfo item2 in sectorWithNeighbors)
		{
			item2.Check(playerIsMine);
		}
	}

	public static bool AreSectorsNeighbors(PhotonSectorInfo sector1, PhotonSectorInfo sector2)
	{
		if (sector1 != null && sector2 != null)
		{
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					int num = sector1.RowIndex + i;
					int num2 = sector1.ColumnIndex + j;
					if (sector2.RowIndex == num && sector2.ColumnIndex == num2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool AreSectorsNeighbors(int row1, int column1, int row2, int column2)
	{
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				int num = row1 + i;
				int num2 = column1 + j;
				if (row2 == num && column2 == num2)
				{
					return true;
				}
			}
		}
		return false;
	}

	private List<PhotonSectorInfo> GetSectorWithNeighbors(PhotonSectorInfo sector)
	{
		List<PhotonSectorInfo> list = new List<PhotonSectorInfo>();
		if (sector != null)
		{
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					int num = sector.RowIndex + i;
					int num2 = sector.ColumnIndex + j;
					if (num >= 0 && num < _rowsCount && num2 >= 0 && num2 < _columnCount)
					{
						list.Add(GetSector((byte)num, (byte)num2));
					}
				}
			}
		}
		return list;
	}

	private List<PhotonSectorInfo> GetNeighbors(PhotonSectorInfo sector)
	{
		List<PhotonSectorInfo> list = new List<PhotonSectorInfo>();
		if (sector != null)
		{
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						int num = sector.RowIndex + i;
						int num2 = sector.ColumnIndex + j;
						if (num >= 0 && num < _rowsCount && num2 >= 0 && num2 < _columnCount)
						{
							list.Add(GetSector((byte)num, (byte)num2));
						}
					}
				}
			}
		}
		return list;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.85f, 0f, 0.2f);
		Gizmos.DrawCube(new Vector3(base.transform.position.x + _xOffset, base.transform.position.y + _yOffset, base.transform.position.z + _zOffset), new Vector3(_area.x, 0.01f, _area.y));
		float num = _area.x / (float)_columnCount;
		float num2 = _area.y / (float)_rowsCount;
		Gizmos.color = new Color(1f, 1f, 1f, 1f);
		for (int i = 1; i < _columnCount; i++)
		{
			Vector3 vector = new Vector3(base.transform.position.x + _xOffset + num * (float)i - _area.x / 2f, base.transform.position.y + _yOffset, base.transform.position.z + _zOffset - _area.y / 2f);
			Vector3 to = vector + new Vector3(0f, 0f, _area.y);
			Gizmos.DrawLine(vector, to);
		}
		for (int j = 1; j < _rowsCount; j++)
		{
			Vector3 vector2 = new Vector3(base.transform.position.x + _xOffset - _area.x / 2f, base.transform.position.y + _yOffset, base.transform.position.z + _zOffset + num2 * (float)j - _area.y / 2f);
			Vector3 to2 = vector2 + new Vector3(_area.x, 0f, 0f);
			Gizmos.DrawLine(vector2, to2);
		}
		if (_drawSphereInFirstSector)
		{
			float x = base.transform.position.x + _xOffset - _area.x / 2f + num / 2f;
			float y = base.transform.position.y + _yOffset;
			float z = base.transform.position.z + _zOffset - _area.y / 2f + num2 / 2f;
			Gizmos.DrawSphere(new Vector3(x, y, z), num2 / 2f);
		}
	}
}
