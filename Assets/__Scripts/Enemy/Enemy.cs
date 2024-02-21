using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    // Movement Speed 10m/s
    public float speed = 10f;
    // Shots/Second (Unused)
    public float fireRate = 0.3f;
    // Damage needed to destroy enemy
    public float health = 10f;
    // Points earned for this enemy
    public int score = 100;
    // Chance to drop PowerUp
    public float powerUpDropChance = 1f;

    protected bool calledShipDestroyed = false;

    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }

    public Vector3 pos
    {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }
    
    
    void Update()
    {
        Move();

        // Chekcs if Enemy has gone off bottom
        /*if (!bndCheck.isOnScreen)
        {
            if (pos.y < bndCheck.camHeight - bndCheck.radius)
                Destroy(gameObject);
        }*/

        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
            Destroy(gameObject);
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision collider)
    {
        GameObject otherGO = collider.gameObject;

        // Check for collision with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();

        if (p != null)
        {
            // Only damage this Enemy if it's on screen
            if (bndCheck.isOnScreen)
            {
                // Get the damage amount from the Main WEAP_DICT
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                if (health <= 0)
                {
                    if (!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    Destroy(this.gameObject);
                    Hero.Score(this.score);
                }
            }

            // Destroy the ProjectileHero regardless
            Destroy(otherGO);
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " +
                otherGO.name);
        }
    }
}
