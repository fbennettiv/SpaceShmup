using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
///  This is an enum of various possible weapon types.
///  It includes a "shield" type to allow shield PowerUp.
///  Items marked [NI] are Not Implemented just yet.
/// </summary>
public enum eWeaponType
{
    none,       // Default / no weapon
    blaster,    // Simple blaster
    spread,     // Multiple shots simultaneously
    phaser,     // [NI] Shows that move in waves
    missile,    // [NI] Homing missiles
    laser,      // [NI] Damage over time
    shield      // Raise shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties 
/// of a specific weapon in the Inspector. The Main class has
/// an array of WeaponDefinitions that makes this possible. 
/// </summary> 

[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;

    [Tooltip("Letter to show on the PowerUp Cube")]
    public string letter;

    [Tooltip("Color of PowerUp Cube")]
    public Color powerUpColor = Color.white;

    [Tooltip("Prefab of Weapon model that is attached to Player Ship")]
    public GameObject weaponModelPrefab;

    [Tooltip("Prefab of projective that is fired")]
    public GameObject projectilePrefab;

    [Tooltip("Color of the projective that is fired")]
    public Color projectileColor = Color.white;

    [Tooltip("Damage caused when a single projectile hits an enemy")]
    public float damageOnHit = 0;

    [Tooltip("Damaage caused per second by the Laser [NI]")]
    public float damagePerSecond = 0;

    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;

    [Tooltip("Velocity of individual projectiles")]
    public float velocity = 50;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Dynamic")]
    [SerializeField]
    [Tooltip("Setting this manually while playing does not work properly")]
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    // Time the Weapon will fire next
    public float nextShotTime;
    private GameObject weaponModel;
    private Transform shotPointTrans;

    void Start()
    {
        // Set up PROJECTILE_ANCHOR
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);

        // Call SetType() for default _type set
        SetType(_type);

        // Find fireEvent of a Hero Component in parent heirarchy
        Hero hero = GetComponentInParent<Hero>();

        if (hero != null) { hero.fireEvent += Fire; }
    }

    public eWeaponType type
    {
        get { return _type; }
        set { _type = value; }
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;

        if (type == eWeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else { this.gameObject.SetActive(true); }

        // Get WeaponDefinition for this type from Main
        def = Main.GET_WEAPON_DEFINITION(_type);

        // Destroy any old model and then attach a model for this weapon
        if (weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        // You can fire right after _type is set
        nextShotTime = 0;
    }

    private void Fire()
    {
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) { return; }

        // If it hasn't been enough time between shots, return
        if (Time.time < nextShotTime) return;

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        switch (type)
        {
            case eWeaponType.blaster:
                {
                    p = MakeProjectile();
                    p.vel = vel;
                    break;
                }
            case eWeaponType.spread:
                {
                    p = MakeProjectile();
                    p.vel = vel;
                    p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                    p.vel = p.transform.rotation * vel;
                    p = MakeProjectile();
                    p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                    p.vel = p.transform.rotation * vel;
                    break;
                }  

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

        return p;
    }
}