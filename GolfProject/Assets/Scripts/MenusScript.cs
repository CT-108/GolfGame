using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenusScript : MonoBehaviour
{

    public SpriteRenderer Fader;

    public int FadeInTime;

    public int FadeOutTime;

    public int FadeInAlpha; //Value used when it goes from Black to Transparent

    public int FadeOutAlpha; //Value used when it goes from Transparent to Black


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPlay()
    {

    }

    public void OnClickOptions()
    {

    }

    public void OnClickExit()
    {

    }

    IEnumerator FadeIn()
    {
        Fader.DOFade(FadeInAlpha, FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
    }

    IEnumerator FadeOut()
    {
        Fader.DOFade(FadeOutAlpha, FadeOutTime);
        yield return new WaitForSeconds(FadeOutTime);
    }
}
