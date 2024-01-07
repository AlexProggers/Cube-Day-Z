using UnityEngine;

public abstract class PlayerChangeSectorListener : MonoBehaviour
{
	public static PlayerChangeSectorListener I;

	private void Awake()
	{
		I = this;
	}

	public virtual void OnPlayerSectorChange(PhotonMan player, PhotonSectorInfo oldSector, PhotonSectorInfo newSector)
	{
	}
}
