using UnityEngine;
using YooAsset;
using System.Collections;
class HotUpdateEntry : MonoBehaviour
{
    ChatMainWin chatMainWin;
    public void Main()
    {
        Debug.Log("׼������������");
        chatMainWin = ScriptableObject.CreateInstance<ChatMainWin>();
        chatMainWin.Begin();
    }
    private void Start()
    {
        Main();
    }
    private void Update()
    {
        chatMainWin.Update();
    }
}
