/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemChatPage : GComponent
    {
        public GList m_list2;
        public const string URL = "ui://i49jx3tvf00aa0";

        public static UI_itemChatPage CreateInstance()
        {
            return (UI_itemChatPage)UIPackage.CreateObject("ChatMain", "itemChatPage");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_list2 = (GList)GetChildAt(1);
        }
    }
}