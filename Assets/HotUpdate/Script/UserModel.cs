using System;

[Serializable] // 如果你打算在Unity的Inspector中查看或编辑这个类的实例，请添加这个特性  
public class UserInfo
{
    public string UserID { get; set; } // 用户ID  
    public string Username { get; set; } // 用户名  
    public string UserIcon { get; set; } // 电子邮件地址  
                                      // 如果需要存储密码的哈希值，可以添加如下属性  
                                      // public string PasswordHash { get; set; }  

    // 示例：创建一个带有初始值的构造函数  
    public UserInfo(string name, string userId,string userIcon)
    {
        Username = name;
        UserID = userId;
        UserIcon = userIcon;
        // 假设UserID是通过某种方式生成的，这里我们暂时不设置  
    }

    // 如果需要，可以添加更多与用户信息相关的属性和方法  
}


[Serializable]

public class ChatRoom
{
    public string ChatRoomId { get; set; }
    public string ChatRoomName { get; set; }
    public string ChatRoomContent { get; set; }
    public string ChatRoomIcon { get; set; }
    public ChatRoom(string chatRoomId, string chatRoomName, string chatRoomContent,string chatRoomIcon)
    {
        ChatRoomId = chatRoomId;
        ChatRoomName = chatRoomName;
        ChatRoomContent = chatRoomContent;
        ChatRoomIcon = chatRoomIcon;
        // 假设UserID是通过某种方式生成的，这里我们暂时不设置  
    }
}
