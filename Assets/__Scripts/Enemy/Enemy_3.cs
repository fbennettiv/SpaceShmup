using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    [Header("Enemy_3 Inscribed Fields")]
    public float lifeTime = 5;
    public Vector2 midPointYRange = new Vector2(1.5f, 3);
    [Tooltip("If true, the Bezier points and path are drawn in Scene")]
    public bool drawDebugInfo = true;

    [Header("Enemy_3 Private Fields")]
    [SerializeField]
    // Points for Bezier curve
    private Vector3[] points;
    [SerializeField]
    private float birthTime;

    void Start()
    {
        // Initialize points
        points = new Vector3[3];
        
        // The start position is alr set to Main.SpawnEnemy()
        points[0] = pos;

        // Set xMin and xMax the same way Main.SpawnEnemy() does
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        // Pick random middle position in the bottom half of screen
        points[1] = Vector3.zero;
        points[1].x = Random.Range(xMin, xMax);
        float midYMult = Random.Range(midPointYRange[0], midPointYRange[1]);
        points[1].y = -bndCheck.camHeight * midYMult;

        // Pick random final pos above the top of screen
        points[2] = Vector3.zero;
        points[2].y = pos.y;
        points[2].x = Random.Range(xMin, xMax);

        // Set birthTime to current time
        birthTime = Time.time;

        if (drawDebugInfo) DrawDebug();
    }

    public override void Move()
    {
        // Bezier cuves work based on u value between 0 and 1
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1)
        {
            // Destroy Enemy_3
            Destroy(this.gameObject);
            return;
        }

        transform.rotation = Quaternion.Euler(u * 180, 0, 0);

        // Interpolate the three Bezier curve points
        u = u - 0.1f * Mathf.Sin(u * Mathf.PI * 2);
        pos = Utils.Bezier(u, points);
    }

    void DrawDebug()
    {
        // Draw the three points
        Debug.DrawLine(points[0], points[1], Color.cyan, lifeTime);
        Debug.DrawLine(points[1], points[2], Color.yellow, lifeTime);

        // Draw the bezier curve
        float numSections = 20;
        Vector3 prevPoint = points[0];

        Color col;
        Vector3 pt;

        for (int i = 1; i < numSections; i++)
        {
            float u = i / numSections;
            pt = Utils.Bezier(u, points);
            col = Color.Lerp(Color.cyan, Color.yellow, u);
            Debug.DrawLine(prevPoint, pt, col, lifeTime);

            prevPoint = pt;
        }
    }
}
