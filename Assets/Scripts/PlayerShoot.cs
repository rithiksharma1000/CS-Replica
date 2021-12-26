using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player"; 

    [SerializeField] private Camera cam;

    [SerializeField] private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot : No camera refenced !!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.isOn == true)
            return;

        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetButtonDown("Reload"))
            {
                weaponManager.Reload();
                return;
            }
        }

        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= currentWeapon._nextTimeToFire)
            {
                currentWeapon._nextTimeToFire = Time.time + currentWeapon.nextTimeToFire ;
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    [Command] 
    void CmdOnShoot()        //Called on the server when a player shoots.
    {
        RpcShootEffect();
    }

    [ClientRpc]
    void RpcShootEffect()    //Called on all clients for shooting effects.
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        weaponManager.RecoilAnim();
    }

    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)      //Called when bullet hits something,
    {                                                 //taking point of impact and a vector    
        RpcHitEffect(_pos,_normal);                   //normal to the surface as arguments.   
    }

    [ClientRpc]
    void RpcHitEffect(Vector3 _pos, Vector3 _normal)  //Called on all clients for bullet impacts.
    {
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 1f);
    }

    [Client]
    void Shoot()
    {
        if(!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        Debug.Log("Remaining bullets : " + currentWeapon.bullets);

        CmdOnShoot();

        RaycastHit _hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        { 
            // We hit something
            if(_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }

            CmdOnHit(_hit.point, _hit.normal);    //Calling for bullet impact effect 
        }                                         //on the impact point.

        if (currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
        }
    }

    [Command]
    void CmdPlayerShot(string _playerID, int _damage, string _sourcePlayerID)
    {
        Debug.Log(_playerID + " has been shot !");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage, _sourcePlayerID);
    }
}
