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


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());
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
        Fader.DOFade(1, FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
    }

    IEnumerator FadeOut()
    {
        Fader.DOFade(0, FadeOutTime);
        yield return new WaitForSeconds(FadeOutTime);
    }
}
