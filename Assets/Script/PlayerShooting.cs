using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public GameObject[] bulletPrefabs;
    public Transform[] firePoints;
    public float fireRate = 0.1f;
    public float bulletSpeed = 10f;
    public int maxAmmo = 50;
    private int currentAmmo;
    private float nextFire = 0f;
    private bool isShooting = false;
    private bool reloading = false;
    public float reloadTime = 5f;

    public Slider ammoSlider;
    public Slider reloadSlider;
    public Transform targetObject; // Obiekt, w kierunku którego bêd¹ skierowane pociski

    private int currentFirePointIndex = 0;

    private void Start()
    {
        currentAmmo = maxAmmo;
        ammoSlider.maxValue = maxAmmo;
        reloadSlider.maxValue = reloadTime;
        SetActiveReloadSliderChildren(false); // Hide reload slider children at the start
        reloadSlider.gameObject.SetActive(false); // Hide reload slider at the start
        UpdateAmmoUI();
    }

    private void Update()
    {
        if (reloading)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !isShooting)
        {
            StartCoroutine(ShootContinuously());
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopShooting();
        }
    }

    IEnumerator ShootContinuously()
    {
        isShooting = true;
        while (currentAmmo > 0 && Input.GetKey(KeyCode.Space))
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Shoot();
                SwitchFirePoint();
            }
            yield return null;
        }
        StopShooting();
    }

    void StopShooting()
    {
        isShooting = false;
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        Vector3 shootDirection = (targetObject.position - firePoints[currentFirePointIndex].position).normalized;

        GameObject bullet = Instantiate(bulletPrefabs[currentFirePointIndex], firePoints[currentFirePointIndex].position, Quaternion.LookRotation(shootDirection));
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = shootDirection * bulletSpeed;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    void SwitchFirePoint()
    {
        currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Length;
    }

    IEnumerator Reload()
    {
        reloading = true;
        SetActiveReloadSliderChildren(true); // Show reload slider children
        reloadSlider.gameObject.SetActive(true); // Show reload slider
        float reloadTimer = reloadTime;
        while (reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
            reloadSlider.value = reloadTimer;
            yield return null;
        }
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        reloading = false;
        reloadSlider.value = reloadTime;
        reloadSlider.gameObject.SetActive(false); // Hide reload slider
        SetActiveReloadSliderChildren(false); // Hide reload slider children
    }

    void UpdateAmmoUI()
    {
        ammoSlider.value = maxAmmo - currentAmmo;
    }

    void SetActiveReloadSliderChildren(bool isActive)
    {
        foreach (Transform child in reloadSlider.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }
}
