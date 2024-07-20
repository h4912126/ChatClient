/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemUserInfo : GComponent
    {
        public GComponent m_itemUserHead;
        public GTextField m_textUserName;
        public GTextField m_textUserId;
        public const string URL = "ui://i49jx3tvf00aa3";

        public static UI_itemUserInfo CreateInstance()
        {
            return (UI_itemUserInfo)UIPackage.CreateObject("ChatMain", "itemUserInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_itemUserHead = (GComponent)GetChildAt(2);
            m_textUserName = (GTextField)GetChildAt(3);
            m_textUserId = (GTextField)GetChildAt(4);
        }
    }
}