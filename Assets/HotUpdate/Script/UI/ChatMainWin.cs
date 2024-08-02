
using UnityEngine;
using ChatMain;
using FairyGUI;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using YooAsset;

public class ChatMainWin : ScriptableObject
{
    public InputField searchInput;
    TcpLoginClient TcpLogin;
    public Action<bool, string> OnLoginResult;
    UI_ChatMain chatMain;
    Window win;
    private float screenWidth = Screen.width;
    private float screenHeight = Screen.height;
    private string ChatRoomId = "0";
    public bool isConnect = false;
    private List<AssetHandle> _handles = new List<AssetHandle>(100);
    // Start is called before the first frame update
    public void StartGame()
    {

        Init();
        Application.targetFrameRate = 60;
        open();
       
    }
    private object LoadFunc(string name, string extension, System.Type type, out DestroyMethod method)
    {
        method = DestroyMethod.None; //注意：这里一定要设置为None
        string location = $"Assets/FairyRes/{name}{extension}";
        var package = YooAssets.GetPackage("DefaultPackage");
        var handle = package.LoadAssetSync(location, type);
        _handles.Add(handle);
        return handle.AssetObject;
    }
    void Init()
    {
        UIPackage.AddPackage("ChatMain", LoadFunc);
        ChatMainBinder.BindAll();
        chatMain = UI_ChatMain.CreateInstance();
        win = new();
        win.contentPane = chatMain.asCom;
        chatMain.SetSize(screenWidth, screenHeight);
        chatMain.m_itemChatMain.m_itemChatBottom.m_btn0.onClick.Add(()=> {
            chatMain.m_itemChatMain.m_c1.selectedIndex = 0;
        });
        chatMain.m_itemChatMain.m_itemChatBottom.m_btn1.onClick.Add(() => {
            chatMain.m_itemChatMain.m_c1.selectedIndex = 1;
        });
        chatMain.m_itemChatMain.m_itemChatBottom.m_btn2.onClick.Add(() => {
            chatMain.m_itemChatMain.m_c1.selectedIndex = 2;
        });
        chatMain.m_itemChatMain.m_itemChatBottom.m_btn3.onClick.Add(() => {
            chatMain.m_itemChatMain.m_c1.selectedIndex = 3;
        });
        chatMain.m_itemChat.m_itemChatTop.m_chatBack.onClick.Add(() =>
        {

            chatMain.m_c2.selectedIndex = 0;
        });
        chatMain.m_itemChat.m_btnSend.onClick.Add(() =>
        {
            string content = chatMain.m_itemChat.m_textInput.text;
            if (content == null || content == "") {
                Debug.Log("不能输入空字符串");
                return;
            }
            TcpLogin.SendMsgByChatId(ChatRoomId, content);
            chatMain.m_itemChat.m_textInput.text = "";
        });
        chatMain.m_itemChat.m_textInput.onFocusIn.Add(SetKeyboardHeight);
        chatMain.m_itemChat.m_textInput.hideInput = true;
        chatMain.m_itemChat.m_textInput.onFocusOut.Add(SetKeyboardHeight2);
        chatMain.m_itemChatMain.m_list1.itemRenderer = RenderListItem;
        chatMain.m_itemChat.m_itemChatPage.m_list2.SetVirtual();
        chatMain.m_itemChat.m_itemChatPage.m_list2.itemRenderer = RenderListItem2;
        TcpLogin.OnChatRoomResult += OnGetChatRoomResultHandler;
        TcpLogin.OnChatInfoResult += OnChatInfoResultHandler;
        TcpLogin.OnChatAddResult += OnChatAddResultHandler;
    }

    void open() {
        chatMain.m_itemChatMain.m_itemChatBottom.m_c1.selectedIndex = 3;
        chatMain.m_itemChatMain.m_c1.selectedIndex = 3;
        // 连接服务端
        //TcpLogin = new TcpLoginClient();
        //TcpLogin.Login();
        FreshPage3();
        win.Show();
    }
    void OnLoginResultHandler(bool success, string message) {

        if (success)
        {
            Debug.Log("登录成功");
            TcpLogin.GetChatRoomInfo(0,19);
            StartGame();
            FreshPage3();
        }
        else {
            Debug.Log(message);
        }

    }
    public void Begin() {

        Debug.Log("登录成功");
        TcpLogin = ScriptableObject.CreateInstance<TcpLoginClient>();
        TcpLogin.OnLoginResult += OnLoginResultHandler;
        TcpLogin.Login();
        //TcpLogin.GetChatRoomInfo(0, 19);
        //StartGame();
    }
    void OnGetChatRoomResultHandler(bool success, string message) {

        if (success)
        {

            FreshPage1();
        }
        else
        {
            Debug.Log(message);
        }
    }

    void FreshPage3() {
        var userInfo = chatMain.m_itemChatMain.m_itemUserInfo;
        userInfo.m_itemUserHead.m_loaders.url = $"ui://ChatMain/{TcpLogin.userInfo.UserIcon}";
        userInfo.m_textUserName.text = TcpLogin.userInfo.Username;
        userInfo.m_textUserId.text = TcpLogin.userInfo.UserID;

    }
    void FreshPage1()
    {
        chatMain.m_itemChatMain.m_list1.numItems = TcpLogin.chatRooms.Count;
    }
    void RenderListItem(int index, GObject obj) {
        UI_itemChatEntry item = (UI_itemChatEntry)obj;
        item.m_textName.text = TcpLogin.chatRooms[index].ChatRoomName;
        item.m_itemUserHead.m_loaders.url = $"ui://ChatMain/{TcpLogin.chatRooms[index].ChatRoomIcon}";
        string myContent = TcpLogin.chatRooms[index].ChatRoomContent;
        string[] result = myContent.Split('&');
        long nowTimes = 28800 + long.Parse(result[0]);//utc时间手动转当前时区时间
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(nowTimes).DateTime;
        string formattedDateTime = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        item.m_textTime.text = formattedDateTime;
        item.m_textContent.text = result[5];
        item.onClick.Set(() => {
            ChatRoomId = TcpLogin.chatRooms[index].ChatRoomId;
            if (TcpLogin.userChatInfo.ContainsKey($"{ChatRoomId}"))
            {
                var count = TcpLogin.userChatInfo[$"{ChatRoomId}"].Count;
                chatMain.m_itemChat.m_itemChatPage.m_list2.numItems = count;
                chatMain.m_itemChat.m_itemChatPage.m_list2.ScrollToView(count - 1);
                SetKeyboardHeight2();
                chatMain.m_c2.selectedIndex = 1;
            }
            else {
                TcpLogin.GetChatInfoByChatId(ChatRoomId, 0, 99);
            }
            item.m_play.Play();
        }
            
        );
    }
    void RenderListItem2(int index, GObject obj) {

        UI_itemChatContentAndTime item = (UI_itemChatContentAndTime)obj;
        item.width = chatMain.m_itemChat.m_itemChatPage.m_list2.width;
        item.m_showTime.selectedIndex = 1;
        var data = TcpLogin.userChatInfo[ChatRoomId][index];
        long nowTimes = 28800 + long.Parse(data[0]);//utc时间手动转当前时区时间
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(nowTimes).DateTime;
        string formattedDateTime = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        item.m_textTime.text = formattedDateTime;
        var theId = data[1].ToString();
        string context = data[5];
        item.m_itemchatContent.m_textSelf1.text = context; //用于适配高度
        item.m_itemchatContent.m_textOhter1.text = context;
        item.m_itemchatContent.m_textOhter.text = context;
        item.m_itemchatContent.m_textSelf.text = context;
        if (theId == TcpLogin.userInfo.UserID)
        {
            item.m_itemchatContent.m_messagetype.selectedIndex = 0;
            item.m_itemchatContent.m_itemUserHeadSelf.m_loaders.url = $"ui://ChatMain/{data[3]}";
            item.m_itemchatContent.m_textNameSelf.text = data[2];
            if (item.m_itemchatContent.m_textSelf1.width < item.m_itemchatContent.m_textSelf.width)
            {
                item.m_itemchatContent.m_overlength.selectedIndex = 1;
            }
            else {
                item.m_itemchatContent.m_overlength.selectedIndex = 0;
            }
        }
        else {
            item.m_itemchatContent.m_messagetype.selectedIndex = 1;
            item.m_itemchatContent.m_itemUserHeadOther.m_loaders.url = $"ui://ChatMain/{data[3]}";

            item.m_itemchatContent.m_textNameOther.text = data[2];
            if (item.m_itemchatContent.m_textOhter1.width < item.m_itemchatContent.m_textOhter.width)
            {
                item.m_itemchatContent.m_overlength.selectedIndex = 1;
            }
            else
            {
                item.m_itemchatContent.m_overlength.selectedIndex = 0;
            }
        }
        item.height = item.m_itemchatContent.m_pos.y;
    }
    void OnChatInfoResultHandler(bool success, string message) {

        if (success)
        { var count = TcpLogin.userChatInfo[$"{ChatRoomId}"].Count;
            chatMain.m_itemChat.m_itemChatPage.m_list2.numItems = count;
            chatMain.m_itemChat.m_itemChatPage.m_list2.ScrollToView(count-1);
            SetKeyboardHeight2();
            chatMain.m_c2.selectedIndex = 1;
        }
        else
        {
            Debug.Log(message);
        }
    }
    void OnChatAddResultHandler(bool success, string message)
    {

        if (success)
        {
            var count = TcpLogin.userChatInfo[$"{ChatRoomId}"].Count;
            chatMain.m_itemChat.m_itemChatPage.m_list2.numItems = count;
            chatMain.m_itemChat.m_itemChatPage.m_list2.ScrollToView(count-1);
            SetKeyboardHeight2();
            chatMain.m_c2.selectedIndex = 1;
        }
        else
        {
            Debug.Log(message);
        }
    }
    void OnDestroy()
    {
        ReleaseHandles();
        if (chatMain != null)
        {
            chatMain.Dispose();
            chatMain = null;
        }
    }
    private void ReleaseHandles()
    {
        foreach (var handle in _handles)
        {
            handle.Release();
        }
        _handles.Clear();
    }
    // Update is called once per frame
    void Update()
    {
        if (chatMain.m_itemChat.m_textInput.focused ) {
            SetKeyboardHeight();
        }
       
    }
    void SetKeyboardHeight() {
        int mHeight = GetKeyboardHeight();
        if ((screenHeight - mHeight - 20) < (screenHeight - 157))
        {
            chatMain.m_itemChat.m_textInput.y = screenHeight - mHeight - 20;
        }
        else {
            chatMain.m_itemChat.m_textInput.y = screenHeight - 157;
        }
    }
    void SetKeyboardHeight2() {
        chatMain.m_itemChat.m_textInput.y = screenHeight - 157 ;
    }
    /// <summary>
    /// 获取安卓平台上键盘的高度
    /// </summary>
    /// <returns></returns>
    public int GetKeyboardHeight() { 
#if UNITY_EDITOR
        return 180;
#elif UNITY_ANDROID
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").
                Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);
                int res = Screen.height - Rct.Call<int>("height");
                return res;
            }
        }
#endif
    }


}
