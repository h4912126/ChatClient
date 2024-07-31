/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatEntry : GComponent
    {
        public UI_itemUserHead m_itemUserHead;
        public GTextField m_textName;
        public GTextField m_textContent;
        public GTextField m_textTime;
        public Transition m_play;
        public const string URL = "ui://i49jx3tvqp799p";

        public static UI_itemChatEntry CreateInstance()
        {
            return (UI_itemChatEntry)UIPackage.CreateObject("ChatMain", "itemChatEntry");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_itemUserHead = (UI_itemUserHead)GetChildAt(1);
            m_textName = (GTextField)GetChildAt(3);
            m_textContent = (GTextField)GetChildAt(4);
            m_textTime = (GTextField)GetChildAt(5);
            m_play = GetTransitionAt(0);
        }
    }
}