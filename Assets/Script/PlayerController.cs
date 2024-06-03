using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 10f;  // Maksymalna prędkość poruszania się
    public float rotationSpeed = 100f;  // Prędkość obrotu kamery
    public float accelerationRate = 2f;  // Współczynnik przyspieszania
    public float decelerationRate = 1f;  // Współczynnik hamowania

    private float currentSpeed = 0f;
    private float rotationX = 0f;

    void Update()
    {
        // Obrót kamery za pomocą myszki
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Dodajemy ruch w górę i w dół
        rotationX -= mouseY * rotationSpeed * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Obrót postaci za pomocą myszki w poziomie
        transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime, Space.World);

        // Poruszanie się do przodu na podstawie wciskania klawisza "W"
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, accelerationRate * Time.deltaTime);

            // Poruszanie się w lokalnym kierunku osi Z kamery
            transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // Hamowanie na podstawie wciskania klawisza "S"
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, decelerationRate * Time.deltaTime);

            // Jeśli klawisz "S" nie jest wciśnięty, zatrzymaj się
            if (currentSpeed > 0)
                transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        }
    }
}
