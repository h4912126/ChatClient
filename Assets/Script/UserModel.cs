using System;

[Serializable] // ����������Unity��Inspector�в鿴��༭������ʵ����������������  
public class UserInfo
{
    public string UserID { get; set; } // �û�ID  
    public string Username { get; set; } // �û���  
    public string UserIcon { get; set; } // �����ʼ���ַ  
                                      // �����Ҫ�洢����Ĺ�ϣֵ�����������������  
                                      // public string PasswordHash { get; set; }  

    // ʾ��������һ�����г�ʼֵ�Ĺ��캯��  
    public UserInfo(string name, string userId,string userIcon)
    {
        Username = name;
        UserID = userId;
        UserIcon = userIcon;
        // ����UserID��ͨ��ĳ�ַ�ʽ���ɵģ�����������ʱ������  
    }

    // �����Ҫ��������Ӹ������û���Ϣ��ص����Ժͷ���  
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
        // ����UserID��ͨ��ĳ�ַ�ʽ���ɵģ�����������ʱ������  
    }
}
