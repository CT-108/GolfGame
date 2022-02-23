using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    public float windForce;
    private Vector2 windDirection;
    private Rigidbody2D rb;
    public GameObject objDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        windDirection = objDir.transform.position;
    }
    private void FixedUpdate()
    {
        rb.AddForce(windDirection * windForce);
    }
}
