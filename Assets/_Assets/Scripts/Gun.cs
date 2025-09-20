using UnityEngine;

public class Gun : MonoBehaviour
{
    public ParticleSystem bulletMuzzle;
    public Transform bulletSpawnPos;
    public float fireRate;
    [SerializeField] private ParticleSystem waterSpray;

    private int[] spreadAngles = new[] {-2,-1,0,2,3};

    public void Shoot(Transform target, int index = 0)
    {
        if (index == 0)
        {
            Bullet bullet = ObjectPooling.Instance.Spawn<Bullet>(PoolType.BulletGreen,bulletSpawnPos.position);
            if (bullet != null)
            {
                bullet.Launch(target);
            }
        }
        if (index == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                Bullet bullet = ObjectPooling.Instance.Spawn<Bullet>(PoolType.BulletRed,bulletSpawnPos.position);
                if (bullet != null)
                {
                    bullet.Launch(Quaternion.AngleAxis(spreadAngles[i],Vector3.up) * target.position);
                }
            }
        }
        if (index == 2)
        {
            Bullet bullet = ObjectPooling.Instance.Spawn<Bullet>(PoolType.missile,bulletSpawnPos.position);
            if (bullet != null)
            {
                bullet.Launch(target);
            }
        }
        bulletMuzzle.Play();
    }

    public void StartSpray()
    {
        bulletMuzzle.Play();
        waterSpray.Play();
    }

    public void StopSpray()
    {
        bulletMuzzle.Stop();
        waterSpray.Stop();
    }
}
