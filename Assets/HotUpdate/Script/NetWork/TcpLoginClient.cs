using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading;

[Serializable]
public class PlayerData
{
    public string name;
    public string addr;
    public string userId;
    public string userIcon;

}

public class TcpLoginClient : ScriptableObject
{

    private SynchronizationContext _mainThreadContext;
    private List<byte> remainingData = new();
    private TcpClient tcpClient;
    private NetworkStream stream;
    public List<ChatRoom> chatRooms = new() ;
    public Dictionary<string, List<string[]>> userChatInfo = new();
    public string ServerIP = "2409:8a62:e42:5a70:b993:170:1eb7:d32f";
    public int ServerPort = 9527;
    // ��¼��Ϣ  
    public string Username = "user";
    public string Password = "pass";

    // ��¼����ص�  
    public Action<bool, string> OnLoginResult;
    public Action<bool, string> OnChatRoomResult;
    public Action<bool, string> OnChatInfoResult;
    public Action<bool, string> OnChatAddResult;
    public UserInfo userInfo;

    private void ConnectAndLogin()
    {
        try
        {
            tcpClient = new TcpClient(ServerIP, ServerPort);
            stream = tcpClient.GetStream();
            var sign = GetUserSign();
            if (sign == "") {
                Debug.Log("��֧�ֵ��豸");
                return;
            }
            // ���͵�¼����  
            string loginMessage = "login" + "&" + sign + "\n";
            byte[] data = Encoding.UTF8.GetBytes(loginMessage);
            stream.Write(data, 0, data.Length);
            ListenMessage();
        }
        catch (Exception ex)
        {
            Debug.LogError("��¼ʧ��: " + ex.Message);
            if (OnLoginResult != null)
            {
                OnLoginResult(false, "���Ӵ���: " + ex.Message);
            }
        }
    }
    public async Task SynGetChatRoomInfo(int start ,int end)
    {
        try
        {
            string getChatIds = "getChatIds" + "&"+ start.ToString() + "&" + end.ToString() + "\n";
            byte[] data = Encoding.UTF8.GetBytes(getChatIds);
            await stream.WriteAsync(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError("��ȡ������ʧ��: " + ex.Message);
            if (OnChatRoomResult != null)
            {
                OnChatRoomResult(false, "���Ӵ���: " + ex.Message);
            }
        }
    }
    public void GetChatInfoByChatId(string chatId,int start,int end) {
        try
        {

            string getChatIds = "getChatInfo" + "&" + chatId + "&" + start.ToString() + "&" + end.ToString() + "\n";
            byte[] data = Encoding.UTF8.GetBytes(getChatIds);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError("��ȡ������Ϣʧ��: " + ex.Message);
            OnChatInfoResult?.Invoke(false, "���Ӵ���: " + ex.Message);
        }

    }
    public void SendMsgByChatId(string chatId,string msg) {
        try
        {
            string chatmsg = "chat" + "&" + chatId + "&"+ msg + "\n";
            byte[] data = Encoding.UTF8.GetBytes(chatmsg);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError("����������Ϣʧ��: " + ex.Message);
            OnChatInfoResult?.Invoke(false, "���Ӵ���: " + ex.Message);
        }
    }
    // ������Դ����ѡ����Ϊ������ConnectAndLogin���Ѿ��ر������ӣ�  
    private void OnDestroy()
    {
        // �����Ҫ��������������Ӷ�����������  
    }


    private async Task ListenForMessages()
    {
        try
        {
            while (tcpClient.Connected) // ��������Ȼ��ʱѭ��  
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // ���û�ж�ȡ�����ݣ����������ѹر�  
                HandleReceivedData(buffer, bytesRead);

            }
        }
        catch (Exception ex)
        {
            Debug.LogError("������Ϣʱ����: " + ex.Message);
        }

        // ����˳�ѭ������ر����ӣ�����ֻ��ʾ���������ϣ���в�ͬ����Ϊ��  
        CloseConnection();
    }
    public void HandleReceivedData(byte[] buffer, int bytesRead)
    {
        byte[] receivedBytes = new byte[bytesRead];
        Array.Copy(buffer, 0, receivedBytes, 0, bytesRead); 
        remainingData.AddRange(receivedBytes);
        // ���¶�ȡ��������ӵ�ʣ��������  
        // ���Դ�ʣ�������д������ܶ��������Ϣ  
        while (TryProcessMessage(ref remainingData))
        {
            // ���TryProcessMessage����true����ʾ������һ����������Ϣ  
            // ���Լ������Դ�����һ����Ϣ  
        }

        // ��ʱremainingData�п��ܻ���ʣ�����ݣ�����û��ϵ  
        // ��һ��HandleReceivedData����ʱ�������ݻᱻ��ӵ�remainingData��ĩβ  
    }

    private bool TryProcessMessage(ref List<byte> data)
    {
        if (data.Count < 2) return false; // û���㹻����������ȡ����  

        // ��ȡǰ�����ֽ���Ϊ����  
        byte[] lengthBytes = data.GetRange(0, 2).ToArray();
        int length = lengthBytes[0]*256 + lengthBytes[1]; // ���賤����16λ��  

        // ����Ƿ����㹻�������������Ϣ�Ķ�ȡ  
        if (data.Count < length + 2) return false; // ���㹻������  

        // ��ȡ��Ϣ����  
        byte[] messageBytes = data.GetRange(2, length).ToArray();

        // ��remainingData���Ƴ��Ѵ������Ϣ�����������ֽں���Ϣ���ݣ�  
        data.RemoveRange(0, length + 2);

        // ������Ϣ�����磬ת��Ϊ�ַ�������ӡ��  
        string message = Encoding.UTF8.GetString(messageBytes);
        Console.WriteLine("Received message: " + message);
        // �����������������������������HandleMessage(message);  
        HandleMessage(message);
        return true; // ��ʾ������һ����������Ϣ  
    }
    private void HandleMessage(string message)
    {
        // �����ﴦ����յ�����Ϣ  
        Debug.Log("Received: " + message);

        // ������Ϣ����ִ����Ӧ�Ĳ���...  
        JObject jsonData = JObject.Parse(message);
        string method = (string)jsonData["method"];
        var js = jsonData["msg"];
        if (method == "loginPara")
        {
            var name = js["name"];
            var userId = js["userId"];
            var userIcon = js["userIcon"];
            userInfo = new UserInfo(name.ToString(), userId.ToString(), userIcon.ToString());
            _mainThreadContext.Post(new SendOrPostCallback(o =>
            {
                // ���������UI  
                Debug.Log("Updating UI from main thread");
                OnLoginResult?.Invoke(true, "��ȡ�û���Ϣ�ɹ�");
            }), null);

        }
        else if (method == "ChatIdsPara")
        {
            _mainThreadContext.Post(new SendOrPostCallback(o =>
            {
                foreach (var item in js)
                {
                    var chatId = item["chatRoomId"];
                    var chatName = item["chatRoomName"];
                    var chatIcon = item["chatRoomIcon"];
                    var chatContent = item["chatRoomContent"];
                    var chatRoom = new ChatRoom(chatId.ToString(), chatName.ToString(), chatContent.ToString(), chatIcon.ToString());
                    chatRooms.Add(chatRoom);
                }
                // ���������UI  
                OnChatRoomResult?.Invoke(true, "��ȡ�����ҳɹ�");
            }), null);
        }
        else if (method == "ChatInfoPara") {

            _mainThreadContext.Post(new SendOrPostCallback(o =>
            {
                string id = "0";
                List<string[]> chatList = new();
                foreach (var item in js)
                {
                    string str = item.ToString();
                    string[] strL = str.Split('&');
                    id = strL[4];
                    chatList.Add(strL);
                }
                userChatInfo[id] = chatList;
                // ���������UI  
                OnChatInfoResult?.Invoke(true, "��ȡ������Ϣ�ɹ�");
            }), null);
        }
        else if (method == "chatPara")
        {

            _mainThreadContext.Post(new SendOrPostCallback(o =>
            {
                string str = js.ToString();
                string[] strL = str.Split('&');
                string id = strL[4];
                userChatInfo[id].Add(strL);
                // ���������UI  
                OnChatAddResult?.Invoke(true, "��ȡ���µ�������Ϣ");
            }), null);
        }

    }

    private void CloseConnection()
    {
        if (stream != null)
        {
            stream.Close();
        }
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }

    // �޸�Start������������������  

    public async void ListenMessage()
    {
        try
        { 
            if (tcpClient != null && tcpClient.Connected)
            {
                _mainThreadContext = SynchronizationContext.Current;
                // �ں�̨�����м�����Ϣ  
                await Task.Run(ListenForMessages);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("�������Ӻͼ���ʱ����: " + ex.Message);
        }
    }
    public void Login()
    {
        ConnectAndLogin();
    }
    public async void GetChatRoomInfo(int start,int end)
    {
        await SynGetChatRoomInfo(start,end);
    }
    public string GetUserSign()
    {
#if UNITY_EDITOR
        return "Unity";
#elif UNITY_ANDROID
        return SystemInfo.deviceUniqueIdentifier;
#else 
    return "";
#endif


    }
    // ...�������������ֲ��䣩 
}