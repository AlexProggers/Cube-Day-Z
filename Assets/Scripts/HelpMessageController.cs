using System.Collections.Generic;
using UnityEngine;


public enum HelpMessageType
{
	None = 0,
	PlaceItem = 1,
	Activate = 2,
	Plant = 3,
	Use = 4,
	Storage = 5,
	PickUp = 6
}

[System.Serializable]
public class HelpMessage
{
	public HelpMessageType Type;

	public string Message;

	public string RussianMessage;
}


public class HelpMessageController : MonoBehaviour
{
	public static HelpMessageController I;

	[SerializeField]
	private List<HelpMessage> _helpMessages;

	[SerializeField]
	private tk2dTextMesh _message;

	[SerializeField]
	private tk2dTextMesh _itemName;

	public HelpMessageType CurrentMessageType { get; private set; }

	private void Awake()
	{
		I = this;
	}

	public void SetObjName(string itemName, bool isPlayerNick = false)
	{
		if (isPlayerNick)
		{
			if (itemName.Contains("CFFC300FF VIP"))
			{
				string[] array = itemName.Split('^');
				string text = "^C35AE17FF" + array[0];
				itemName = text + "^" + array[1];
			}
			else
			{
				_itemName.color = Color.green;
			}
		}
		else
		{
			_itemName.color = Color.white;
		}
		_itemName.text = itemName;
	}

	public void ShowMessage(HelpMessageType type)
	{
		if (CurrentMessageType == type)
		{
			return;
		}
		if (type != 0)
		{
			List<HelpMessage> list = _helpMessages.FindAll((HelpMessage m) => m.Type == type);
			if (list.Count > 0)
			{
				CurrentMessageType = type;
				switch (DataKeeper.Language)
				{
				case Language.Russian:
					_message.text = list[UnityEngine.Random.Range(0, list.Count)].RussianMessage;
					break;
				case Language.English:
					_message.text = list[UnityEngine.Random.Range(0, list.Count)].Message;
					break;
				default:
					_message.text = list[UnityEngine.Random.Range(0, list.Count)].Message;
					break;
				}
			}
			else
			{
				Debug.Log("No messages of this type");
				Hide();
			}
		}
		else
		{
			Hide();
		}
	}

	public void Hide()
	{
		CurrentMessageType = HelpMessageType.None;
		_message.text = string.Empty;
	}
}
