using BattleRoyale;
using UnityEngine;

public class MapViewController : MonoBehaviour
{
	[SerializeField]
	private Vector2 _mapSize;

	[SerializeField]
	private int _rowsCount;

	[SerializeField]
	private int _columnCount;

	[SerializeField]
	private GameObject _mySectorView;

	private Vector2[,] _sectorsPosition;

	private bool _isInitialized;

	[SerializeField]
	private Transform _leftFogSide;

	[SerializeField]
	private Transform _rightFogSide;

	[SerializeField]
	private Transform _topFogSide;

	[SerializeField]
	private Transform _bottomFogSide;

	[SerializeField]
	private Transform _safetyLeftFogSide;

	[SerializeField]
	private Transform _safetyRightFogSide;

	[SerializeField]
	private Transform _safetyTopFogSide;

	[SerializeField]
	private Transform _safetyBottomFogSide;

	[SerializeField]
	private Transform _safetyZoneArea;

	[SerializeField]
	private Renderer[] _fogRenderes;

	public static MapViewController I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
		ToggleFog();
	}

	private void OnEnable()
	{
		SynchFogArea();
		CalculateSafetyFogArea();
	}

	private void Initizlie()
	{
		_sectorsPosition = new Vector2[_rowsCount, _columnCount];
		float num = _mapSize.x / (float)_columnCount;
		float num2 = _mapSize.y / (float)_rowsCount;
		for (byte b = 0; b < _rowsCount; b++)
		{
			for (byte b2 = 0; b2 < _columnCount; b2++)
			{
				float x = (float)(int)b2 * num;
				float y = (float)(int)b * num2;
				_sectorsPosition[b, b2] = new Vector2(x, y);
			}
		}
		_isInitialized = true;
	}

	public void OnPlayerSectorChanged(int row, int column)
	{
		if (!_isInitialized)
		{
			Initizlie();
		}
		_mySectorView.transform.localPosition = _sectorsPosition[row, column];
	}

	public void SynchFogArea()
	{
		if (DataKeeper.GameType == GameType.BattleRoyale)
		{
			PhotonSectorInfo sector = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I.GetFogSidePos(FogSide.Left));
			PhotonSectorInfo sector2 = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I.GetFogSidePos(FogSide.Right));
			PhotonSectorInfo sector3 = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I.GetFogSidePos(FogSide.Top));
			PhotonSectorInfo sector4 = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I.GetFogSidePos(FogSide.Bottom));
			if (sector != null)
			{
				_leftFogSide.transform.localPosition = _sectorsPosition[sector.RowIndex, sector.ColumnIndex];
			}
			if (sector2 != null)
			{
				_rightFogSide.transform.localPosition = _sectorsPosition[sector2.RowIndex, sector2.ColumnIndex];
			}
			if (sector3 != null)
			{
				_topFogSide.transform.localPosition = _sectorsPosition[sector3.RowIndex, sector3.ColumnIndex];
			}
			if (sector4 != null)
			{
				_bottomFogSide.transform.localPosition = _sectorsPosition[sector4.RowIndex, sector4.ColumnIndex];
			}
			SwitchRenderers();
		}
	}

	public void CalculateSafetyFogArea()
	{
		if (DataKeeper.GameType == GameType.BattleRoyale)
		{
			PhotonSectorInfo sector = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I._nextLeftSide.position);
			if (sector != null)
			{
				_safetyLeftFogSide.transform.localPosition = _sectorsPosition[sector.RowIndex, sector.ColumnIndex];
			}
			PhotonSectorInfo sector2 = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I._nextRightSide.position);
			if (sector2 != null)
			{
				_safetyRightFogSide.transform.localPosition = _sectorsPosition[sector2.RowIndex, sector2.ColumnIndex];
			}
			PhotonSectorInfo sector3 = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I._nextTopSide.position);
			if (sector3 != null)
			{
				_safetyTopFogSide.transform.localPosition = _sectorsPosition[sector3.RowIndex, sector3.ColumnIndex];
			}
			PhotonSectorInfo sector4 = PhotonSectorService.I.GetSector(BattleRoyaleFogManager.I._nextBottomSide.position);
			if (sector4 != null)
			{
				_safetyBottomFogSide.transform.localPosition = _sectorsPosition[sector4.RowIndex, sector4.ColumnIndex];
			}
		}
	}

	private void ToggleFog()
	{
		bool active = false || DataKeeper.GameType == GameType.BattleRoyale;
		_leftFogSide.gameObject.SetActive(active);
		_rightFogSide.gameObject.SetActive(active);
		_topFogSide.gameObject.SetActive(active);
		_bottomFogSide.gameObject.SetActive(active);
		_safetyZoneArea.gameObject.SetActive(active);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.J))
		{
			CalcDistance();
		}
	}

	private void CalcDistance()
	{
		Debug.Log(string.Concat(_safetyLeftFogSide.transform.localPosition, " --", _safetyLeftFogSide.transform.localPosition));
		float num = Vector3.Distance(_safetyLeftFogSide.transform.position, _safetyRightFogSide.transform.position);
		Debug.Log(num);
	}

	private void SwitchRenderers()
	{
		bool flag = false;
		if (BattleRoyaleFogManager.I != null && BattleRoyaleFogManager.I.CanShowAreaOnMap)
		{
			flag = true;
		}
		for (int i = 0; i < _fogRenderes.Length; i++)
		{
			_fogRenderes[i].enabled = flag;
		}
	}
}
