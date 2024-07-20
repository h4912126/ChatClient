/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatContentAndTime : GComponent
    {
        public Controller m_showTime;
        public const string URL = "ui://i49jx3tvf00aa7";

        public static UI_itemChatContentAndTime CreateInstance()
        {
            return (UI_itemChatContentAndTime)UIPackage.CreateObject("ChatMain", "itemChatContentAndTime");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_showTime = GetControllerAt(0);
        }
    }
}