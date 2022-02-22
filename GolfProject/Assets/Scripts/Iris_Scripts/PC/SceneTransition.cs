using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Image Fade;

    private void Start()
    {
        Fade.DOFade(0, 1.5f);
    }
    public void LevelEnding()
    {
        Fade.DOFade(1, 1.5f).OnComplete(FadeComplete);
    }

    public void FadeComplete()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}