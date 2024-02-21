using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BlinkColorOnHit))]
public class EnemyShield : MonoBehaviour
{
    [Header("Inscribed")]
    public float health = 10;

    private List<EnemyShield> protectors = new List<EnemyShield>();
    private BlinkColorOnHit blinker;

    void Start()
    {
        blinker = GetComponent<BlinkColorOnHit>();
        blinker.ignoreOnCollisionEnter = true;

        if (transform.parent == null) return;

        EnemyShield shieldParent = transform.parent.GetComponent<EnemyShield>();

        if (shieldParent != null)
        {
            shieldParent.AddProtector(this);
        }
    }

    /// <summary>
    /// Called by another EnemyShield to join the protectors of this EnemyShield
    /// </summary>
    /// <param> name="shieldChild"The EnemyShield that will protect this</param>
    public void AddProtector(EnemyShield shieldChild)
    {
        protectors.Add(shieldChild);
    }

    /// <summary>
    /// Shortcut for gameObject.activeInHierarchy and gameObject.SetActive()
    /// </summary>
    public bool isActive
    {
        get { return gameObject.activeInHierarchy; }
        private set { gameObject.SetActive(value); }

    }

    /// <summary>
    /// Called by Enemy_4.OnCollisionEnter() & parent�s EnemyShields.TakeDamage()
    /// to distribute damage to EnemyShield protectors.
    /// </summary
    /// <param> name="dmg"The amount of damage to behandled</param>
    /// <returns>Any damage not handled by this shield</returns>
    public float TakeDamage(float dmg)
    {

        foreach (EnemyShield es in protectors)
        {
            if (es.isActive)
            {
                dmg = es.TakeDamage(dmg);

                // If all damage was handled, return 0 damage
                if (dmg == 0) return 0;
            }
        }

        // If the code gets here, then this EnemyShield will blink & take damage
        // Make the blinker blink
        blinker.SetColors();

        health -= dmg;
        if (health <= 0)
        {
            // Deactivate this EnemyShield GameObject
            isActive = false;
            // Return any damage that was not absorbed by this EnemyShield
            return -health;
        }

        return 0;

    }
}
