/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChat : GComponent
    {
        public UI_itemChatPage m_itemChatPage;
        public UI_itemChatTop m_itemChatTop;
        public const string URL = "ui://i49jx3tvf00aa5";

        public static UI_itemChat CreateInstance()
        {
            return (UI_itemChat)UIPackage.CreateObject("ChatMain", "itemChat");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_itemChatPage = (UI_itemChatPage)GetChildAt(0);
            m_itemChatTop = (UI_itemChatTop)GetChildAt(1);
        }
    }
}