using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 0.33f; // 3 pociski na sekundê
    private Transform player; // referencja do gracza

    void Start()
    {
        // Pobierz referencjê do obiektu gracza
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating("Shoot", 0f, fireRate);
    }

    void Shoot()
    {
        // Oblicz kierunek strza³u w stronê gracza
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;

        // Tworzenie pocisku
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(directionToPlayer));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        // Nadanie pociskowi prêdkoœci w kierunku gracza
        bulletRb.velocity = directionToPlayer * bulletSpeed;
    }
}