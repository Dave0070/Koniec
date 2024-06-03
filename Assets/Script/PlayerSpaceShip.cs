using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpaceShip : MonoBehaviour
{
    Rigidbody spaceshipRB;
    Camera playerCamera;
    bool isStrafing; // Nowa zmienna dla sprawdzenia, czy statek porusza siê na boki

    //INP
    float verticalMove;
    float horizontalMove;
    float mouseInputX;
    float mouseInputY;
    float rollInput;
    float upDownInput;

    //SM
    [SerializeField]
    float speedMulti = 1;
    [SerializeField]
    float speedMultAngle = 0.5f;
    [SerializeField]
    float speedRollMultAngel = 0.05f;
    [SerializeField]
    float upDownSpeed = 1;
    [SerializeField]
    float strafeSpeed = 1; // Prêdkoœæ przesuwania siê na boki
    [SerializeField]
    float dashDistance = 10f; // Odleg³oœæ przemieszczenia siê w czasie dasha
    [SerializeField]
    int maxDashCount = 3; // Maksymalna liczba dostêpnych dashy
    [SerializeField]
    float dashRegenCooldown = 10f; // Czas regeneracji dashy w sekundach
    [SerializeField]
    float dashFOVChange = 20f; // Zmiana pola widzenia kamery podczas dasha
    [SerializeField]
    float dashFOVTime = 0.1f; // Czas trwania zmiany pola widzenia kamery podczas dasha
    [SerializeField]
    float returnFOVTime = 0.1f; // Czas trwania powrotu pola widzenia kamery do wartoœci pocz¹tkowej

    [SerializeField]
    KeyCode unlockKey = KeyCode.Space; // Klawisz do przytrzymania
    [SerializeField]
    float unlockDuration = 3f; // Czas przytrzymania klawisza
    [SerializeField]
    float rotationSensitivity = 1f; // Czu³oœæ rotacji w osi y
    [SerializeField]
    float rotationSmoothness = 0.1f; // P³ynnoœæ rotacji

    int currentDashIndex = 0; // Indeks aktualnie u¿ywanego dasha
    int availableDashCount = 0; // Liczba dostêpnych dashy
    float lastDashTime; // Czas ostatniego u¿ycia dasha
    float originalFOV; // Poprzednia wartoœæ pola widzenia kamery

    bool isUnlocked = false; // Flaga wskazuj¹ca, czy pe³ne sterowanie jest dostêpne
    float unlockStartTime; // Czas rozpoczêcia przytrzymania klawisza

    Coroutine fovCoroutine; // Referencja do aktualnej korutyny zmiany FOV

    // UI
    public Text dashCooldownText;
    public Slider dashCooldownSlider;
    public Image[] dashImages;

    float currentYRotation; // Aktualna rotacja y
    float targetYRotation; // Docelowa rotacja y

    // Start is called before the first frame update
    void Start()
    {
        spaceshipRB = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
        lastDashTime = Time.time - dashRegenCooldown; // Ustawienie czasu tak, aby od razu rozpocz¹æ regeneracjê
        availableDashCount = maxDashCount; // Gracz zaczyna z maksymaln¹ iloœci¹ dashy
        originalFOV = playerCamera.fieldOfView; // Zapisanie pocz¹tkowej wartoœci pola widzenia kamery
        UpdateDashUI();

        currentYRotation = transform.eulerAngles.y;
        targetYRotation = currentYRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Sprawdzenie, czy klawisz jest przytrzymany przez odpowiedni czas
        if (Input.GetKey(unlockKey))
        {
            if (!isUnlocked)
            {
                if (unlockStartTime == 0f)
                {
                    unlockStartTime = Time.time;
                }
                else if (Time.time - unlockStartTime >= unlockDuration)
                {
                    isUnlocked = true;
                }
            }
        }
        else
        {
            unlockStartTime = 0f;
        }

        // Jeœli sterowanie nie jest odblokowane, ogranicz do rotacji w osi y
        if (!isUnlocked)
        {
            mouseInputX = Input.GetAxis("Mouse X") * rotationSensitivity;
            targetYRotation += mouseInputX;

            // Interpolacja do nowej wartoœci rotacji y
            currentYRotation = Mathf.Lerp(currentYRotation, targetYRotation, rotationSmoothness);
            Quaternion rotation = Quaternion.Euler(0, currentYRotation, 0);
            spaceshipRB.MoveRotation(rotation);
            return; // Zakoñcz Update() jeœli sterowanie nie jest odblokowane
        }

        verticalMove = Input.GetAxis("Vertical");
        horizontalMove = Input.GetAxis("Horizontal");
        rollInput = Input.GetAxis("Roll");

        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");

        upDownInput = 0;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            upDownInput = 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            upDownInput = -1;
        }

        // Sprawdzenie, czy statek porusza siê na boki
        isStrafing = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        // Sprawdzenie, czy mo¿na u¿yæ dasha (prawy przycisk myszy)
        if (Input.GetMouseButtonDown(1) && availableDashCount > 0)
        {
            // Przesuniêcie statku o okreœlon¹ odleg³oœæ w przestrzeni w kierunku, w którym patrzy
            spaceshipRB.AddForce(transform.forward * dashDistance, ForceMode.Impulse);
            availableDashCount--; // Zmniejszenie liczby dostêpnych dashy
            lastDashTime = Time.time; // Zapisanie czasu ostatniego u¿ycia dasha

            // Przerwanie poprzedniej korutyny zmiany FOV, jeœli istnieje
            if (fovCoroutine != null)
                StopCoroutine(fovCoroutine);

            // Rozpoczêcie korutyny zmiany pola widzenia kamery podczas dasha
            fovCoroutine = StartCoroutine(ChangeFOV(dashFOVChange, dashFOVTime));

            // Wy³¹czenie obrazu reprezentuj¹cego u¿ywany dash
            dashImages[availableDashCount].enabled = false;
        }

        // Sprawdzenie, czy czas od ostatniego u¿ycia dasha przekroczy³ czas regeneracji
        if (Time.time - lastDashTime > dashRegenCooldown && availableDashCount < maxDashCount)
        {
            availableDashCount++; // Zwiêkszenie liczby dostêpnych dashy
            lastDashTime = Time.time; // Zapisanie czasu rozpoczêcia regeneracji

            // W³¹czenie obrazu reprezentuj¹cego nowy dostêpny dash
            dashImages[availableDashCount - 1].enabled = true;
        }

        UpdateDashUI();
    }

    IEnumerator ChangeFOV(float targetFOV, float duration)
    {
        // Zapisanie poprzedniej wartoœci pola widzenia kamery
        float startFOV = playerCamera.fieldOfView;

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Interpolacja liniowa dla p³ynnej zmiany FOV
            float newFOV = Mathf.Lerp(startFOV, targetFOV, timeElapsed / duration);
            playerCamera.fieldOfView = newFOV;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ustawienie pola widzenia na docelowe FOV po zakoñczeniu interpolacji
        playerCamera.fieldOfView = targetFOV;

        // Resetowanie referencji do korutyny
        fovCoroutine = null;

        // Czekanie 0.1 sekundy
        yield return new WaitForSeconds(returnFOVTime);

        // Rozpoczêcie korutyny zmiany pola widzenia kamery podczas powrotu FOV do wartoœci pocz¹tkowej
        fovCoroutine = StartCoroutine(ReturnFOV(startFOV, returnFOVTime));
    }

    IEnumerator ReturnFOV(float targetFOV, float duration)
    {
        // Zapisanie aktualnej wartoœci pola widzenia kamery
        float currentFOV = playerCamera.fieldOfView;

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Interpolacja liniowa dla p³ynnego powrotu FOV do wartoœci pocz¹tkowej
            float newFOV = Mathf.Lerp(currentFOV, targetFOV, timeElapsed / duration);
            playerCamera.fieldOfView = newFOV;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ustawienie pola widzenia na pierwotne FOV po zakoñczeniu interpolacji
        playerCamera.fieldOfView = targetFOV;

        // Resetowanie referencji do korutyny
        fovCoroutine = null;
    }

    void FixedUpdate()
    {
        // Jeœli sterowanie nie jest odblokowane, ogranicz do rotacji w osi y
        if (!isUnlocked)
        {
            return; // Zakoñcz FixedUpdate() jeœli sterowanie nie jest odblokowane
        }

        // Jeœli statek porusza siê na boki, zablokuj mo¿liwoœæ cofania siê
        if (isStrafing)
        {
            verticalMove = Mathf.Max(0, verticalMove); // Ustaw wartoœæ pionowego ruchu na 0 lub wiêksz¹, aby uniemo¿liwiæ cofanie siê
        }

        spaceshipRB.AddForce(spaceshipRB.transform.TransformDirection(Vector3.forward) * verticalMove * speedMulti, ForceMode.VelocityChange);
        spaceshipRB.AddForce(spaceshipRB.transform.TransformDirection(Vector3.right) * horizontalMove * speedMulti, ForceMode.VelocityChange);

        spaceshipRB.AddTorque(spaceshipRB.transform.right * speedMultAngle * mouseInputY * -1, ForceMode.VelocityChange);
        spaceshipRB.AddTorque(spaceshipRB.transform.up * speedMultAngle * mouseInputX, ForceMode.VelocityChange);

        spaceshipRB.AddTorque(spaceshipRB.transform.forward * speedRollMultAngel * rollInput, ForceMode.VelocityChange);

        spaceshipRB.AddForce(spaceshipRB.transform.up * upDownInput * upDownSpeed, ForceMode.VelocityChange);
    }

    void UpdateDashUI()
    {
        if (dashCooldownText != null)
        {
            float timeSinceLastDash = Time.time - lastDashTime;
            float timeLeft = dashRegenCooldown - timeSinceLastDash;
            if (timeLeft < 0)
                timeLeft = 0;
            dashCooldownText.text = "Next Dash: " + timeLeft.ToString("0.0");
        }

        if (dashCooldownSlider != null)
        {
            float timeSinceLastDash = Time.time - lastDashTime;
            float timeLeft = dashRegenCooldown - timeSinceLastDash;
            float cooldownProgress = 1f - (timeLeft / dashRegenCooldown);
            dashCooldownSlider.value = cooldownProgress;
        }
    }
}
