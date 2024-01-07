using BattleRoyaleFramework;
using System.Collections;
using UnityEngine;

public class CommonInfoController : MonoBehaviour {
	
	[SerializeField]
	private tk2dTextMesh _playersLabel;

	[SerializeField]
	private tk2dTextMesh _battleRoyaleLabel;

	[SerializeField]
	private tk2dTextMesh _skywarsLabel;

    void FixedUpdate()
    {
        if (DataKeeper.Language == Language.Russian)
        {
            _playersLabel.text = "Игроков онлайн: " + PhotonNetwork.countOfPlayers;
            _battleRoyaleLabel.text = "Голодные игры: " + BRFConnection.GetPlayersCountInGameMode(GameType.BattleRoyale);
            _skywarsLabel.text = "Sky Wars: " + BRFConnection.GetPlayersCountInGameMode(GameType.SkyWars);
        }
        else
        {
            _playersLabel.text = "Players online: " + PhotonNetwork.countOfPlayers;
            _battleRoyaleLabel.text = "Battle Royale: " + BRFConnection.GetPlayersCountInGameMode(GameType.BattleRoyale);
            _skywarsLabel.text = "Sky Wars: " + BRFConnection.GetPlayersCountInGameMode(GameType.SkyWars);
        }
    }
}
