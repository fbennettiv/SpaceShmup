using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class BlinkColorOnHit : MonoBehaviour
{
    // # seconds to show damage
    private static float blinkDuration = 0.1f;
    private static Color blinkColor = Color.red;

    [Header("Dynamic")]
    public bool showingColor = false;
    // Time to stop showing the color
    public float blinkCompleteTime;
    public bool ignoreOnCollisionEnter = false;

    // All the Materials of this & its children
    private Material[] materials;
    private Color[] originalColors;
    private BoundsCheck bndCheck;

    private void Awake()
    {
        bndCheck = GetComponentInParent<BoundsCheck>();

        // Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    private void Update()
    {
        if (showingColor && Time.time > blinkCompleteTime)
            RevertColor();
    }

    void OnCollisionEnter(Collision collider)
    {
        if (ignoreOnCollisionEnter) return;

        // Check for collisions with ProjectileHero
        ProjectileHero p = collider.gameObject.GetComponent<ProjectileHero>();
        if (p != null)
        {
            if (bndCheck != null && !bndCheck.isOnScreen)
            {
                // Dont show dmg if off screen
                return;
            }
            SetColors();
        }
    }

    ///<summary>
    /// Sets the Albed color (main color) of all materials in the
    /// materaisl array to blinkColor, sets showing COlor to true,
    /// sets the time that the colors should be reverted
    /// </summary>
    public void SetColors()
    {
        foreach (Material m in materials)
        {
            m.color = blinkColor;
        }

        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
    }

    ///<summary>
    /// Reverts all materials in the materials array back to their original color
    /// and sets showingColor to false.
    /// </summary>
    void RevertColor()
    {
        for (int i = 0;i < materials.Length;i++)
        {
            materials[i].color = originalColors[i];
        }
        showingColor = false;
    }
}
