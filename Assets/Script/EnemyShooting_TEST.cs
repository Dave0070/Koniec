using UnityEngine;

public class EnemyShooting_TEST : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float minShootTime = 3f;
    public float maxShootTime = 7f;
    public float minBreakTime = 3f;
    public float maxBreakTime = 6f;
    public float fireRate = 0.33f; // 3 pociski na sekundê

    void Start()
    {
        InvokeRepeating("RandomizedShoot", 0f, 1f);
        InvokeRepeating("Shoot", 0f, fireRate);
    }

    void RandomizedShoot()
    {
        float randomTime = Random.Range(minShootTime, maxShootTime);
        InvokeRepeating("Shoot", 0f, 1f / randomTime);
        Invoke("StopShooting", randomTime);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);

    }

    void StopShooting()
    {
        CancelInvoke("Shoot");
        float randomBreakTime = Random.Range(minBreakTime, maxBreakTime);
        Invoke("RandomizedShoot", randomBreakTime);
    }
}