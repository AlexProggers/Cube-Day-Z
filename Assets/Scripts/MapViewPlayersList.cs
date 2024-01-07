using System.Collections;
using System.Text;
using UnityEngine;

public class MapViewPlayersList : MonoBehaviour
{
	[SerializeField]
	private GameObject _playersListObj;

	[SerializeField]
	private tk2dTextMesh _playersListLabel;

	[SerializeField]
	private tk2dTextMesh _playersListLabelShadow;

	[SerializeField]
	private tk2dUIScrollableArea _playerListArea;

	[SerializeField]
	private float _onePlayerRecordSize = 0.15f;

	[SerializeField]
	private int _playerListScrollSpeed = 20;

	[SerializeField]
	private ShadowText _roomInfo;

	private void OnEnable()
	{
		if (!PhotonNetwork.offlineMode)
		{
			_playersListObj.SetActive(true);
			StartCoroutine("UpdatePlayerList");
		}
		else
		{
			_playersListObj.SetActive(false);
		}
	}

	private void OnDisable()
	{
		StopCoroutine("UpdatePlayerList");
	}

	private IEnumerator UpdatePlayerList()
	{
		while (true)
		{
			SetLabel();
			yield return new WaitForSeconds(3f);
		}
	}

	private void SetLabel()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		PhotonPlayer[] playerList = PhotonNetwork.playerList;

		for (int i = 0; i < playerList.Length; i++)
		{
			stringBuilder.Append("^CF6BB38FF").Append(1 + i).Append(". ^CFFFFFFFF")
				.Append(playerList[i].NickName)
				.Append("\n");
			stringBuilder2.Append(1 + i).Append(". ").Append(playerList[i].NickName)
				.Append("\n");
		}
		if (PhotonNetwork.room != null)
		{
			string text = PhotonNetwork.room.Name + "\n";
			bool flag = false;
			if (PhotonNetwork.room.CustomProperties.ContainsKey("zombies"))
			{
				flag = (bool)PhotonNetwork.room.CustomProperties["zombies"];
			}
			text = ((DataKeeper.Language != 0) ? (text + "World type: " + ((!flag) ? "Without zombies" : "With zombies")) : (text + "Тип мира: " + ((!flag) ? "Без зомби" : "С зомби")));
			_roomInfo.SetText(text);
		}
		else
		{
			_roomInfo.SetText(string.Empty);
		}
		_playersListLabel.text = stringBuilder.ToString();
		_playersListLabelShadow.text = stringBuilder2.ToString();
		_playerListArea.ContentLength = _onePlayerRecordSize * (float)playerList.Length;
		_playerListArea.scrollBar.buttonUpDownScrollDistance = 1f / (float)playerList.Length * (float)_playerListScrollSpeed;
	}
}
