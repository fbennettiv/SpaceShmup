using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Enemy_1 Inscribed Fields")]
    [Tooltip("# of seconds for a full sine wave")]

    public float waveFrequency = 2;
    [Tooltip("Sine wave width in meters")]
    public float waveWidth = 4;
    [Tooltip("Amount the ship will roll left and right with the sine wave")]
    public float waveRotY = 45;

    // Initial x value of pos
    private float x0;
    private float birthTime;

    void Start()
    {
        // x0 to initial pos
        x0 = pos.x;

        birthTime = Time.time;
    }

    // Override the Move method in Enemy
    public override void Move()
    {
        // Get editable Vector3, since pos is a property and we cannot directly edit
        Vector3 tempPos = pos;

        // Theta adjust based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // rotate a bit about y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //base.Move() still handles the movement in y
        base.Move();
    }
}
