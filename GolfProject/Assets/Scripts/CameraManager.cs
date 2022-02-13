using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject camera;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player") && !other.isTrigger)
        {
            //if (BallControl.Instance.isInHole)
            camera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            camera.SetActive(false);
        }
    }
}
