using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    //Singleton Property
    static public Hero S { get; private set; }

    [Header("Inscribed")]

    //Control the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Dynamic")]
    [Range(0, 4)]
    [SerializeField]
    private float _shieldLevel = 1;

    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();

    // Create a WeaponFireDelegate fireEvent.
    public event WeaponFireDelegate fireEvent;

    void Awake()
    {
        if (S == null)
            S = this;
        else
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        //fireEvent += TempFire;

        // Reset the Weapons to start _Hero with 1 blaster
        ClearWeapons();
        weapons[0].SetType(eWeaponType.blaster);
    }

    // Update is called once per frame
    // Varies on fps
    void Update()
    {
        // Pull in information from Input Class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        // Change transform.position based on the axis
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);

        // Use fireEvent to fire Weapons when the Spacebar is pressed
        if (Input.GetAxis("Jump") == 1 && fireEvent != null)
            fireEvent();
    }

    private void OnTriggerEnter(Collider collider)
    {
        Transform rootT = collider.gameObject.transform.root;

        GameObject go = rootT.gameObject;
        /*Debug.Log("Shield trigger hit by: "
            + go.gameObject.name);*/

        // Make sure it's not the same triggering go as last time
        if (go == lastTriggerGo) { return; }
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        PowerUp pUp = go.GetComponent<PowerUp>();

        // If the shield was triggered by an enemy
        if (enemy != null)
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (pUp != null)
        {
            AbsorbedPowerUp(pUp);
        }
        else
        {
            Debug.LogWarning("Shield trigger hit by non-Enemy: "
                + go.name);
        }
    }

    public void AbsorbedPowerUp(PowerUp powerUp)
    {
        Debug.Log("Absorbed PowerUp: " + powerUp.type);
        switch (powerUp.type)
        {
            case eWeaponType.shield:
                shieldLevel++;
                break;

            default:
                // If it is the same type
                if (powerUp.type == weapons[0].type)
                {
                    Weapon weap = GetEmptyWeaponSlot();
                    if (weap != null)
                    {
                        // Set it to pUp.type
                        weap.SetType(powerUp.type);
                    }
                    else
                    {
                        ClearWeapons();
                        weapons[0].SetType(powerUp.type);
                    }
                }
                break;
        }
        powerUp.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get { return _shieldLevel; }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);

            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.HERO_DIED();
            }
        }
    }

    ///<summary>
    /// Finds the first empty weapon slot (i.e., type = none)
    /// and returns it
    /// </summary>
    /// <returns>
    /// The first empty Weapon slot or null if none are empty
    /// </returns>
    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == eWeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Sets the type of all Weapon Slots to none
    /// </summary>
    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(eWeaponType.none);
        }
    }
}
