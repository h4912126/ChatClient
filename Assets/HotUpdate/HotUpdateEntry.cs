using UnityEngine;
using YooAsset;
using System.Collections;
class HotUpdateEntry : MonoBehaviour
{
    public static void Main()
    {
        Debug.Log("׼������������");
        ChatMainWin chatMainWin = ScriptableObject.CreateInstance<ChatMainWin>();
        chatMainWin.Begin();
    }
    private void Start()
    {
        Main();
    }
}
