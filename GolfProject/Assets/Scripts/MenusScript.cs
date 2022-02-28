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

    public static bool IsMusicPlaying = true;

    public GameObject MainCanvas;
    public GameObject OptionsCanvas;

    public Image No;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());
        OptionsCanvas.SetActive(false);

        No.DOFade(0, 0);
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

    public void OnClickAudio()
    {

        Debug.Log(IsMusicPlaying); //Bon si c'est false ça joue la musique et inversement PTDR breff

        if (IsMusicPlaying == false)
        {
            StartCoroutine(AudioOff());
        }

        if (IsMusicPlaying == true)
        {
            StartCoroutine(AudioOn());
        }
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
        SceneManager.LoadScene(1);
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

    IEnumerator AudioOff()
    {
        No.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        IsMusicPlaying = true;
    }

    IEnumerator AudioOn()
    {
        No.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        IsMusicPlaying = false;
    }
}
