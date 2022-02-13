using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallControl : MonoBehaviour
{
    public float power = 10f;
    public float maxDrag = 5f;
    public Rigidbody2D rb;
    public LineRenderer lr;
    //private bool isInHole;
    public List<GameObject> blocRoom;
    private int numberHit;    

    Vector2 dragStartPos;
    Touch touch;

    private void Start()
    {
        //isInHole = false;
        numberHit = 0;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                DragStart();
            }
            if (touch.phase == TouchPhase.Moved)
            {
                Dragging();
            }
            if (touch.phase == TouchPhase.Ended)
            {
                DragRealease();
                numberHit++;
                Debug.Log(numberHit);
            }
        }
    }
    private void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(touch.position);
        //dragStartPos.z = 0f;
        lr.positionCount = 1;
        lr.SetPosition(0, dragStartPos);
    }
    private void Dragging()
    {
        Vector2 draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
        //dragStartPos.z = 0f;
        lr.positionCount = 2;
        lr.SetPosition(1, draggingPos);
    }
    private void DragRealease()
    {
        lr.positionCount = 0;

        Vector2 dragReleasePos = Camera.main.ScreenToWorldPoint(touch.position);
        //dragStartPos.z = 0f;

        Vector2 force = dragStartPos - dragReleasePos;
        Vector2 clampedForce = Vector2.ClampMagnitude(force, maxDrag) * power;
        rb.AddForce(clampedForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Hole")
        {
            Debug.Log("Victoire");
            blocRoom[0].SetActive(false);
            //isInHole = true;
        }
        if(collision.gameObject.tag == "Hole2")
        {
            Debug.Log("Victoire");
            blocRoom[1].SetActive(false);
            //isInHole = true;
        }
        if(collision.gameObject.tag == "Hole3")
        {
            Debug.Log("Victoire");
            SceneManager.LoadScene("Scene_Iris 1");
            //isInHole = true;
        }

        if (collision.gameObject.tag == "Room2")
        {
            blocRoom[0].SetActive(true);
            //isInHole = false;
            numberHit = 0;
        }
        
        if (collision.gameObject.tag == "Room3")
        {
            blocRoom[1].SetActive(true);
            //isInHole = false;
            numberHit = 0;
        }
    }
}
