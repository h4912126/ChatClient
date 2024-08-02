using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class LoginEntry : MonoBehaviour
{
    public static void Main()
    {
        Debug.Log("准备创建登录界面");
        LoginMainWin loginMainWin = ScriptableObject.CreateInstance<LoginMainWin>();
        loginMainWin.Start();
    }
    private void Start()
    {
        Main();
    }
}

