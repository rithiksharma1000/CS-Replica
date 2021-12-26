using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour
{

    List<GameObject> serverList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject serverListItemPrefab;

    [SerializeField]
    private Transform serverListParent;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshServerList();
    }

    public void RefreshServerList()
    {
        ClearServerList();

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading server list...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";

        if (!success || matchList == null)
        {
            status.text = "Couldn't get server list.";
            return;
        }

        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject _serverListItemGO = Instantiate(serverListItemPrefab);
            _serverListItemGO.transform.SetParent(serverListParent);

            ServerListItem _serverListItem = _serverListItemGO.GetComponent<ServerListItem>();
            if (_serverListItem != null)
            {
                _serverListItem.Setup(match, JoinRoom);
            }


            // as well as setting up a callback function that will join the game.

            serverList.Add(_serverListItemGO);
        }

        if (serverList.Count == 0)
        {
            status.text = "No servers at the moment.";
        }
    }

    void ClearServerList()
    {
        for (int i = 0; i < serverList.Count; i++)
        {
            Destroy(serverList[i]);
        }

        serverList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        StartCoroutine(WaitForJoin());
    }

    IEnumerator WaitForJoin()
    {
        ClearServerList();

        int countdown = 20;
        while (countdown > 0)
        {
            status.text = "JOINING... (" + countdown + ")";

            yield return new WaitForSeconds(1);

            countdown--;
        }

        // Failed to connect
        status.text = "Failed to connect.";
        yield return new WaitForSeconds(1);

        MatchInfo matchInfo = networkManager.matchInfo;
        if (matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }

        RefreshServerList();

    }

}