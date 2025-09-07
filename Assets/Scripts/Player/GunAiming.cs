using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class GunAiming : MonoBehaviour
{
    Vector3 GunStartingPos;
    GrapplingGunConfig grappleGun;
    GrappleRopeConfigs grappleRope;
    WeaponBehaviour weaponBehave;
    

    public enum WeaponType
    {
        Pistol,
        Rifle,
        GrapplingGun
    }

    public WeaponType weaponType = WeaponType.Pistol;

    #region GunAiming Attributes

    [Header("Transform Reference")]
    Transform Player;
    [SerializeField] Transform GunPivot;

    [Header("Player Aim Rotation")]
    [SerializeField] bool rotateOverTime = false;
    [Range(0, 60)] [SerializeField] float rotationSpeed = 4f;

    #endregion

    private void Awake()
    {
        grappleGun = GetComponentInChildren<GrapplingGunConfig>();
        grappleRope = GetComponentInChildren<GrappleRopeConfigs>();
        weaponBehave = GetComponentInChildren<WeaponBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GunStartingPos = GunPivot.localScale;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        GunPivot.transform.position = Player.transform.position;
        WeaponSelection();
        
        if(weaponType == WeaponType.Pistol)
        {
            weaponBehave.fireMode = WeaponBehaviour.FireMode.SemiAuto;
        }
    }


    public void GunAim(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 PlayerArmDirection = lookPoint - GunPivot.position;

        //Calculating Angle in Degree
        float angle = Mathf.Atan2(PlayerArmDirection.y, PlayerArmDirection.x) * Mathf.Rad2Deg;

        if(rotateOverTime && allowRotationOverTime)
        {
            GunPivot.rotation = Quaternion.Lerp(GunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            GunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void WeaponsType()
    {
        switch(weaponType)
        {
            case WeaponType.GrapplingGun:
                grappleGun.GrapplingGunConfigs();
                break;
            case WeaponType.Pistol:
                Pistol();
                break;
            case WeaponType.Rifle:
                rifle();
                break;
        }
    }

    void WeaponSelection()
    {

        if(Input.GetAxis("Mouse ScrollWheel") > 0f) //Scroll Up
        {
            weaponType++;

            if((int)weaponType >= System.Enum.GetValues(typeof(WeaponType)).Length)
            {
                weaponType = 0;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) //Scroll Down
        {
            weaponType--;

            if ((int)weaponType < 0)
            {
                weaponType = (WeaponType)System.Enum.GetValues(typeof(WeaponType)).Length - 1;
            }
        }

        else if(weaponType == WeaponType.Pistol)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GunAim(MousePos, true);
        }
        else if(weaponType == WeaponType.Rifle)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GunAim(MousePos, true);
        }

        //Debug.Log("CurrentWeapon: " + weaponType);

        WeaponsType();
    }

    void Pistol()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            weaponBehave.StartFiring();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            weaponBehave.StopFiring();
        }
    }

    void rifle()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            weaponBehave.StartFiring();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            weaponBehave.StopFiring();
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            weaponBehave.fireMode++;
            Debug.Log(weaponBehave.fireMode);

            if((int)weaponBehave.fireMode >=3)
            {
                weaponBehave.fireMode = 0;
            }
        }
    }
    
}
