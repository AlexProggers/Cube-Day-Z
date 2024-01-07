using UnityEngine;

public class RagdollViewController : MonoBehaviour
{
	[SerializeField]
	private Renderer _character;

	public void SetView(Material material)
	{
		_character.material.CopyPropertiesFromMaterial(material);
	}
}
