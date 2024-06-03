using UnityEngine;

public class EnemyShootingTest1 : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float bulletSpeed = 10f;
    public float fireRate = 0.33f;
    public int shotsBeforePause = 10;
    public float pauseTime = 5f;

    private Transform player;
    private int shotCounter = 0;
    private bool isPaused = false;

    // Referencja do skryptu EnemyShip
    private EnemyShip enemyShipScript;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Pobierz referencjê do skryptu EnemyShip na tym samym obiekcie
        enemyShipScript = GetComponent<EnemyShip>();

        InvokeRepeating("Shoot", 0f, fireRate);
    }

    void Shoot()
    {
        if (!isPaused)
        {
            foreach (Transform firePoint in firePoints)
            {
                Vector3 directionToPlayer = (player.position - firePoint.position).normalized;

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(directionToPlayer));
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                bulletRb.velocity = directionToPlayer * bulletSpeed;
            }

            shotCounter++;

            if (shotCounter >= shotsBeforePause)
            {
                // Zatrzymaj strzelanie i poinformuj skrypt EnemyShip
                isPaused = true;
                enemyShipScript.StopShooting();
                Invoke("ResumeShooting", pauseTime);
            }
        }
    }

    void ResumeShooting()
    {
        // Wznów strzelanie i poinformuj skrypt EnemyShip
        isPaused = false;
        enemyShipScript.StartShooting();
        shotCounter = 0;
    }
}