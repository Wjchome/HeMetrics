using System.Collections.Generic;

public class BulletManager
{
    private List<Bullet> bullets= new List<Bullet>();

    public void UpdateFrame()
    {
        foreach (Bullet bullet in bullets)
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

    public void AddBullet(Bullet bullet)
    {
        bullets.Add(bullet);
    }

}