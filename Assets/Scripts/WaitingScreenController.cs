using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingScreenController : MonoBehaviour
{
    [SerializeField]
    private GameObject _battleRoyaleText;
    [SerializeField]
    private GameObject _skyWarsText;
    
    void OnEnable()
    {
        _battleRoyaleText.SetActive(DataKeeper.IsBattleRoyaleClick);
        _skyWarsText.SetActive(DataKeeper.IsSkyWarsClick);
    }
}
