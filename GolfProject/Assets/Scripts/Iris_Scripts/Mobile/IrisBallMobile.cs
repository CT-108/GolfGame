using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IrisBallMobile : MonoBehaviour
{
    public float power = 10f;
    public float maxDrag = 5f;
    public Rigidbody2D rb;
    public LineRenderer lr;
    public List<GameObject> blocRoom;
    private int numberHit;
    private int recoltedCoins = 0;
    [SerializeField]
    private TMP_Text textCoins;

    //public PhysicsMaterial2D basic;
    public PhysicsMaterial2D test;

    Vector2 dragStartPos;
    Touch touch;

    private void Start()
    {
        numberHit = 0;
        textCoins.text = "" + recoltedCoins;

        rb = GetComponent<Rigidbody2D>();
        
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
        //Quand la balle atteint le trou et peut passer à la room suivante
        if (collision.gameObject.tag == "Hole")
        {
            Debug.Log("Victoire");
            blocRoom[0].SetActive(false);
        }
        if (collision.gameObject.tag == "Hole2")
        {
            Debug.Log("Victoire");
            blocRoom[1].SetActive(false);
        }
        if (collision.gameObject.tag == "Hole3")
        {
            Debug.Log("Victoire");
            StartCoroutine(WaitForNextScene());
        }


        //Quand la balle arrive dans la room d'après elle ne peut plus revenir en arrière
        if (collision.gameObject.tag == "Room2")
        {
            blocRoom[0].SetActive(true);
            numberHit = 0;
            rb.sharedMaterial = test;
        }

        if (collision.gameObject.tag == "Room3")
        {
            blocRoom[1].SetActive(true);
            numberHit = 0;
        }


        //Collectibles
        if (collision.gameObject.tag == "Coin")
        {
            recoltedCoins++;
            textCoins.text = "" + recoltedCoins;
            Destroy(collision.gameObject);
        }

    }

    IEnumerator WaitForNextScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Scene_Iris 1");
    }
}
