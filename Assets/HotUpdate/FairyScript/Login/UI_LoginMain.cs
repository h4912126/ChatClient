/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_LoginMain : GComponent
    {
        public Controller m_c1;
        public GProgressBar m_loginbar;
        public GTextField m_textinfo;
        public GTextField m_textState;
        public const string URL = "ui://vpzz49eo9o121";

        public static UI_LoginMain CreateInstance()
        {
            return (UI_LoginMain)UIPackage.CreateObject("Login", "LoginMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_c1 = GetControllerAt(0);
            m_loginbar = (GProgressBar)GetChildAt(1);
            m_textinfo = (GTextField)GetChildAt(2);
            m_textState = (GTextField)GetChildAt(3);
        }
    }
}