using Aubergine;
using UnityEngine;

public enum HallucinogenType
{
	None = 0,
	OrangeApple = 1,
	GreenApple = 2,
	RedApple = 3,
	PurpleApple = 4,
	BlueApple = 5,
	PinkApple = 6
}

public class HallucinogenController : MonoBehaviour
{
	public static HallucinogenController I;

	[SerializeField]
	private PP_Contours _contours;

	[SerializeField]
	private PP_SinCity _mySinCity;

	[SerializeField]
	private PP_SobelEdge _sobelEdge;

	[SerializeField]
	private PP_Waves _myWaves;

	[SerializeField]
	private PP_LightWave _myLightWave;

	[SerializeField]
	private PP_Displacement _myDisplacement;

	[SerializeField]
	private PP_Godrays _godrays;

	[SerializeField]
	private PP_Negative _myNegative;

	private HallucinogenType _currentEffect;

	private Color _curvedEffectFogColor = new Color(0.63f, 0.45f, 0.079f);

	private void Awake()
	{
		I = this;
	}

	public void EnableHallucinogen(HallucinogenType type)
	{
		DisableHallucinogen();
		switch (type)
		{
		case HallucinogenType.RedApple:
			_contours.enabled = true;
			_mySinCity.enabled = true;
			break;
		case HallucinogenType.GreenApple:
			_sobelEdge.enabled = true;
			break;
		case HallucinogenType.OrangeApple:
			_myWaves.enabled = true;
			break;
		case HallucinogenType.PurpleApple:
			_myLightWave.enabled = true;
			break;
		case HallucinogenType.BlueApple:
			_myDisplacement.enabled = true;
			_godrays.enabled = true;
			break;
		case HallucinogenType.PinkApple:
			_sobelEdge.enabled = true;
			_myNegative.enabled = true;
			break;
		}
		if (type != 0)
		{
			_currentEffect = type;
		}
	}

	public void DisableHallucinogen()
	{
		switch (_currentEffect)
		{
		case HallucinogenType.RedApple:
			_contours.enabled = false;
			_mySinCity.enabled = false;
			break;
		case HallucinogenType.GreenApple:
			_sobelEdge.enabled = false;
			break;
		case HallucinogenType.OrangeApple:
			_myWaves.enabled = false;
			break;
		case HallucinogenType.PurpleApple:
			_myLightWave.enabled = false;
			break;
		case HallucinogenType.BlueApple:
			_myDisplacement.enabled = false;
			_godrays.enabled = false;
			break;
		case HallucinogenType.PinkApple:
			_sobelEdge.enabled = false;
			_myNegative.enabled = false;
			break;
		}
		_currentEffect = HallucinogenType.None;
	}
}
