using UnityEngine;
using YooAsset;
using System.Collections;
class HotUpdateEntry : MonoBehaviour
{
    public static void Main()
    {
        Debug.Log("准备创建主界面");
        ChatMainWin chatMainWin = ScriptableObject.CreateInstance<ChatMainWin>();
        chatMainWin.Begin();
    }
    private void Start()
    {
        Main();
    }
}
