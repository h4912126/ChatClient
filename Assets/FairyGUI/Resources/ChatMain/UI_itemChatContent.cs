/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatContent : GComponent
    {
        public Controller m_messagetype;
        public Controller m_overlength;
        public UI_itemUserHead m_itemUserHeadOther;
        public UI_itemUserHead m_itemUserHeadSelf;
        public GTextField m_textOhter;
        public GTextField m_textSelf;
        public GTextField m_textNameOther;
        public GTextField m_textNameSelf;
        public GImage m_pic;
        public GTextField m_textSelf1;
        public GTextField m_textOhter1;
        public GLoader m_pos;
        public const string URL = "ui://i49jx3tvzcpd9t";

        public static UI_itemChatContent CreateInstance()
        {
            return (UI_itemChatContent)UIPackage.CreateObject("ChatMain", "itemChatContent");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_messagetype = GetControllerAt(0);
            m_overlength = GetControllerAt(1);
            m_itemUserHeadOther = (UI_itemUserHead)GetChildAt(0);
            m_itemUserHeadSelf = (UI_itemUserHead)GetChildAt(1);
            m_textOhter = (GTextField)GetChildAt(3);
            m_textSelf = (GTextField)GetChildAt(5);
            m_textNameOther = (GTextField)GetChildAt(6);
            m_textNameSelf = (GTextField)GetChildAt(7);
            m_pic = (GImage)GetChildAt(8);
            m_textSelf1 = (GTextField)GetChildAt(9);
            m_textOhter1 = (GTextField)GetChildAt(11);
            m_pos = (GLoader)GetChildAt(12);
        }
    }
}