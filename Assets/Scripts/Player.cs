using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {


    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField] private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SyncVar]
    public string playerNick;

    [SyncVar] public int kills;
    [SyncVar] public int deaths;


    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField] private GameObject deathUI;
    private GameObject deathui;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        //Disabling the death UI
        if(!firstSetup)
            deathui.SetActive(false);

        CmdBroadCastNewPlayerSetup();
        
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            deathui = Instantiate(deathUI);
            deathui.SetActive(false);          //Disabling the death UI.
            firstSetup = false;
        }

        SetDefaults();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        //if(Input.GetKeyDown(KeyCode.K))
        //{
        //    RpcTakeDamage(101);
       // }
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount, string _sourcePlayerID)
    {
        if (isDead) return;

        currentHealth -= _amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, 100);

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0)
        {
            Die(_sourcePlayerID);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void Die(string _sourcePlayerID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourcePlayerID);
        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;       //Incease kills count
            GameManager.instance.onPlayerKilledCallback.Invoke(transform.name, sourcePlayer.transform.name);
        }

        //Increase deaths count
        deaths++;

        //Disable Components

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;
        Debug.Log(transform.name + " is DEAD !!");

        //Enabling the death UI
        if(isLocalPlayer)
            deathui.SetActive(true);

        //Call Respawn Method
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.reapawnTime);
        
        SetDefaults();

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

    }
}
