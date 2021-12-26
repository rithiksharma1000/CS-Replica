using UnityEngine;

[System.Serializable]

public class PlayerWeapon
{
    public string name = "Glock";

    public int damage = 10;
    public float range = 100f;

    public float fireRate = 0f; //In bullets per second.

    public float nextTimeToFire = 0.5f;

    [HideInInspector] public float _nextTimeToFire;

    public int maxBullets = 20;
    [HideInInspector]
    public int bullets;

    public float reloadTime = 1f;

    public GameObject graphics;  //Weapon Graphics.

    public PlayerWeapon()
    {
        bullets = maxBullets;
        _nextTimeToFire = nextTimeToFire;
    }

}
