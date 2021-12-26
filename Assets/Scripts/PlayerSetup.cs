using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDisable;

    [SerializeField] string remoteLayerName = "RemotePlayer";

    [SerializeField] string dontDrawLayerName = " DontDraw";
    [SerializeField] GameObject playerGraphics;

    [SerializeField] GameObject playerUIPrefab;
    public GameObject playerUIInstance;

    Camera sceneCamera;

    void Start()
    {
        //Disable the components that should only be
        //active on the player that we control
        if(!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }else
        {
            //We are the local player : disable the scene camera
            sceneCamera = Camera.main;
            if ( sceneCamera != null )
            {
                sceneCamera.gameObject.SetActive(false);
            }

            //Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //Configure PlayerUI 
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.Log("No PlayerUI component on PlayerUI prefab.");
            ui.SetPlayer(GetComponent<Player>());

            GetComponent<Player>().SetupPlayer();
        }

       
    }

    [Command]
    void CmdSetNick(string playerID, string nick)
    {
        Player player = GameManager.GetPlayer(playerID);
        if(player != null)
        {
            Debug.Log(nick + " has joined !");
            player.playerNick = nick;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
        
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisable()
    {
        Destroy(playerUIInstance);

        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }


}
