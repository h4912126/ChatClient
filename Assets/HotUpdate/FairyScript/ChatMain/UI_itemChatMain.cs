/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatMain : GComponent
    {
        public Controller m_c1;
        public UI_itemChatBottom m_itemChatBottom;
        public UI_itemChatTop m_itemChatTop;
        public GList m_list1;
        public UI_itemAddFriendLine m_itemAddFriendLine;
        public UI_itemUserInfo m_itemUserInfo;
        public const string URL = "ui://i49jx3tvotn10";

        public static UI_itemChatMain CreateInstance()
        {
            return (UI_itemChatMain)UIPackage.CreateObject("ChatMain", "itemChatMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c1 = GetControllerAt(0);
            m_itemChatBottom = (UI_itemChatBottom)GetChildAt(1);
            m_itemChatTop = (UI_itemChatTop)GetChildAt(2);
            m_list1 = (GList)GetChildAt(3);
            m_itemAddFriendLine = (UI_itemAddFriendLine)GetChildAt(4);
            m_itemUserInfo = (UI_itemUserInfo)GetChildAt(5);
        }
    }
}