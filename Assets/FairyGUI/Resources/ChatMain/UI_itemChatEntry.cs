/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatEntry : GComponent
    {
        public Controller m_c1;
        public const string URL = "ui://i49jx3tvqp799p";

        public static UI_itemChatEntry CreateInstance()
        {
            return (UI_itemChatEntry)UIPackage.CreateObject("ChatMain", "itemChatEntry");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c1 = GetControllerAt(0);
        }
    }
}