using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    //This is an unusual but handy use of Vector2s. x holds a min value
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 6f; //Seconds the PowerUp exists
    public float fadeTime = 4f; //Seconds until it will fade

    [Header("Dynamic")]
    public eWeaponType _type; // They type of the PowerUp
    public GameObject cube; //Reference to the Cube child
    public TextMesh letter; // Reference to the TextMesh
    public Vector3 rotPerSecond; //Euler rotation speed
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;


    void Awake()
    {
        //Find the Cube reference
        cube = transform.GetChild(0).gameObject;
        //Find the TextMesh and other components
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        //Set a random velocity
        Vector3 vel = Random.onUnitSphere; //Get Random XYZ velocity
        //Random.onUnitSphere gives you a vector point that is somewhere on
        // the surface of the sphere with a radius of 1m around the origin
        vel.z = 0; //Flatten the vel to the XY plane
        vel.Normalize(); //Normalize a Vector3 makes it length 1m
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        //Set the rotation of this GameObject to R:[0, 0, 0]
        transform.rotation = Quaternion.identity;
        //Quaternion.identity is equal to no rotation

        //Set up the rotPerSecond for the Cube child using rotMinMax x and y
        rotPerSecond = new Vector3(Random.Range(rotMinMax[0], rotMinMax[1]),
                                    Random.Range(rotMinMax[0], rotMinMax[1]),
                                    Random.Range(rotMinMax[0], rotMinMax[1]));

        birthTime = Time.time;
    }


    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        //Fade out the PowerUp over time
        //Given the default values, a PowerUp will exist for 10 seconds
        // and then fade out over 4 seconds
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;

        if (u >= 1) //Destroy this PowerUp
        {
            Destroy(this.gameObject);
            return;
        }

        if (u > 0)
        {
            Color c = cubeMat.color;
            c.a = 1f - u;   // Set the alpha of PowerCube to 1-u
            cubeMat.color = c;
            // Fade the letter too, just not as much
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen)
        {
            //If the powerup has drifted entirely off screen, destroy it
            Destroy(gameObject);
        }
    }


    public eWeaponType type { get { return _type; } set { SetType(value); } }


    public void SetType(eWeaponType wt)
    {
        //Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        //Set the color of the cube child
        cubeMat.color = def.powerUpColor;
        //letter.color = def.color; //We could colorize the letter too
        letter.text = def.letter; //Set the letter that is shown
        _type = wt;  //Finally actually set the type  //maybe fix with type -> _type
    }
    public void AbsorbedBy(GameObject target)
    {
        //This function is called by the Hero class when a Powerup is collected
        //We could tween into the target and shrink in size,
        // but for now, just destroy this.gameObject
        Destroy(this.gameObject);
    }
    


}
