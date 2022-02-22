using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System;

public class JeromeScript : MonoBehaviour
{
    public GameObject ball;
    [SerializeField] private float waitForLoose;
    [SerializeField] private float waitForFade;
    [SerializeField] private float linearDragOnGround;
    [SerializeField] private float linearDragOnAir;
    [SerializeField] private float minValueToBeAbleToShoot;
    [SerializeField] private float durationFadeInBall;

    [SerializeField] private int minHitGold;
    [SerializeField] private int minRecoltedCoinGold;
    [SerializeField] private int minHitSilver;
    [SerializeField] private int maxHitSilver;
    [SerializeField] private int minHitBronze;


    private Rigidbody2D rb;
    public LineRenderer lr;
    private SpriteRenderer sr;
    
    public int[] limitHits;
    private int _currentLimitHit;
    private int numberHit;
    private int recoltedCoins = 0;
    private int recoltedCoinsPerLevel = 0;
    private int holeTime = 0;

    public float power = 10f;
    public float maxDrag = 5f;

    public TMP_Text textCoins;
    public TMP_Text textHits;

    public List<MaterialType> MaterialTypes;

    public int PitContact = 0;

    private bool isBeingHeld = false;
    private bool isAbleToShoot = true;

    private bool asWon = false;

    public bool[] room;
    public GameObject[] startPoints;

    //public List<PhysicsMaterial2D> groundEffect;

    public CamMovement camScript;
    private CamMovement camMovement;

    Vector2 dragStartPos;
    Vector3 startPosition;

    Vector3 newPosition;

   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ball.transform.position = startPoints[0].transform.position;
        _currentLimitHit = 0;
        numberHit = 0;
        textCoins.text = "" + recoltedCoins;
        textHits.text = "" + numberHit + " / " + limitHits[(_currentLimitHit)];        
    }

    private void Update()
    {
        if (rb.velocity.y == 0)
            rb.drag = linearDragOnGround;
        else
            rb.drag = linearDragOnAir;

        if (PitContact > 1)
            PitContact = 1;

        //&& ball.GetComponent<MaterialType>().IsInPit == false
        if (rb.velocity.magnitude < minValueToBeAbleToShoot && PitContact == 0)
        {
            isAbleToShoot = true;
            newPosition = ball.transform.position;
        }
        else
            isAbleToShoot = false;

        if (Input.GetMouseButtonDown(0) && isAbleToShoot == true && !camScript.camIsMoving && !(numberHit >= limitHits[(_currentLimitHit)]))
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

        camScript.CamMouvement();

        textHits.text = "" + numberHit + " / " + limitHits[(_currentLimitHit)];
    }

    private void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lr.positionCount = 1;
        lr.SetPosition(0, dragStartPos);
    }
    private void Dragging()
    {
        if (isBeingHeld == true)
        {
            Vector2 draggingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lr.positionCount = 2;
            lr.SetPosition(1, draggingPos);
        }
    }
    private void DragRealease()
    {
        lr.positionCount = 0;

        Vector2 dragReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 force = dragStartPos - dragReleasePos;
        Vector2 clampedForce = Vector2.ClampMagnitude(force, maxDrag) * power;
        rb.AddForce(clampedForce, ForceMode2D.Impulse);

        numberHit++; 
        isBeingHeld = false;
    }
    

    private void HitLimit()
    {
        if (room[1])
            _currentLimitHit = 1;
        if (room[2])
            _currentLimitHit = 2;

        if (numberHit >= limitHits[(_currentLimitHit)])
        {
            PitContact++;
            StartCoroutine(Perdu());
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hole") 
        {        
            asWon = true;
            camScript.camCanMove = true; 
            StartCoroutine(camScript.CanMoveChrono()); 
            holeTime++; 
            if (holeTime == 1) 
            {
                camScript.StartMovingCamera();

                sr.DOFade(0, durationFadeInBall);
                StartCoroutine(FadeIn());
            }

            if ((numberHit <= minHitGold) && (recoltedCoinsPerLevel > minRecoltedCoinGold))
                Debug.Log("Médaille or");
            else
                Debug.Log("Médaille argent");
            if (numberHit >= minHitSilver && numberHit <= maxHitSilver)
                Debug.Log("Médaille argent");
            if (numberHit >= minHitBronze && numberHit < limitHits[(_currentLimitHit)])
                Debug.Log("Médaille bronze");
        }

        if (collision.gameObject.tag == "Hole2")
        {
            asWon = true;
            holeTime = 0;
            camScript.camCanMove = true;
            StartCoroutine(camScript.CanMoveChrono());
            holeTime++;
            if (holeTime == 1)
            {
                camScript.startMarker.position = new Vector3(23.53f, 0, -20);
                camScript.endMarker.position = new Vector3(47.06f, 0, -20);
                camScript.StartMovingCamera();

                sr.DOFade(0, durationFadeInBall);
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
            holeTime++;
            if (holeTime == 1)
                SceneManager.LoadScene("Ice_World");
        }

        if (collision.gameObject.tag == "Room2")
        {
            room[1] = true;
            room[0] = false;
            room[2] = false;
       
            numberHit = 0;
            asWon = false;
            recoltedCoinsPerLevel = 0;
        }

        if (collision.gameObject.tag == "Room3")
        {
            room[2] = true;
            room[1] = false;
            room[0] = false;
           
            numberHit = 0;
            asWon = false;
            recoltedCoinsPerLevel = 0;
        }

        if (collision.gameObject.tag == "Coin")
        {
            recoltedCoins++;
            recoltedCoinsPerLevel++;
            textCoins.text = "" + recoltedCoins;
            Destroy(collision.gameObject);
        }

        //if (collision.gameObject.tag == "BasicGround")
        //    rb.sharedMaterial = groundEffect[0];

        //if (collision.gameObject.tag == "Sand")
        //    rb.sharedMaterial = groundEffect[1];

        //if (collision.gameObject.tag == "Ice")
        //    rb.sharedMaterial = groundEffect[2];

        //if (collision.gameObject.tag == "Water")
        //    rb.sharedMaterial = groundEffect[3];

        foreach (var item in MaterialTypes)
        {
            if (item.NameTag == collision.gameObject.tag)
            {

                if (item.IsInPit && PitContact == 0)
                {
                    StartCoroutine(BallInPit());
                    PitContact++;
                    rb.velocity = Vector3.zero;
                    rb.inertia = 0;
                
                    Debug.Log(item);
                }

                if (item.IsInPit == false)
                    rb.sharedMaterial = item.PhysicsMaterial;
            }
        }
        
    }


    IEnumerator BallInPit()
    {
        isAbleToShoot = false;
        sr.DOFade(0, 1);
        yield return new WaitForSeconds(1);
        ball.transform.position = newPosition;
        sr.DOFade(1, 1);
        rb.inertia = 0.025f;
        yield return new WaitForSeconds(1);
        PitContact--;
        isAbleToShoot = true;
    }
    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(waitForFade); //le temps que la cam soit arrivée au level suivant
        Debug.Log("ok");
        if (room[0])
            ball.transform.position = startPoints[1].transform.position;

        if (room[1])
            ball.transform.position = startPoints[2].transform.position;
        sr.DOFade(1, 1.5f); // on reset l'alpha de la balle à 1
    }

    IEnumerator Perdu()
    {
        yield return new WaitForSeconds(waitForLoose);
        Debug.Log("t'es nul");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    [Serializable]
    public class MaterialType
    {
        public string NameTag;
        public PhysicsMaterial2D PhysicsMaterial;
        public bool IsInPit = false;
    }
}