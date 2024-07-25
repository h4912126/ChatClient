/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace ChatMain
{
    public partial class UI_itemUserHead : GComponent
    {
        public GLoader m_loaders;
        public const string URL = "ui://i49jx3tvqp799o";

        public static UI_itemUserHead CreateInstance()
        {
            return (UI_itemUserHead)UIPackage.CreateObject("ChatMain", "itemUserHead");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_loaders = (GLoader)GetChildAt(0);
        }
    }
}