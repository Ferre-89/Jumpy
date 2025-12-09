using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // La bola
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 3, -10);

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No hay target asignado");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Seguir solo en Y (vertical)
            float targetY = Mathf.Max(target.position.y, 0);
            Vector3 desiredPosition = new Vector3(0, targetY, 0) + offset;

            // Movimiento suave
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

            transform.position = smoothedPosition;
        }
    }
}