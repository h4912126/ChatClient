using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class LoginEntry : MonoBehaviour
{
    public static void Main()
    {
        Debug.Log("׼��������¼����");
        LoginMainWin loginMainWin = ScriptableObject.CreateInstance<LoginMainWin>();
        loginMainWin.Start();
    }
    private void Start()
    {
        Main();
    }
}

