using FairyGUI;

public class BondItem : GComponent
{
    public GTextField nameText;
    public GTextField countText;
    public GTextField levelsText;

    public override void ConstructFromResource()
    {
        base.ConstructFromResource();

        nameText = GetChild("Txt_name") as GTextField;
        countText = GetChild("Txt_count") as GTextField;
        levelsText = GetChild("Txt_levels") as GTextField;
    }
}