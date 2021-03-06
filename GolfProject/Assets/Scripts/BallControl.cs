using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System;

public class BallControl : MonoBehaviour
{
    public static BallControl Instance;
    private Rigidbody2D rb;
    public LineRenderer lr;
    public GameObject trail;
    private SpriteRenderer sr;
    public GameObject ball;
    public GameObject[] startPoints;
    Vector2 dragStartPos;
    public CamMovement camScript;
    public SceneTransition sceneScript;
    public AudioManager audioScript;
    Vector3 newPosition;
    public GameObject[] Hole;
    public GameObject audio;
    public List<MaterialType> MaterialTypes;
    public GameObject medal;

    public int room = 0;
    private int _currentLimitHit;
    private int numberHit;
    
    private int recoltedCoinsPerLevel = 0;
    private int holeTime = 0;
    public int pitContact = 0;
    public int[] minHitGold;
    public int[] minRecoltedCoinGold;
    public int[] minHitSilver;
    public int[] maxHitSilver;
    public int[] minHitBronze;
    public int[] limitHits;

    public float waitForLoose;
    public float waitForFade;
    public float linearDragOnGround;
    public float linearDragOnAir;
    public float minValueToBeAbleToShoot;
    public float durationFadeInBall;
    public float power = 10f;
    public float maxDrag = 5f;

    public TMP_Text textCoins;
    public TMP_Text textHits;
    public TMP_Text textMedals;

    private bool hadHit;
    public bool isBeingHeld = false;
    private bool isAbleToShoot = true;
    private bool asWon = false;


    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ball.transform.position = startPoints[0].transform.position;
        _currentLimitHit = 0;
        numberHit = 0;
        textCoins.text = "" + KeepingVariables.Instance.recoltedCoins;
        textHits.text = "" + numberHit + " / " + limitHits[(_currentLimitHit)] + " coups";        
    }

    private void Update()
    {
        if (rb.velocity.y > -2 && rb.velocity.y < 0.3)
            rb.drag = linearDragOnGround;
        else
            rb.drag = linearDragOnAir;

        if (pitContact > 1)
            pitContact = 1;

        if (rb.velocity.magnitude < minValueToBeAbleToShoot && pitContact == 0)
        {
            isAbleToShoot = true;
            newPosition = ball.transform.position;
        }
        else
            isAbleToShoot = false;

        if (Input.GetMouseButtonDown(0) && isAbleToShoot == true && !camScript.camIsMoving && !(numberHit >= limitHits[(_currentLimitHit)]))
        {

            isBeingHeld = true;
            DragStart();
            
        }

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Dragging();
        }

        if (Input.GetMouseButtonUp(0) && isBeingHeld == true)
        {
            DragRealease();            
        }

        if (isAbleToShoot && rb.velocity.y == 0 && !asWon)
            HitLimit();

        camScript.CamMouvement();

        textHits.text = "" + numberHit + " / " + limitHits[(_currentLimitHit)] + " coups";
    }

    private void DragStart()
    {
        hadHit = false;
        dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lr.positionCount = 1;
        lr.SetPosition(0, dragStartPos);
    }
    private void Dragging()
    {
        hadHit = true;
        if (isBeingHeld == true)
        {
            Vector2 draggingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lr.positionCount = 2;
            lr.SetPosition(1, draggingPos);
        }
    }
    private void DragRealease()
    {
        audioScript.Play("Tir");
        lr.positionCount = 0;

        Vector2 dragReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 force = dragStartPos - dragReleasePos;
        Vector2 clampedForce = Vector2.ClampMagnitude(force, maxDrag) * power;
        rb.AddForce(clampedForce, ForceMode2D.Impulse);

        if (hadHit)
            numberHit++;
        else
            return;
        isBeingHeld = false;
    }
    

    private void HitLimit()
    {
        if (room == 1)
            _currentLimitHit = 1;
        if (room == 2)
            _currentLimitHit = 2;

        if (numberHit >= limitHits[(_currentLimitHit)])
        {
            pitContact++;
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
                audioScript.Play("Victoire");
                camScript.StartMovingCamera();
                trail.SetActive(false);
                sr.DOFade(0, durationFadeInBall);
                StartCoroutine(FadeIn());
                Hole[0].SetActive(false);
            }

            Medals(0);
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
                audioScript.Play("Victoire");
                camScript.startMarker.position = new Vector3(23.53f, 0, -20);
                camScript.endMarker.position = new Vector3(47.06f, 0, -20);
                camScript.StartMovingCamera();
                trail.SetActive(false);
                sr.DOFade(0, durationFadeInBall);
                StartCoroutine(FadeIn());
                Hole[1].SetActive(false);
            }

            Medals(1);

        }

        if (collision.gameObject.tag == "Hole3")
        {
            Debug.Log("Dans le trou");
            holeTime = 0;
            holeTime++;
            if (holeTime == 1)
            {
                audioScript.Play("Victoire");
                sceneScript.LevelEnding();
                Hole[0].SetActive(false);
            }
            Medals(2);
            
        }

        if (collision.gameObject.tag == "Room2")
        {
            room = 1;
       
            numberHit = 0;
            asWon = false;
            recoltedCoinsPerLevel = 0;
        }

        if (collision.gameObject.tag == "Room3")
        {
            room = 2;
           
            numberHit = 0;
            asWon = false;
            recoltedCoinsPerLevel = 0;
        }

        if (collision.gameObject.tag == "Coin")
        {
            KeepingVariables.Instance.recoltedCoins++;
            recoltedCoinsPerLevel++;
            textCoins.text = "" + KeepingVariables.Instance.recoltedCoins;
            Destroy(collision.gameObject);
        }

        foreach (var item in MaterialTypes)
        {
            if (item.NameTag == collision.gameObject.tag)
            {
                if (item.isInPit && pitContact == 0)
                {
                    pitContact++;
                    audioScript.Play(item.NameTag);
                    rb.velocity = Vector3.zero;
                    rb.inertia = 0;
                    StartCoroutine(BallInPit());
                }

                if (item.isInPit == false)
                {
                    audioScript.Play(item.NameTag);
                    rb.sharedMaterial = item.PhysicsMaterial;
                }
            }
        }
    }

    void Medals(int room)
    {
        if ((numberHit <= minHitGold[room]) && (recoltedCoinsPerLevel > minRecoltedCoinGold[room]))
        {
            medal.SetActive(true);
            textMedals.text = "MEDAILLE D'OR";
        }
        else
        {
            medal.SetActive(true);
            textMedals.text = "MEDAILLE D'ARGENT";
        }
        if (numberHit >= minHitSilver[room] && numberHit <= maxHitSilver[room])
        {
            medal.SetActive(true);
            textMedals.text = "MEDAILLE D'ARGENT";
        }
        if (numberHit >= minHitBronze[room] && numberHit < limitHits[(_currentLimitHit)])
        {
            medal.SetActive(true);
            textMedals.text = "MEDAILLE DE BRONZE";
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
        pitContact--;
        isAbleToShoot = true;
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(waitForFade); //le temps que la cam soit arriv?e au level suivant
        medal.SetActive(false);
        if (room == 0)
            ball.transform.position = startPoints[1].transform.position;

        if (room == 1)
            ball.transform.position = startPoints[2].transform.position;
        sr.DOFade(1, 1.5f); // on reset l'alpha de la balle ? 1
        trail.SetActive(true);
    }

    IEnumerator Perdu()
    {
        yield return new WaitForSeconds(waitForLoose);
        DontDestroyOnLoad(audio);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    [Serializable]
    public class MaterialType
    {
        public string NameTag;
        public PhysicsMaterial2D PhysicsMaterial;
        public bool isInPit = false;
    }
}
