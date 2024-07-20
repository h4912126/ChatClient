/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemAddFriendLine : GComponent
    {
        public GTextInput m_textInput;
        public GButton m_btnAdd;
        public const string URL = "ui://i49jx3tvf00aa2";

        public static UI_itemAddFriendLine CreateInstance()
        {
            return (UI_itemAddFriendLine)UIPackage.CreateObject("ChatMain", "itemAddFriendLine");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_textInput = (GTextInput)GetChildAt(1);
            m_btnAdd = (GButton)GetChildAt(2);
        }
    }
}