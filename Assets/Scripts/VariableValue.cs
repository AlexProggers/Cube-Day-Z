using System;
using UnityEngine;

[Serializable]
public class VariableValue
{
	public float Min;

	public float Max;

	public int GetValueInt()
	{
		return (int)UnityEngine.Random.Range(Min, Max + 1f);
	}

	public float GetValueFloat()
	{
		return UnityEngine.Random.Range(Min, Max);
	}
}

[Serializable]
public class KeyValueInt
{
	public string Key;

	public int Value;
}
