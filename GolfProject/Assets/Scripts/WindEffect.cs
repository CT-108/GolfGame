using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    public float[] windForce;
    private Vector2 windDirectionRoom1;
    private Vector2 windDirectionRoom2;
    private Vector2 windDirectionRoom3;
    private Rigidbody2D rb;
    public GameObject[] objDir;

    public BallControl ballScript;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        windDirectionRoom1 = objDir[0].transform.position;
        windDirectionRoom2 = objDir[1].transform.position;
        windDirectionRoom3 = objDir[2].transform.position;
    }
    private void FixedUpdate()
    {
        if(ballScript.room == 0)
            rb.AddForce(windDirectionRoom1 * windForce[0]);

        if(ballScript.room == 1)
            rb.AddForce(windDirectionRoom2 * windForce[1]);

        if(ballScript.room == 2)
            rb.AddForce(windDirectionRoom3 * windForce[2]);
    }
}
