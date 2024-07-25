/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatEntry : GComponent
    {
        public Controller m_c1;
        public UI_itemUserHead m_itemUserHead;
        public GTextField m_textName;
        public GTextField m_textContent;
        public GTextField m_textTime;
        public const string URL = "ui://i49jx3tvqp799p";

        public static UI_itemChatEntry CreateInstance()
        {
            return (UI_itemChatEntry)UIPackage.CreateObject("ChatMain", "itemChatEntry");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c1 = GetControllerAt(0);
            m_itemUserHead = (UI_itemUserHead)GetChildAt(1);
            m_textName = (GTextField)GetChildAt(3);
            m_textContent = (GTextField)GetChildAt(4);
            m_textTime = (GTextField)GetChildAt(5);
        }
    }
}