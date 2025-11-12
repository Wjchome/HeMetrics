using System.Collections.Generic;

public class Bullet1Manager
{
    private List<Bullet1> bullets= new List<Bullet1>();

    public void UpdateFrame()
    {
        foreach (Bullet1 bullet in bullets)
        {
            bullet.UpdateFrame();
        }
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            if (bullets[i].isDead)
            {
                bullets[i].LogicDead();
                bullets.RemoveAt(i); // 用 RemoveAt 更高效（无需查找元素）
            }
        }
    }

    public void AddBullet1(Bullet1 bullet)
    {
        bullets.Add(bullet);
    }

}