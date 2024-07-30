using ChatMain;
using FairyGUI;
using Login;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using YooAsset;

public class LoginMainWin : MonoBehaviour
{
    private List<AssetHandle> _handles = new List<AssetHandle>(100);
    UI_LoginMain loginMain;
    Window win;
    private float screenWidth = Screen.width;
    private float screenHeight = Screen.height;
    // Start is called before the first frame update
    private object LoadFunc(string name, string extension, System.Type type, out DestroyMethod method)
    {
        method = DestroyMethod.None; //注意：这里一定要设置为None
        string location = $"Assets/FairyRes/{name}{extension}";
        var package = YooAssets.GetPackage("DefaultPackage");
        var handle = package.LoadAssetSync(location, type);
        _handles.Add(handle);
        return handle.AssetObject;
    }
    public void Start()
    {
        Init();
        Application.targetFrameRate = 60;
        open();

    }
    void Init()
    {
        UIPackage.AddPackage("Login", LoadFunc);
        LoginBinder.BindAll();
        loginMain = UI_LoginMain.CreateInstance();
        win = new Window();
        win.contentPane = loginMain.asCom;
        loginMain.SetSize(screenWidth, screenHeight);

    }
    void open()
    {

        win.Show();
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void ReleaseHandles()
    {
        foreach (var handle in _handles)
        {
            handle.Release();
        }
        _handles.Clear();
    }
    public void Close()
    {
        ReleaseHandles();
        if (loginMain != null)
        {
            loginMain.Dispose();
            loginMain = null;
        }
    }
    public void SetPage(int index)
    {

        loginMain.m_c1.selectedIndex = index;
    }

    public void SetProgress(int value)
    {
        loginMain.m_loginbar.value = value;
        loginMain.m_textinfo.text = "进度:" + value.ToString() + "%";
    }
}
