﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class ServerListItem : MonoBehaviour
{

    public delegate void JoinServerDelegate(MatchInfoSnapshot _match);
    private JoinServerDelegate joinServerCallback;

    [SerializeField]
    private Text serverNameText;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinServerDelegate _joinServerCallback)
    {
        match = _match;
        joinServerCallback = _joinServerCallback;

        serverNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinServer()
    {
        joinServerCallback.Invoke(match);
    }

}