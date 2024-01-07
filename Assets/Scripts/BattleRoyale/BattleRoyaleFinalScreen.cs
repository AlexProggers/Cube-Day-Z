using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleFinalScreen : MonoBehaviour
	{
		[SerializeField]
		private tk2dTextMesh _message;

		[SerializeField]
		private tk2dTextMesh _allPlrCount;

		private void OnEnable()
		{
			int myFinalPlace = BattleRoyaleGameManager.I.MyFinalPlace;
			if (BattleRoyaleGameManager.I != null)
			{
				string text = "Повезет в следующий раз!";
				if (DataKeeper.Language == Language.English)
				{
					text = "Better luck next time!";
				}
				if (myFinalPlace == 1)
				{
					text = "Грандиозная победа!";
					if (DataKeeper.Language == Language.English)
					{
						text = "Winner winner chicken dinner!";
					}
				}
				if (DataKeeper.Language == Language.Russian)
				{
					_message.text = "^CFFFFFFFFТы занял " + myFinalPlace + " место из " + BattleRoyaleGameManager.I.AllPlayersOnStart + ". \n^CFFBB38FF" + text;
				}
				else
				{
					_message.text = "^CFFFFFFFFYou made it to #" + myFinalPlace + " / " + BattleRoyaleGameManager.I.AllPlayersOnStart + ".\n^CFFBB38FF" + text;
				}
			}
		}
	}
}
