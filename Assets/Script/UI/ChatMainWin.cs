
using UnityEngine;
using ChatMain;
using FairyGUI;
using System;
using System.Collections;

public class ChatMainWin : MonoBehaviour
{
    TcpLoginClient TcpLogin;
    public Action<bool, string> OnLoginResult;
    UI_ChatMain chatMain;
    Window win;
    private float screenWidth = Screen.width;
    private float screenHeight = Screen.height;
    private string ChatRoomId = "0";
    public bool isConnect = false;
    private void Awake()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {

        open();
       
    }
    void Init()
    {
        UIPackage.AddPackage("ChatMain");
        ChatMainBinder.BindAll();
        chatMain = UI_ChatMain.CreateInstance();
        win = new Window();
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
        chatMain.m_itemChatMain.m_list1.itemRenderer = RenderListItem;
        chatMain.m_itemChat.m_itemChatPage.m_list2.itemRenderer = RenderListItem2;
    }

    void open() {
        chatMain.m_itemChatMain.m_itemChatBottom.m_c1.selectedIndex = 3;
        chatMain.m_itemChatMain.m_c1.selectedIndex = 3;
        // 连接服务端
        TcpLogin = new TcpLoginClient();
        TcpLogin.Login();
        TcpLogin.OnLoginResult += OnLoginResultHandler;
        TcpLogin.OnChatRoomResult += OnGetChatRoomResultHandler;
        TcpLogin.OnChatInfoResult += OnChatInfoResultHandler;
        TcpLogin.OnChatAddResult += OnChatAddResultHandler;
        win.Show();
    }
    void OnLoginResultHandler(bool success, string message) {

        if (success)
        {
            Debug.Log("登录成功");
            TcpLogin.GetChatRoomInfo(0,19);
            FreshPage3();
        }
        else {
            Debug.Log(message);
        }

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
                chatMain.m_c2.selectedIndex = 1;
            }
            else {
                TcpLogin.GetChatInfoByChatId(ChatRoomId, 0, 99);
            }


            item.m_c1.selectedIndex = 1;
            IEnumerator MyCoroutine()
            {
                Debug.Log("Coroutine started");
                yield return new WaitForSeconds(1); // 等待1秒  
                Debug.Log("Coroutine finished waiting");
                item.m_c1.selectedIndex = 0;
            }
            StartCoroutine(MyCoroutine());

        }
            
        );
    }
    void RenderListItem2(int index, GObject obj) {
        UI_itemChatContentAndTime item = (UI_itemChatContentAndTime)obj;
        item.m_showTime.selectedIndex = 1;
        var data = TcpLogin.userChatInfo[ChatRoomId][index];
        long nowTimes = 28800 + long.Parse(data[0]);//utc时间手动转当前时区时间
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(nowTimes).DateTime;
        string formattedDateTime = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        item.m_textTime.text = formattedDateTime;
        var theId = data[1].ToString();
        if (theId == TcpLogin.userInfo.UserID)
        {
            item.m_itemchatContent.m_messagetype.selectedIndex = 0;
            item.m_itemchatContent.m_itemUserHeadSelf.m_loaders.url = $"ui://ChatMain/{data[3]}";
            item.m_itemchatContent.m_textSelf.text = data[5];
            item.m_itemchatContent.m_textNameSelf.text = data[2];
        }
        else {
            item.m_itemchatContent.m_messagetype.selectedIndex = 1;
            item.m_itemchatContent.m_itemUserHeadOther.m_loaders.url = $"ui://ChatMain/{data[3]}";
            item.m_itemchatContent.m_textOhter.text = data[5];
            item.m_itemchatContent.m_textNameOther.text = data[2];
        }


    }
    void OnChatInfoResultHandler(bool success, string message) {

        if (success)
        { var count = TcpLogin.userChatInfo[$"{ChatRoomId}"].Count;
            chatMain.m_itemChat.m_itemChatPage.m_list2.numItems = count;
            chatMain.m_itemChat.m_itemChatPage.m_list2.ScrollToView(count-1);
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
            chatMain.m_c2.selectedIndex = 1;
        }
        else
        {
            Debug.Log(message);
        }
    }
    void OnDestroy()
    {
        if (chatMain != null)
        {
            chatMain.Dispose();
            chatMain = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
