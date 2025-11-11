public class NetManager
{
    public long serverTimer;

    public void Init()
    {
        serverTimer = 0;
    }

    public void UpdateFrame()
    {
        serverTimer++;
    }
}