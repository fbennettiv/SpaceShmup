using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    // Pirvate singleton for Main
    static private Main S;

    [Header("Inscribed")]
    // Array of Enemy prefabs
    public GameObject[] prefabEnemies;
    // Enemies spawned/second
    public float enemySpawnPerSecond = 0.5f;
    // Inset from the sides
    public float enemyInsetDefault = 1.5f;

    public float gameRestartDelay = 2f;

    private BoundsCheck bndCheck;

    private void Awake()
    {
        S = this;

        // GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    public void SpawnEnemy()
    {
        // Pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]);

        // Position the Enemy above the screen with a random x position
        float enemyInset = enemyInsetDefault;
        
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // Set the initial position for the spawned Enemy

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    void DelayedRestart()
    {
        // Invoke the Restart(0 method in gameRestartDelay
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        // Reload __Scene_0 to restart game
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }
}
