using FairyGUI;

public class CharacterClickItem : GComponent
{
    public GTextField nameTxt;
    public GList showList;
    public GTextField valueTxt;
    public override void ConstructFromResource()
    {
        base.ConstructFromResource();
        nameTxt = GetChild("Txt_name")as GTextField;
        showList = GetChild("List_show") as GList;
        valueTxt = GetChild("Txt_value")as GTextField;
    }
}