/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatBottom : GComponent
    {
        public Controller m_c1;
        public GButton m_btn0;
        public GButton m_btn1;
        public GButton m_btn2;
        public GButton m_btn3;
        public const string URL = "ui://i49jx3tvzcpd9r";

        public static UI_itemChatBottom CreateInstance()
        {
            return (UI_itemChatBottom)UIPackage.CreateObject("ChatMain", "itemChatBottom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c1 = GetControllerAt(0);
            m_btn0 = (GButton)GetChildAt(1);
            m_btn1 = (GButton)GetChildAt(2);
            m_btn2 = (GButton)GetChildAt(3);
            m_btn3 = (GButton)GetChildAt(4);
        }
    }
}