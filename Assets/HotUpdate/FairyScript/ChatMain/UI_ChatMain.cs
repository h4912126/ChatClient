/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_ChatMain : GComponent
    {
        public Controller m_c2;
        public UI_itemChat m_itemChat;
        public UI_itemChatMain m_itemChatMain;
        public const string URL = "ui://i49jx3tvf00aa6";

        public static UI_ChatMain CreateInstance()
        {
            return (UI_ChatMain)UIPackage.CreateObject("ChatMain", "ChatMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c2 = GetControllerAt(0);
            m_itemChat = (UI_itemChat)GetChildAt(0);
            m_itemChatMain = (UI_itemChatMain)GetChildAt(1);
        }
    }
}