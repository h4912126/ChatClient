/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatContent : GComponent
    {
        public Controller m_c1;
        public Controller m_showTime;
        public const string URL = "ui://i49jx3tvzcpd9t";

        public static UI_itemChatContent CreateInstance()
        {
            return (UI_itemChatContent)UIPackage.CreateObject("ChatMain", "itemChatContent");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c1 = GetControllerAt(0);
            m_showTime = GetControllerAt(1);
        }
    }
}