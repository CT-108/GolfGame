using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepingVariables : MonoBehaviour
{
    private static KeepingVariables instance = null;
    public static KeepingVariables Instance => instance;

    public int recoltedCoins;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
