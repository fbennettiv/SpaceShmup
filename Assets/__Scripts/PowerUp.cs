using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]

public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    [Tooltip("x holds a min value and y a max value for a Random.Range() call")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    [Tooltip("x holds a min value and y a max value for a Random.Range() call")]
    public Vector2 driftMinMax = new Vector2(.25f, 2);

    // PowerUp will exist for a # of seconds
    public float lifeTime = 10;
    // Then it fades a # seconds
    public float fadeTime = 4;

    [Header("Dynamic")]
    // Type of PowerUp
    public eWeaponType _type;
    // Ref to PowerUp Cube
    public GameObject cube;
    // Ref to TextMesh
    public TextMesh letter;
    // Euler rotation speed for PowerCube
    public Vector3 rotPerSecond;
    // Time.time this was instantiated
    public float birthTime;
    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    void Awake()
    {
        // Find the Cube reference (there’s only a single child) 
        cube = transform.GetChild(0).gameObject;
        // Find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        // Set a random velocity
        Vector3 vel = Random.onUnitSphere;
        // Flatten the vel to the XY plane
        vel.z = 0;
        // Normalizing a Vector3 sets its length to 1m
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Set the rotation of this PowerUp GameObject to R:[0, 0, 0 ]
        // Quaternion.identity is equal to no rotation.
        transform.rotation = Quaternion.identity;
        // Randomize rotPerSecond for PowerCube using rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax[0], rotMinMax[1]),
        Random.Range(rotMinMax[0], rotMinMax[1]), 
        Random.Range(rotMinMax[0], rotMinMax[1]));

        birthTime = Time.time;
    }


    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // Fade out the PowerUp over time
        // Given the default values, a PowerUp will exist for 10 seconds

        // and then fade out over 4 seconds.
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;

        // If u >= 1, destroy this PowerUp 
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // If u>0, decrease the opacity (i.e., alpha) of the PowerCube & Letter
        if (u > 0)
        {
            Color c = cubeMat.color;
            // Set the alpha of PowerCube to 1-u
            c.a = 1f - u;

            cubeMat.color = c;
            // Fade the Letter too, just not as much
            c = letter.color;
            // Set the alpha of the letter to 1-(u/2)
            c.a = 1f - (u * 0.5f);

            letter.color = c;
        }

        if (!bndCheck.isOnScreen)
        {
            // If the PowerUp has drifted entirely off screen, destroy it
            Destroy(gameObject);
        }
    }

    public eWeaponType type
    {
        get { return _type; }
        set
        {
            SetType(value);
        }
    }

    public void SetType(eWeaponType wt)
    {
        // Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        // Set the color of PowerCube
        cubeMat.color = def.powerUpColor; 

        //letter.color = def.color; // We could colorize the letter too

        // Set the letter that is shown
        letter.text = def.letter;

        // Finally actually set the type
        _type = wt;
    }

    /// <summary>
    /// This function is called by the Hero class when a PowerUp is collected.
    /// </summary>
    /// <param name="target">The GameObject absorbing this
    ///PowerUp</param>> 
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
