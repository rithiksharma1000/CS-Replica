using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour {

    public Text healthText;

    private Player player;

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject scoreBoard;

    [SerializeField] Text ammoText;

    private WeaponManager weaponManager;

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
        weaponManager = player.GetComponent<WeaponManager>();
    }
    private void Update()
    {
        SetHealthText(player.GetCurrentHealth());
        SetAmmoText(weaponManager.GetCurrentWeapon().bullets);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }

    

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    public void SetHealthText(int _currhealth)
    {
        healthText.text = _currhealth.ToString();
    }

    void SetAmmoText(int _amount)
    {
        ammoText.text = _amount.ToString();
    }
}
