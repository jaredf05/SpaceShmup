using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types
/// It also includes a "shield" type tp allow a shield power-up
/// Items marked [NI] below anr Not Implemented in the IGDPD book.
/// </summary>
public enum eWeaponType
{
    none,       //The default / no weapon
    blaster,    //A simple blaster
    spread,     //Two shots simultaneously
    phaser,     //[NI] Shots that move in waves
    missile,    //[NI] Homing missles
    laser,      //[Ni] Damage over time
    shield      //Raise shieldLevel
}


/// <summary>
/// The WeaponDefinition class allows you to set the properties
///     of a specific weapon in the Inspector. The Main class has
///     an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;
    public string letter;                 
    public Color powerUpColor = Color.white;
    public GameObject weaponModelPrefab;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Amount of damage caused
    public float damagePerSec = 0;
    public float delayBetweenShots = 0; 
    public float velocity = 50; //Speed of projectiles
}


public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Dynamica")]
    [SerializeField]
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    public float nextShotTime; //Time the Weappon will fire next

    private GameObject weaponModel;
    private Transform shotPointTrans;

    void Start()
    {
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);
        // Call SetType for the default _type set in the Inspector
        SetType(_type);

        // Find the fireEvent of a HeroComponent in the parent hierarchy
        Hero hero = GetComponentInParent<Hero>();
        if (hero != null) hero.fireEvent += Fire;
    }

    public eWeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;
        if (type == eWeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        // Get the WeaponDefinition for this type from Main
        def = Main.GET_WEAPON_DEFINITION(_type);
        // Destroy any old model and then attatch a model for this weapon
        if (weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0;  // You can fire immediately after _type is set
    }

    public void Fire()
    {
        //If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        //If it hasnt been enough time between shots, return
        if (Time.time < nextShotTime) return;

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        switch (type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;

            case eWeaponType.spread:
                p = MakeProjectile();
                p.vel = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;
        }
    }


    private ProjectileHero MakeProjectile()
    {
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return (p);
    }

}
