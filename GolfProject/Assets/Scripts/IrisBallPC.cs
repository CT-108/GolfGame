using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IrisBallPC : MonoBehaviour
{
    public float power = 10f;
    public float maxDrag = 5f;
    public Rigidbody2D rb;
    public LineRenderer lr;
    public List<GameObject> blocRoom; // empêchent au joueur d'aller dans un autre niveau tant qu'il n'a pas gagné
    private int numberHit; // nbre de coups
    private int recoltedCoins = 0;
    [SerializeField]
    private TMP_Text textCoins;
    private bool isBeingHeld = false;
    private bool isIn = false; //est DANS le trou
    private int holeTime = 0; // quand la balle rebondie dans le trou de victoire
    // pour savoir dans quelle partie du niveau on est
    private bool inRoom1 = true;
    private bool inRoom2 = false;
    private bool inRoom3 = false;

    public List<PhysicsMaterial2D> groundEffect; // 0 = basic 1 = sable

    Vector2 dragStartPos;

    //mouvement de cam quand on gagne le niveau, elle décale d'un point à l'autre
    public GameObject cam;
    public Transform startMarker;
    public Transform endMarker;
    public float speed;
    private float startTime;
    private float journeyLength;

    private void Start()
    {
        numberHit = 0; 
        textCoins.text = "" + recoltedCoins;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
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

        if (Input.GetMouseButtonUp(0) && isBeingHeld == true)
        {
            DragRealease();
            numberHit++; // on compte un coup quand le joueur lâche la souris/doigt
            Debug.Log(numberHit);
            isBeingHeld = false;
        }

        CamMouvement();
        //HitLimit();
    }


    private void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dragStartPos.z = 0f;
        lr.positionCount = 1;
        lr.SetPosition(0, dragStartPos);
    }
    private void Dragging()
    {
        Vector2 draggingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dragStartPos.z = 0f;
        lr.positionCount = 2;
        lr.SetPosition(1, draggingPos);
    }
    private void DragRealease()
    {
        lr.positionCount = 0;

        Vector2 dragReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dragStartPos.z = 0f;

        Vector2 force = dragStartPos - dragReleasePos;
        Vector2 clampedForce = Vector2.ClampMagnitude(force, maxDrag) * power;
        rb.AddForce(clampedForce, ForceMode2D.Impulse);
    }

    private void CamMouvement()
    {
        if (isIn) // qd la balle est dans le trou la caméra slide vers le prochain niveau
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            cam.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
        }
    }
    

    private void HitLimit() // limite des coups, pas encore setup le "perdu" etc, ça reset la room dans laquelle on est
    {
        if(inRoom1)
        {
            if (numberHit >= 5) // nombre modulable
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
                Debug.Log("Reload Scene"); // on restart la scene dans laquelle ON EST
            }
        } 
        if (inRoom2)
        {
            if (numberHit >= 6)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
                Debug.Log("Reload Scene");
            }
        }
        if (inRoom3)
        {
            if (numberHit >= 10)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
                Debug.Log("Reload Scene");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hole") // quand la balle arrive dans le trou 
        {
            isIn = true; // elle est bien dedans
            StartCoroutine(isInChrono()); // on démarre un chrono qui sert à la faire disparaître (pas encore setup)
            holeTime++; // la variable passe donc à 1
            if (holeTime == 1) // on l'empêche de comptabiliser les rebonds de la balle dans le trou comme des victoires à chaque fois
            { 
                Debug.Log("Victoire");
                // pour calculer la distance que la cam aura à faire à l'instant T
                startTime = Time.time;
                journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
            }
            blocRoom[0].SetActive(false); // on désactive l'obstacle entre les 2 niveaux concernés

        }
        if (collision.gameObject.tag == "Hole2")
        {
            holeTime = 0;
            isIn = true;
            StartCoroutine(isInChrono());
            holeTime++;
            if (holeTime == 1)
            {
                Debug.Log("Victoire");
                //cam 
                startMarker.position = new Vector3(22.21f, 0, -20);
                endMarker.position = new Vector3(44.42f, 0, -20);
                startTime = Time.time;
                journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
            }
            blocRoom[1].SetActive(false);

        }

        if (collision.gameObject.tag == "Hole3")
        {
            Debug.Log("Victoire");
            //StartCoroutine(WaitForNextScene()); 
        }


        //Quand la balle arrive dans la room d'après elle ne peut plus revenir en arrière
        if (collision.gameObject.tag == "Room2")
        {
            inRoom2 = true;
            inRoom1 = false;
            inRoom3 = false;
            
            blocRoom[0].SetActive(true);
            numberHit = 0;
            //rb.sharedMaterial = test;
        }

        if (collision.gameObject.tag == "Room3")
        {
            inRoom2 = false;
            inRoom1 = false;
            inRoom3 = true;

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

        if (collision.gameObject.tag == "Sand")
        {
            rb.sharedMaterial = groundEffect[1];
            // à la base j'avais voulu faire une collision entre la balle et le sable directement mais il y avait quand même un rebond qui se faisait avec l'ancien material
            // du coup j'ai créé un enfant à chaque bloc de sable qui a juste un collider plus épais que l'original comme ça la balle détecte ce collider là
            // ce qui fait qu'elle a le temps de changer de material avant de toucher le vrai sol qui modifie son material
        }

        if (collision.gameObject.tag == "BasicGround")
        {
            rb.sharedMaterial = groundEffect[0];
            // idem qu'au dessus
        }
    }

    /*IEnumerator WaitForNextScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Scene_Iris 1");
    }*/

    IEnumerator isInChrono() 
    {
        yield return new WaitForSeconds(5);
        isIn = false;
    }
}
