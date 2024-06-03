using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HealthBasedObject
{
    public GameObject objectToActivateOrDeactivate;
    public float activationHealthThreshold;
}

[System.Serializable]
public class SliderColorGradient
{
    public Gradient gradient;
}

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private List<HealthBasedObject> healthBasedObjects;
    [SerializeField] private GameObject initialBoundaryMesh; // Pocz¹tkowy Mesh z MeshColliderem

    public HealthBar healthBar;
    public GameObject deathScreen;

    [Header("Proximity Sliders")]
    public Slider proximitySliderXZ;
    public Slider proximitySliderY;

    [Header("Proximity Slider Colors")]
    public SliderColorGradient gradientXZ;
    public SliderColorGradient gradientY;

    private float currentHealth;
    private bool isOutsideInitialBoundary = false;
    private MeshCollider boundaryCollider;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);
        healthBar.SetSlider(0); // Set the health bar slider to its minimum value at the start

        // Przypisanie pocz¹tkowego mesh'a z MeshColliderem
        AssignBoundaryMesh(initialBoundaryMesh);
    }

    private void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (currentHealth <= 0 || isOutsideInitialBoundary)
        {
            Die();
        }

        foreach (var healthBasedObject in healthBasedObjects)
        {
            if (currentHealth <= healthBasedObject.activationHealthThreshold && healthBasedObject.objectToActivateOrDeactivate != null)
            {
                healthBasedObject.objectToActivateOrDeactivate.SetActive(true);
            }
            else
            {
                healthBasedObject.objectToActivateOrDeactivate.SetActive(false);
            }
        }

        UpdateProximitySliders();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();
    }

    public void HealPlayer(float amount)
    {
        currentHealth += amount;
        UpdateHealthUI();
    }

    public void Die()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        Debug.Log("You died!");
        deathScreen.SetActive(true);
    }

    private IEnumerator CheckOutsideInitialBoundaryCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // SprawdŸ co sekundê

            Vector3 closestPoint = boundaryCollider.ClosestPoint(transform.position);

            if (closestPoint == transform.position)
            {
                if (isOutsideInitialBoundary)
                {
                    Debug.Log("Player is inside the boundary.");
                    isOutsideInitialBoundary = false;
                }
            }
            else
            {
                if (!isOutsideInitialBoundary)
                {
                    Debug.Log("Player is outside the boundary.");
                    isOutsideInitialBoundary = true;
                }
            }
        }
    }

    public void AssignBoundaryMesh(GameObject boundaryMesh)
    {
        initialBoundaryMesh = boundaryMesh;

        boundaryCollider = initialBoundaryMesh.GetComponent<MeshCollider>();
        MeshFilter meshFilter = initialBoundaryMesh.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            boundaryCollider.sharedMesh = meshFilter.mesh;
        }

        StartCoroutine(CheckOutsideInitialBoundaryCoroutine());
    }

    private void UpdateProximitySliders()
    {
        UpdateProximitySliderXZ();
        UpdateProximitySliderY();
    }

    private void UpdateProximitySliderXZ()
    {
        Vector3 playerPosition = transform.position;

        // Oblicz odleg³oœci gracza od œcian granicy wzd³u¿ osi X i Z
        float distanceToMinX = Mathf.Abs(playerPosition.x - boundaryCollider.bounds.min.x);
        float distanceToMaxX = Mathf.Abs(playerPosition.x - boundaryCollider.bounds.max.x);
        float distanceToMinZ = Mathf.Abs(playerPosition.z - boundaryCollider.bounds.min.z);
        float distanceToMaxZ = Mathf.Abs(playerPosition.z - boundaryCollider.bounds.max.z);

        // ZnajdŸ minimaln¹ odleg³oœæ od granicy w p³aszczyŸnie XZ
        float minDistanceXZ = Mathf.Min(distanceToMinX, distanceToMaxX, distanceToMinZ, distanceToMaxZ);
        float maxDistanceXZ = Mathf.Max(boundaryCollider.bounds.size.x, boundaryCollider.bounds.size.z) / 2;

        // Oblicz fill dla slidera XZ na podstawie odleg³oœci od granicy
        float fillAmountXZ = 1 - Mathf.Clamp01(minDistanceXZ / maxDistanceXZ);
        proximitySliderXZ.value = fillAmountXZ;
        UpdateSliderColor(proximitySliderXZ, fillAmountXZ, gradientXZ.gradient);
    }

    private void UpdateProximitySliderY()
    {
        Vector3 playerPosition = transform.position;

        // Oblicz odleg³oœæ gracza od dolnej i górnej krawêdzi granicy wzd³u¿ osi Y
        float distanceToBottom = Mathf.Abs(playerPosition.y - boundaryCollider.bounds.min.y);
        float distanceToTop = Mathf.Abs(playerPosition.y - boundaryCollider.bounds.max.y);

        // Oblicz fill dla slidera Y na podstawie odleg³oœci od dolnej krawêdzi granicy
        float fillAmountY = Mathf.Clamp01(distanceToTop / (distanceToBottom + distanceToTop)); // Wartoœæ fillAmount w zakresie 0-1
        proximitySliderY.value = fillAmountY;
        UpdateSliderColor(proximitySliderY, fillAmountY, gradientY.gradient);
    }

    private void UpdateSliderColor(Slider slider, float fillAmount, Gradient gradient)
    {
        Color color = gradient.Evaluate(fillAmount);
        slider.fillRect.GetComponent<Image>().color = color;
        // Dodatkowo, zmieniamy kolor background'u slidera na transparentny, aby gradient fill by³ widoczny
        slider.fillRect.GetComponent<Image>().color = color;
        slider.fillRect.GetComponent<Image>().sprite = null; // Usuwamy sprite, aby w pe³ni wyœwietliæ gradient fill
        slider.fillRect.GetComponent<Image>().type = Image.Type.Filled; // Ustawiamy typ fill'u na 'Filled'
        slider.fillRect.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal; // Ustawiamy metodê fill'u na 'Horizontal'
        slider.fillRect.GetComponent<Image>().fillOrigin = (int)Slider.Direction.LeftToRight; // Ustawiamy kierunek fill'u na lewo->prawo
    }

    private void UpdateHealthUI()
    {
        healthBar.SetSlider(maxHealth - currentHealth);
    }
}
