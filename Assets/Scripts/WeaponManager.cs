using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private string weaponLayerName = "Weapon";

    [SerializeField] private Transform weaponHolder;

    [SerializeField] private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    public bool isReloading = false;

    void Start ()
    {
        EquipWeapon(primaryWeapon);
	}

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()      //Weapon graphics
    {
        return currentGraphics;
    }

    void EquipWeapon (PlayerWeapon _weapon)
    { 
        currentWeapon = _weapon;

        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
            Debug.Log("No WeaponGraphics component on the weapon object : " + _weaponIns.name);

        if(isLocalPlayer)
        {
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    public void Reload()
    {
        if (isReloading)
            return;

        StartCoroutine(Reload_Coroutine());
    }

    private IEnumerator Reload_Coroutine()
    {
        Debug.Log("Reloading...");

        isReloading = true;

        CmdOnReload();

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = currentGraphics.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Reload");
            
            //anim.PlayQueued("shoot", QueueMode.CompleteOthers);
        }
        else Debug.Log("Null Anim");
    }

    public void RecoilAnim()
    {
        CmdRecoilAnim();
    }

    [Command]
    void CmdRecoilAnim()
    {
        RpcRecoilAnim();
    }

    [ClientRpc]
    void RpcRecoilAnim()
    {
        Animator recoilAnim = currentGraphics.GetComponent<Animator>();
        if (recoilAnim != null)
        {
            //recoilAnim.ResetTrigger("Shoot");
            //recoilAnim.GetComponent<Animator>().StopPlayback(); //this stops shooting animation from playing after reload animation.
            // recoilAnim.SetBool("Shoot",true);
            recoilAnim.CrossFadeInFixedTime("Shoot", 0.06667f);
        }
    }

    private void FixedUpdate()
    {
        Animator anim = currentGraphics.GetComponent<Animator>();
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Shoot")) anim.SetBool("Shoot",false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            Animator anim = currentGraphics.GetComponent<Animator>();
            anim.SetBool("IsMovingForward",true);
        }
        else
        {
            Animator anim = currentGraphics.GetComponent<Animator>();
            anim.SetBool("IsMovingForward", false);
        }
    }

}
