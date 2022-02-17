using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class Mobile_BallControl : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    public float power = 10f;
    public float maxDrag = 5f;
    private Rigidbody2D RigidBody;
    public LineRenderer LineRenderer;
    private SpriteRenderer sr;
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

    Touch touch;
    Vector2 dragStartPos;

    //mouvement de cam quand on gagne le niveau, elle décale d'un point à l'autre
    public GameObject cam;
    public Transform startMarker;
    public Transform endMarker;
    public float speed;
    private float startTime;
    private float journeyLength;

    //private bool isBeingHeld = false;

    public bool isAbleToShoot = true;

    private void Start()
    {
        numberHit = 0;
        textCoins.text = "" + recoltedCoins;

        RigidBody = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (RigidBody.velocity.magnitude != 0)
            isAbleToShoot = false;
        else
            isAbleToShoot = true;

        if (Input.touchCount > 0 && isAbleToShoot == true)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                DragStart();
                isBeingHeld = true;
            }
            if (touch.phase == TouchPhase.Moved)
            {
                Dragging();
            }
            if (touch.phase == TouchPhase.Ended && isBeingHeld == true)
            {
                DragRealease();
                numberHit++;
                Debug.Log(numberHit);
                isBeingHeld = false;
            }
        }

        CamMouvement();
        HitLimit();
    }
    private void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(touch.position);
        //dragStartPos.z = 0f;
        LineRenderer.positionCount = 1;
        LineRenderer.SetPosition(0, dragStartPos);
    }
    private void Dragging()
    {
        if (isBeingHeld == true)
        {
            Vector2 draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
            //dragStartPos.z = 0f;
            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(1, draggingPos);
        } 
    }
    private void DragRealease()
    {
        LineRenderer.positionCount = 0;

        Vector2 dragReleasePos = Camera.main.ScreenToWorldPoint(touch.position);
        //dragStartPos.z = 0f;

        Vector2 force = dragStartPos - dragReleasePos;
        Vector2 clampedForce = Vector2.ClampMagnitude(force, maxDrag) * power;
        RigidBody.AddForce(clampedForce, ForceMode2D.Impulse);
    }

    private void HitLimit() // limite des coups, pas encore setup le "perdu" etc, ça reset la room dans laquelle on est
    {
        if (inRoom1)
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

    private void CamMouvement()
    {
        if (isIn) // qd la balle est dans le trou la caméra slide vers le prochain niveau
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            cam.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
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

                sr.DOFade(0, 1.5f); // on fait un fondu pour faire disparaître la balle
                StartCoroutine(fadeIn()); // on lance une coroutine pour remettre l'alpha à 0                
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

                sr.DOFade(0, 1.5f);
                StartCoroutine(fadeIn());
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
            //RigidBody.sharedMaterial = test;
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
            RigidBody.sharedMaterial = groundEffect[1];
            // à la base j'avais voulu faire une collision entre la balle et le sable directement mais il y avait quand même un rebond qui se faisait avec l'ancien material
            // du coup j'ai créé un enfant à chaque bloc de sable qui a juste un collider plus épais que l'original comme ça la balle détecte ce collider là
            // ce qui fait qu'elle a le temps de changer de material avant de toucher le vrai sol qui modifie son material
        }

        if (collision.gameObject.tag == "BasicGround")
        {
            RigidBody.sharedMaterial = groundEffect[0];
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

    IEnumerator fadeIn()
    {
        yield return new WaitForSeconds(2.3f); //le temps que la cam soit arrivée au level suivant
        if (inRoom1)
            ball.transform.position = new Vector3(12.31f, -4.33f, 0);
        if (inRoom2)
            ball.transform.position = new Vector3(34.34f, -4.33f, 0);
        Debug.Log("FADE");
        sr.DOFade(1, 1.5f); // on reset l'alpha de la balle à 1
    }
}