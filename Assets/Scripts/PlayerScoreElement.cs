using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreElement : MonoBehaviour {

    [SerializeField] Text PlayerNameText;

    [SerializeField] Text killsText;

    [SerializeField] Text deathsText;

    public void Set(string _playerName,int  _kills, int _deaths )
    {
        
        PlayerNameText.text = _playerName;
        killsText.text = _kills.ToString();
        deathsText.text = _deaths.ToString();
    }

}
