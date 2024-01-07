using Photon;

public enum WorldObjectActionType
{
	None = 0,
	Farming = 1,
	Activate = 2,
	Storage = 3
}

public class BaseWorldObjectAction : MonoBehaviour
{
	public virtual bool CanUse
	{
		get
		{
			return true;
		}
	}

	public virtual void Use()
	{
	}

	public virtual void OnHit()
	{
	}

	public virtual void OnObjectDestroy()
	{
	}
}
