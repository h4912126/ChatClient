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
    // 登录信息  
    public string Username = "user";
    public string Password = "pass";

    // 登录结果回调  
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
                Debug.Log("不支持的设备");
                return;
            }
            // 发送登录请求  
            string loginMessage = "login" + "&" + sign + "\n";
            byte[] data = Encoding.UTF8.GetBytes(loginMessage);
            stream.Write(data, 0, data.Length);
            ListenMessage();
        }
        catch (Exception ex)
        {
            Debug.LogError("登录失败: " + ex.Message);
            if (OnLoginResult != null)
            {
                OnLoginResult(false, "连接错误: " + ex.Message);
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
            Debug.LogError("获取聊天室失败: " + ex.Message);
            if (OnChatRoomResult != null)
            {
                OnChatRoomResult(false, "连接错误: " + ex.Message);
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
            Debug.LogError("获取聊天信息失败: " + ex.Message);
            OnChatInfoResult?.Invoke(false, "连接错误: " + ex.Message);
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
            Debug.LogError("发送聊天信息失败: " + ex.Message);
            OnChatInfoResult?.Invoke(false, "连接错误: " + ex.Message);
        }
    }
    // 清理资源（可选，因为我们在ConnectAndLogin中已经关闭了连接）  
    private void OnDestroy()
    {
        // 如果需要，可以在这里添加额外的清理代码  
    }


    private async Task ListenForMessages()
    {
        try
        {
            while (tcpClient.Connected) // 当连接仍然打开时循环  
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // 如果没有读取到数据，可能连接已关闭  
                HandleReceivedData(buffer, bytesRead);

            }
        }
        catch (Exception ex)
        {
            Debug.LogError("监听消息时出错: " + ex.Message);
        }

        // 如果退出循环，则关闭连接（这里只是示例，你可能希望有不同的行为）  
        CloseConnection();
    }
    public void HandleReceivedData(byte[] buffer, int bytesRead)
    {
        byte[] receivedBytes = new byte[bytesRead];
        Array.Copy(buffer, 0, receivedBytes, 0, bytesRead); 
        remainingData.AddRange(receivedBytes);
        // 将新读取的数据添加到剩余数据中  
        // 尝试从剩余数据中处理尽可能多的完整消息  
        while (TryProcessMessage(ref remainingData))
        {
            // 如果TryProcessMessage返回true，表示处理了一个完整的消息  
            // 可以继续尝试处理下一个消息  
        }

        // 此时remainingData中可能还有剩余数据，但这没关系  
        // 下一次HandleReceivedData调用时，新数据会被添加到remainingData的末尾  
    }

    private bool TryProcessMessage(ref List<byte> data)
    {
        if (data.Count < 2) return false; // 没有足够的数据来读取长度  

        // 读取前两个字节作为长度  
        byte[] lengthBytes = data.GetRange(0, 2).ToArray();
        int length = lengthBytes[0]*256 + lengthBytes[1]; // 假设长度是16位的  

        // 检查是否有足够的数据来完成消息的读取  
        if (data.Count < length + 2) return false; // 不足够的数据  

        // 提取消息内容  
        byte[] messageBytes = data.GetRange(2, length).ToArray();

        // 从remainingData中移除已处理的消息（包括长度字节和消息内容）  
        data.RemoveRange(0, length + 2);

        // 处理消息（例如，转换为字符串并打印）  
        string message = Encoding.UTF8.GetString(messageBytes);
        Console.WriteLine("Received message: " + message);
        // 可以在这里调用其他处理函数，比如HandleMessage(message);  
        HandleMessage(message);
        return true; // 表示处理了一个完整的消息  
    }
    private void HandleMessage(string message)
    {
        // 在这里处理接收到的消息  
        Debug.Log("Received: " + message);

        // 根据消息内容执行相应的操作...  
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
                // 在这里更新UI  
                Debug.Log("Updating UI from main thread");
                OnLoginResult?.Invoke(true, "获取用户信息成功");
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
                // 在这里更新UI  
                OnChatRoomResult?.Invoke(true, "获取聊天室成功");
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
                // 在这里更新UI  
                OnChatInfoResult?.Invoke(true, "获取聊天信息成功");
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
                // 在这里更新UI  
                OnChatAddResult?.Invoke(true, "获取到新的聊天信息");
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

    // 修改Start方法以启动监听任务  

    public async void ListenMessage()
    {
        try
        { 
            if (tcpClient != null && tcpClient.Connected)
            {
                _mainThreadContext = SynchronizationContext.Current;
                // 在后台任务中监听消息  
                await Task.Run(ListenForMessages);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("启动连接和监听时出错: " + ex.Message);
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
    // ...（其他方法保持不变） 
}