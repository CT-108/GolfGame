using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Image Fade;
    public GameObject FadeObj;

    private void Start()
    {
        Fade.DOFade(0, 1.5f).OnComplete(EnabledFade);
    }
    public void LevelEnding()
    {
        Fade.DOFade(1, 1.5f).OnComplete(FadeComplete);
    }

    public void FadeComplete()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        EnabledFade();
        Debug.Log("Changement de scènes");
    }

    void EnabledFade()
    {
        FadeObj.SetActive(false);
    }
}
