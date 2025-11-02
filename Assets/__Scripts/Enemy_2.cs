using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Enemy_2 Inscribed Fields")]
    public float lifeTime = 10;
    // Enemy_2 uses a sin wave to modify a 2-point linear interpolation
    [Tooltip("Determines how much the Sine wave will ease the interpolation")]
    public float sinEccentricity = 0.6f;
    public AnimationCurve rotCurve;

    [Header("Enemy_2 Private Fields")]
    
    [SerializeField] private Vector3 p0, p1;
    [SerializeField] private float birthTime;
    private Quaternion baseRotation;

    void Start()
    {
        //Pick any point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //Pick any point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //Possibly swap sides
        if(Random.value > 0.5f)
        {
            //Setting the .x of each point to its negative will move it to 
            // the other side of the screen
            p0.x *= -1;
            p1.x *= -1;
        }
        //Set the birthTim to the current time
        birthTime = Time.time;
        // Set up the initial ship rotation
        transform.position = p0;
        transform.LookAt(p1, Vector3.back);
        baseRotation = transform.rotation;
    }
    public override void Move()
    {
        // Bexier curves work based on a value between 0 and 1
        float u = (Time.time - birthTime) / lifeTime;

        //If u>1, then it has been longer than lifeTime since birthTime
        if (u > 1)
        {
            //This enemy_2 has finished its life
            Destroy(this.gameObject);
            return;
        }

        // Use the AnimationCuirve to set the rotation about Y
        float shipRot = rotCurve.Evaluate(u) * 360;
        transform.rotation = baseRotation * Quaternion.Euler(-shipRot, 0, 0);


        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        //Interpolate the two linear interpolation points
        pos = (1 - u) * p0 + u * p1;
    }
}