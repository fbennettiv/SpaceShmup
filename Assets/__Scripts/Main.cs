using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class Main : MonoBehaviour
{
    // Private singleton for Main
    static private Main S;

    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
    // # of enemies to exclude
    private int eLvl = 10;

    [Header("Inscribed")]
    public bool isScreen = false;
    public bool isWaves = false;
    public bool spawnEnemies = true;
    // Array of Enemy prefabs
    public GameObject[] prefabEnemies;
    // Enemies spawned/second
    public float enemySpawnPerSecond = 0.5f;
    // Inset from the sides
    public float enemyInsetDefault = 1.5f;

    public float gameRestartDelay = 2f;
    public GameObject prefabPowerUp;

    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[]{
                                            eWeaponType.blaster, eWeaponType.blaster,
                                            eWeaponType.spread, eWeaponType.shield };




    private BoundsCheck bndCheck;

    private void Awake()
    {
        if (isScreen)
            Invoke(nameof(ScreenLoader), 3f);

        S = this;

        // GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
        Invoke(nameof(Waves), 1f / enemySpawnPerSecond);

        // A generic Dictionary with eWeaponType as the key
        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void Waves()
    {
        if (!isWaves)
            return;

        switch (Hero.S.score)
        {
            case 1 when Hero.S.score >= 100 && Hero.S.score < 500:
                eLvl = 7;
                break;
            case 2 when Hero.S.score >= 500 && Hero.S.score < 1000:
                eLvl = 4;
                break;
            case 3 when Hero.S.score >= 1000 && Hero.S.score < 1500:
                eLvl = 1;
                break;
            case > 1500:
                eLvl = 0;
                spawnEnemies = false;
                break;
        }
    }

    public void SpawnEnemy()
    {
        // If SpawnEnemy is false, skip to the next invoke of SpawnEnemy()
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        int ndx = 0;

        // Pick a random Enemy prefab to instantiate
        if (!isWaves)
            ndx = Random.Range(0, prefabEnemies.Length);
        if (isWaves)
            ndx = Random.Range(0, prefabEnemies.Length - eLvl);

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
        Invoke(nameof(Waves), 1f / enemySpawnPerSecond);
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    /// <summary>
    /// Static function that gets a WeaponDefinition from the WEAP_DICT static
    /// protected field of the Main class
    /// </summary>
    /// <returns> The WeaponDefinition, or if there is no WeaponDefinition with
    /// the eWeaponType passed in, returns a new WeaponDefinition with a 
    /// eWeaponType of eWeapon.none. </returns>
    /// <param name="wt"> The eWeaponType of the desired
    /// WeaponDefinition </param>
    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return WEAP_DICT[wt];
        }

        // If no entry of the correct type exists in WEAP_DICT, return a new
        // WeaponDefinition with a type of eWeaponType.none (default value)
        return new WeaponDefinition();
    }

    /// <summary>
    /// Called by an Enemy ship when it is destroyed. 
    /// Sometimes creates a powerUp in place of ship.
    /// </summary>
    /// <param name="e"> The enemy that is destroyed </param>
    static public void SHIP_DESTROYED(Enemy e)
    {
        // Potentially generates PowerUp
        if (Random.value <= e.powerUpDropChance)
        {
            // Choose a PowerUp from the possibilities in PowerUpFreq
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            // Spawn a PowerUp
            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();

            // Set it to proper WeaponType
            pUp.SetType(pUpType);

            // Set it to the position of the destroyed ship
            pUp.transform.position = e.transform.position;
        }
    }

    void ScreenLoader()
    {
        SceneManager.LoadScene("_Start");
    }

    void DelayedRestart()
    {
        // Invoke the Restart(0 method in gameRestartDelay
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        // Reload __Scene_0 to restart game
        SceneManager.LoadScene("_GameOver");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }
}
