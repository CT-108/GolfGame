using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenusScript : MonoBehaviour
{

    public SpriteRenderer Fader;

    public int FadeInTime;
    public int FadeOutTime;

    public GameObject MainCanvas;
    public GameObject OptionsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());
        OptionsCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPlay()
    {
        StartCoroutine(FadeInAndPlay());
    }

    public void OnClickOptions()
    {
        StartCoroutine(FadeInOptions());
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickBack()
    {
        StartCoroutine(FadeBackInMenu());
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

    IEnumerator FadeInAndPlay()
    {
        Fader.DOFade(1, FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
        SceneManager.LoadScene(0);
    }

    IEnumerator FadeInOptions()
    {
        Fader.DOFade(1, FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
        MainCanvas.SetActive(false);
        OptionsCanvas.SetActive(true);
        Fader.DOFade(0, FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
    }

    IEnumerator FadeBackInMenu()
    {
        Fader.DOFade(1, FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
        MainCanvas.SetActive(true);
        OptionsCanvas.SetActive(false);
        Fader.DOFade(0, FadeInTime);
        yield return new WaitForSeconds(FadeInTime);
    }
}
