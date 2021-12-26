using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public MatchSettings matchSettings;

    public delegate void OnPlayerKilledCallback(string player, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple GameManagers in the scene !");
        }
        else
        {
            instance = this;
        }
    }

   /* public void AddPlayerScoreElement(string _playerName)
    {
        GameObject playerscoreElement = Instantiate(scoreBoard.PlayerScoreElementPrefab, scoreBoard.playerScoreElementParent);
        PlayerScoreElement _playerScoreElement = playerscoreElement.GetComponent<PlayerScoreElement>();

        if (_playerScoreElement != null)
        {
            _playerScoreElement.Set(_playerName, 0, 0);
        }

        scoreBoard.playerScoreList.Add(playerscoreElement);
    }*/
    
    #region Player tracking 

    private const string PLAYER_ID_PREFIX = "Player";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
        _player.kills = 0; _player.deaths = 0;
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static Player GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }

    //void OnGUI ()
    //{
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();
    //
    //    foreach (string _playerID in players.Keys)
    //    {
    //        GUILayout.Label(_playerID + "  -  " + players[_playerID].transform.name);
    //   }
    //
    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //}

    #endregion
}
