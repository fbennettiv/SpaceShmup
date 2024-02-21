using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(EnemyShield))]
public class Enemy_4 : Enemy
{
    [Header("Enemy_4 Inscribed Fields")]
    // Durations of interpolation movement
    public float duration = 4;
    private EnemyShield[] allShields;
    private EnemyShield thisShield;
    // The two points to interpolate
    private Vector3 p0, p1;
    // Birth time for this Enemy_4
    private float timeStart;

    void Start()
    {
        allShields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();

        // Initially set p0 & p1 to the current position (from Main.SpawnEnemy())
        p0 = p1 = pos;
        InitMovement();
    }

    void InitMovement()
    {
        // Set p0 to the old p1
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // Make sure that is moves to the different quadrant of the screen
        if (p0.x * p1.x > 0 && p0.y * p1.y > 0)
        {
            if (Mathf.Abs(p0.x) > Mathf.Abs(p0.y))
            {
                p1.x *= -1;
            }
            else
            {
                p1.y *= -1;
            }
        }

        // reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        // Overrides Enemy.Move() with linear inpterpolation
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        // Easing: Sine -0.15
        u = u - 0.15f * Mathf.Sin(u * 2 * Mathf.PI);
        // Simple linear interpolation
        pos = (1 - u) * p0 + u * p1;
    }

    /// <summary>
    /// Enemy_4 Collisions are handled differently from other Enemy subclasses
    /// to enable protection by EnemyShields.
    /// </summary
    /// <param name="coll"></param>
    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        // Make sure this was hit by a ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            // Destroy the ProjectileHero regardless of bndCheck.isOnScreen
            Destroy(otherGO);

            // Only damage this Enemy if it�s on screen
            if (bndCheck.isOnScreen)
            {
                // Find the GameObject of this Enemy_4 that was actually hit
                GameObject hitGO = coll.contacts[0].thisCollider.gameObject;
                if (hitGO == otherGO)
                {
                    hitGO =
                   coll.contacts[0].otherCollider.gameObject;
                }

                // Get the damage amount from the Main WEAP_DICT.
                float dmg = Main.GET_WEAPON_DEFINITION(
               p.type).damageOnHit;

                // Find the EnemyShield that was hit (if there was one)
                bool shieldFound = false;
                foreach (EnemyShield es in allShields)
                { 
                    if (es.gameObject == hitGO)
                    {
                        es.TakeDamage(dmg);
                        shieldFound = true;
                    }
                }
                if (!shieldFound) thisShield.TakeDamage(dmg);

                // If thisShield is still active, then it has not been destroyed
                if (thisShield.isActive) return;

                // This ship was destroyed so tell Main about it
                if (!calledShipDestroyed)
                {
                    Main.SHIP_DESTROYED(this);
                    calledShipDestroyed = true;
                }

                // Destroy this Enemy_4
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Enemy_4 hit by non-ProjectileHero: "
           + otherGO.name);
        }
    }
}
