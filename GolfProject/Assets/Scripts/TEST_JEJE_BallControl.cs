using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class TEST_JEJE_BallControl : MonoBehaviour
{
    public float power = 10f;
    public float maxDrag = 5f;
    public Rigidbody2D RigidBody;
    public LineRenderer LineRenderer;
    public List<GameObject> blocRoom;
    private int numberHit;
    private int recoltedCoins = 0;
    [SerializeField]
    private TMP_Text textCoins;

    public GameObject Ball;

    //public PhysicsMaterial2D basic;
    public PhysicsMaterial2D test;

    private bool isBeingHeld = false;

    Vector2 dragStartPos;

    private void Start()
    {
        numberHit = 0;
        textCoins.text = "" + recoltedCoins;

        RigidBody = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && RigidBody.velocity.sqrMagnitude == 0)
        {
            isBeingHeld = true;

            if (Input.GetMouseButtonDown(0))
            {
                DragStart();
            }
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                Dragging();
            }
        }

        if (Input.GetMouseButtonUp(0) && isBeingHeld == true && RigidBody.velocity.sqrMagnitude == 0)
        {
            DragRealease();
            numberHit++;
            Debug.Log(numberHit);
            isBeingHeld = false;
        }
    }
    private void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dragStartPos.z = 0f;
        LineRenderer.positionCount = 1;
        LineRenderer.SetPosition(0, dragStartPos);
    }
    private void Dragging()
    {
        Vector2 draggingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dragStartPos.z = 0f;
        LineRenderer.positionCount = 2;
        LineRenderer.SetPosition(1, draggingPos);
    }
    private void DragRealease()
    {
        LineRenderer.positionCount = 0;

        Vector2 dragReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dragStartPos.z = 0f;

        Vector2 force = dragStartPos - dragReleasePos;
        Vector2 clampedForce = Vector2.ClampMagnitude(force, maxDrag) * power;
        RigidBody.AddForce(clampedForce, ForceMode2D.Impulse);
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
            RigidBody.sharedMaterial = test;
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