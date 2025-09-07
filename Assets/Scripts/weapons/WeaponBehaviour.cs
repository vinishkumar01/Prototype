using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform GunBarrel;
    [SerializeField] GameObject bulletTrail;
    [SerializeField] LayerMask hitLayer;

    [Header("Weapon Stats")]
    [SerializeField] int MagSize;
    [SerializeField] float fireRate;
    [SerializeField] protected bool isAutomatic;
    [SerializeField] float BulletRange = 50f;
    [SerializeField] int BurstCount = 3;

    [Header("Reloading")]
    [SerializeField] bool AutoReload;
    [SerializeField] float ReloadTime;

    int BulletsLeft;
    bool isReloading;
    bool CanShoot;

    [Header("Gun Animations")]
    [SerializeField] Animator _muzzleFlashAnimator;

    public enum FireMode { SemiAuto, FullAuto, Burst }
    public FireMode fireMode = FireMode.FullAuto;

    private void Start()
    {
        BulletsLeft = MagSize;
        CanShoot = true;
        AutoReload = true;
    }

    public void StartFiring()
    {
        //Checking the condition if the player is shooting means the Canshoot is true or is reloading then we are skipping the whole method and again when called if canshoot and isReloading is false then the whole method is executed.
        if (CanShoot || isReloading) return;
        CanShoot = true;

        switch (fireMode)
        {
            case FireMode.FullAuto:
                StartCoroutine(FireAuto());
                break;
            case FireMode.SemiAuto:
                ShootOnce();
                CanShoot = false;
                break;
            case FireMode.Burst:
                StartCoroutine(FireBurst());
                break;
        }

    }

    public void StopFiring()
    {
        CanShoot = false;
    }

    IEnumerator FireAuto()
    {

        while (CanShoot && BulletsLeft > 0)
        {
            ShootOnce();
            yield return new WaitForSeconds(fireRate);
        }

        if (BulletsLeft == 0 && AutoReload)
        {
            yield return Reload();
        }
    }

    IEnumerator FireBurst()
    {
        int shotsFired = 0;
        while (shotsFired < BurstCount && BulletsLeft > 0)
        {
            ShootOnce();
            shotsFired++;
            yield return new WaitForSeconds(fireRate);
        }
        if (BulletsLeft == 0 && AutoReload)
        {
            yield return Reload();
        }

        CanShoot = false;
    }

    void ShootOnce()
    {
        if (BulletsLeft <= 0 || isReloading)
        {
            if (BulletsLeft == 0 && AutoReload && !isReloading)
            {
                StartCoroutine(Reload());
            }
            return;
        }

        Vector3 origin = GunBarrel.position;
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 TargetDirection = (mousepos - origin).normalized;

        var RayHit = Physics2D.Raycast(origin, TargetDirection, BulletRange, hitLayer);

        //Debug.DrawLine(origin, origin + TargetDirection * 100f, Color.magenta, .1f);

        //Instantiating Bullet trails
        var BulletTrail = PoolManager.SpawnObject(bulletTrail, origin, Quaternion.identity, PoolManager.PoolType.GameObjects);
        _muzzleFlashAnimator.SetTrigger("Shoot");

        var trailScript = BulletTrail.GetComponent<BulletTracer>();

        if (RayHit.collider)
        {
            trailScript.initialize(origin, RayHit.point);

            var hittable = RayHit.collider.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.RecieveHit(RayHit);
            }
        }
        else
        {
            var endPosition = origin + TargetDirection * BulletRange;
            trailScript.initialize(origin, endPosition);
        }

        BulletsLeft--;
        //Debug.Log("Bullets Decrementing while shooting: " + BulletsLeft);

    }

    public IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(ReloadTime);
        BulletsLeft = MagSize;
        //Debug.Log("Bullets Reloaded: " + BulletsLeft);

        isReloading = false;
    }
}
