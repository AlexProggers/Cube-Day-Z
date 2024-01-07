using System.Globalization;
using UnityEngine;

public static class ParseUtils
{
	public static Vector2 Vector2FromString(string str)
	{
		if (str == null)
		{
			Debug.LogWarning("Vector2 parse failed because string is null, setting default value");
			return new Vector2(66f, 66f);
		}
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0], CultureInfo.InvariantCulture);
		float y = float.Parse(array[1], CultureInfo.InvariantCulture);
		return new Vector2(x, y);
	}

	public static Vector3 Vector3FromString(string str)
	{
		if (str == null)
		{
			Debug.LogWarning("Vector3 parse failed because string is null, setting default value");
			return new Vector3(66f, 66f, 66f);
		}
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0], CultureInfo.InvariantCulture);
		float y = float.Parse(array[1], CultureInfo.InvariantCulture);
		float z = float.Parse(array[2], CultureInfo.InvariantCulture);
		return new Vector3(x, y, z);
	}

	public static Vector4 Vector4FromString(string str)
	{
		if (str == null)
		{
			Debug.LogWarning("Vector4 parse failed because string is null, setting default value");
			return new Vector4(66f, 66f, 66f, 66f);
		}
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0], CultureInfo.InvariantCulture);
		float y = float.Parse(array[1], CultureInfo.InvariantCulture);
		float z = float.Parse(array[2], CultureInfo.InvariantCulture);
		float w = float.Parse(array[3], CultureInfo.InvariantCulture);
		return new Vector4(x, y, z, w);
	}
}
