using System.IO;
using SimpleJSON;

public class LuBanDataManager
{
    private Tables tables;

    public void Init()
    {
        string gameConfDir = "Assets/Scripts/Luban/data";
        tables = new Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
    }

    public TbCharacterData CharacterData()
    {
        return tables.TbCharacterData;
    }
}