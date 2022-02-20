using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class Level1_BallControl : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    public float power = 10f;
    public float maxDrag = 5f;
    private Rigidbody2D rb;
    public LineRenderer lr;
    private SpriteRenderer sr;
    public int[] limitHits;
    private int _currentLimitHit; // empêchent au joueur d'aller dans un autre niveau tant qu'il n'a pas gagné
    private int numberHit; // nbre de coups
    private int recoltedCoins = 0;
    private int recoltedCoinsPerLevel = 0;
   
    public TMP_Text textCoins;
    public TMP_Text textHits;

    private bool isBeingHeld = false;
    private bool isAbleToShoot = true;
    private bool canMove = false; //est DANS le trou
    private int holeTime = 0; // quand la balle rebondie dans le trou de victoire
    // pour savoir dans quelle partie du niveau on est
    private bool inRoom1 = true;
    private bool inRoom2 = false;
    private bool inRoom3 = false;

    bool asWon = false;

    private bool camIsMoving = false;

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
        _currentLimitHit = 0;
        numberHit = 0;
        textCoins.text = "" + recoltedCoins;
        textHits.text = "" + numberHit + " / " + limitHits[(_currentLimitHit)];

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (rb.velocity.x != 0)
            isAbleToShoot = false;
        else
            isAbleToShoot = true;

        //(isAbleToShoot);

        if (Input.GetMouseButtonDown(0) && isAbleToShoot == true && !camIsMoving && !(numberHit >= limitHits[(_currentLimitHit)]))
        {

            isBeingHeld = true;

            if (Input.GetMouseButtonDown(0))
            {
                DragStart();
            }
        }

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Dragging();
        }

        if (Input.GetMouseButtonUp(0) && isBeingHeld == true)
        {
            DragRealease();
            
        }

        if (isAbleToShoot && rb.velocity.magnitude == 0 && !asWon)
            HitLimit();

        CamMouvement();     
        

        textHits.text = "" + numberHit + " / " + limitHits[(_currentLimitHit)];
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
        if (isBeingHeld == true)
        {
            Vector2 draggingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //dragStartPos.z = 0f;
            lr.positionCount = 2;
            lr.SetPosition(1, draggingPos);
        }
    }
    private void DragRealease()
    {
        lr.positionCount = 0;

        Vector2 dragReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dragStartPos.z = 0f;

        Vector2 force = dragStartPos - dragReleasePos;
        Vector2 clampedForce = Vector2.ClampMagnitude(force, maxDrag) * power;
        rb.AddForce(clampedForce, ForceMode2D.Impulse);

        numberHit++; // on compte un coup quand le joueur lâche la souris/doigt
                     //(numberHit);
        isBeingHeld = false;
    }

    private void CamMouvement()
    {
        if (canMove) // qd la balle est dans le trou la caméra slide vers le prochain niveau
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            cam.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
            camIsMoving = true;
        }
    }

    private void HitLimit() // limite des coups, pas encore setup le "perdu" etc, ça reset la room dans laquelle on est
    {
        if (inRoom2)
            _currentLimitHit = 1;
        if (inRoom3)
            _currentLimitHit = 2;

        if (numberHit >= limitHits[(_currentLimitHit)])
        {
            StartCoroutine(Perdu());
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hole") // quand la balle arrive dans le trou 
        {         

            asWon = true;
            canMove = true; // elle est bien dedans
            StartCoroutine(CanMoveChrono()); // on démarre un chrono qui sert à la faire disparaître (pas encore setup)
            holeTime++; // la variable passe donc à 1
            if (holeTime == 1) // on l'empêche de comptabiliser les rebonds de la balle dans le trou comme des victoires à chaque fois
            {
                // pour calculer la distance que la cam aura à faire à l'instant T
                startTime = Time.time;
                journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

                sr.DOFade(0, 1.5f); // on fait un fondu pour faire disparaître la balle
                StartCoroutine(FadeIn()); // on lance une coroutine pour remettre l'alpha à 0
            }

            if ((numberHit <= 3) && (recoltedCoinsPerLevel > 2))
                Debug.Log("Médaille or");
            else
                Debug.Log("Médaille argent");
            if (numberHit >= 4 && numberHit <= 6)
                Debug.Log("Médaille argent");
            if (numberHit >= 7 && numberHit < limitHits[(_currentLimitHit)])
                Debug.Log("Médaille bronze");

        }
        if (collision.gameObject.tag == "Hole2")
        {
            asWon = true;
            holeTime = 0;
            canMove = true;
            StartCoroutine(CanMoveChrono());
            holeTime++;
            if (holeTime == 1)
            {
                //cam 
                startMarker.position = new Vector3(23.53f, 0, -20);
                endMarker.position = new Vector3(47.06f, 0, -20);
                startTime = Time.time;
                journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

                sr.DOFade(0, 1.5f);
                StartCoroutine(FadeIn());
            }

            if ((numberHit <= 3) && (recoltedCoinsPerLevel > 2))
                Debug.Log("Médaille or");
            else
                Debug.Log("Médaille argent");
            if (numberHit >= 4 && numberHit <= 6)
                Debug.Log("Médaille argent");
            if (numberHit >= 7 && numberHit < limitHits[(_currentLimitHit)])
                Debug.Log("Médaille bronze");
        }

        if (collision.gameObject.tag == "Hole3")
        {
            holeTime = 0;
            canMove = true;
        }


        //Quand la balle arrive dans la room d'après elle ne peut plus revenir en arrière
        if (collision.gameObject.tag == "Room2")
        {
            
            inRoom2 = true;
            inRoom1 = false;
            inRoom3 = false;
       
            numberHit = 0;
            asWon = false;
            recoltedCoinsPerLevel = 0;

        }

        if (collision.gameObject.tag == "Room3")
        {
            
            inRoom2 = false;
            inRoom1 = false;
            inRoom3 = true;
           
            numberHit = 0;
            asWon = false;
            recoltedCoinsPerLevel = 0;
        }


        //Collectibles
        if (collision.gameObject.tag == "Coin")
        {
            recoltedCoins++;
            recoltedCoinsPerLevel++;
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

    IEnumerator CanMoveChrono()
    {
        yield return new WaitForSeconds(3);
        canMove = false;
        camIsMoving = false;
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(2.3f); //le temps que la cam soit arrivée au level suivant
        if (inRoom1)
            ball.transform.position = new Vector3(13.54f, -2.13f, 0);
        if (inRoom2)
            ball.transform.position = new Vector3(37.36f, -3.84f, 0);
        sr.DOFade(1, 1.5f); // on reset l'alpha de la balle à 1
    }

    IEnumerator Perdu()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("t'es nul");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
