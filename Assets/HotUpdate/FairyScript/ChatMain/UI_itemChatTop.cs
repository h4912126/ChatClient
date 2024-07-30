/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatTop : GComponent
    {
        public Controller m_c1;
        public GTextField m_textFriendNmae;
        public GButton m_chatBack;
        public const string URL = "ui://i49jx3tvzcpd9s";

        public static UI_itemChatTop CreateInstance()
        {
            return (UI_itemChatTop)UIPackage.CreateObject("ChatMain", "itemChatTop");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c1 = GetControllerAt(0);
            m_textFriendNmae = (GTextField)GetChildAt(2);
            m_chatBack = (GButton)GetChildAt(3);
        }
    }
}