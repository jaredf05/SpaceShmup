using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;  // Enables the loading and reloading of scenes

public class Main : MonoBehaviour
{
    static private Main S;  // A private Singleton for Main
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;          // Array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f;    // # Enemies spawned/second
    public float enemyInsetDefault = 1.5f;      // Inset from the sides
    public float gameRestartDelay = 2;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;
     public eWeaponType[] powerUpFrequency = new eWeaponType[] {
        eWeaponType.blaster, eWeaponType.blaster,
        eWeaponType.spread, eWeaponType.shield};

    private BoundsCheck bndCheck;



    static public void SHIP_DESTROYED(Enemy e)
        {
            //Potentially generate a PowerUp
            if(Random.value <= e.powerUpDropChance)
            {
                //Choose which powerup to pick
                //Pick one from the possibilities in powerupfrequency
                int ndx = Random.Range(0, S.powerUpFrequency.Length);
                eWeaponType pUpType = S.powerUpFrequency[ndx];
                //Spawn powerup
                GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
                PowerUp pUp = go.GetComponent<PowerUp>();
                pUp.SetType(pUpType);
                //Set it to the position of the destroyed ship
                pUp.transform.position = e.transform.position;
            }
        }



    void Awake()
    {
        S = this;
        // Set bndCheck to reference the BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once
        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);

        // A generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // If spawnEnemies is false, skip to the next invoke of SpawnEnemy()
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

        // Pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

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
        // Invoke the Restart() method in gameRestartDelay seconds
        Invoke(nameof(Restart), gameRestartDelay);

    }

    void Restart()
    {
        // Reload __Scene_0 to restart the game
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }


    ///<summary>
    ///Static function that gets a WeaponDefinition from the WEAP_DICT static
    ///protected field of the Main class
    ///</summary>
    ///<returns>The WeaponDefinition or, if there is no WeaponDefinition with
    /// the WeapnType passed in, returns a new WeaponDefinition with a
    /// WeaponType of none..</returns>
    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        //Check to make sure that the key exists in the dictionary
        // Attempting to retrieve a key that didnt exist would throw an error
        // so the following if statement is important
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        //This returns a new WeaponDefintion with a type of WeaponType.none,
        // which means it has failed to find the right WeaponDefintion.
        return (new WeaponDefinition());
    }
}
