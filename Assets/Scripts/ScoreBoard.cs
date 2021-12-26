using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    [SerializeField]
	GameObject playerScoreboardItem;

	[SerializeField]
	Transform playerScoreboardList;

	void OnEnable ()
	{
		Player[] players = GameManager.GetAllPlayers();

		foreach (Player player in players)
		{
			GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
			PlayerScoreElement item = itemGO.GetComponent<PlayerScoreElement>();
			if (item != null)
			{
				item.Set(player.transform.name, player.kills, player.deaths);
			}
		}
	}

	void OnDisable ()
	{
		foreach (Transform child in playerScoreboardList)
		{
			Destroy(child.gameObject);
		}
	}
}
