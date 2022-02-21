using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public GameObject cam;
    public Transform startMarker;
    public Transform endMarker;
    public float speed;
    private float startTime;
    public float journeyLength;

    public bool camIsMoving = false;
    public bool camCanMove = false;
    public void CamMouvement()
    {
        if (camCanMove)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            cam.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
            camIsMoving = true;
        }
    }

    public void StartMovingCamera()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
    }

    public IEnumerator CanMoveChrono()
    {
        yield return new WaitForSeconds(3);
        camCanMove = false;
        camIsMoving = false;
    }
}
