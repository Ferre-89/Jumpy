using UnityEngine;

public class SimpleControls : MonoBehaviour
{
    public float moveForce = 15f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("Controles listos. Usa A/D o Flechas");
    }

    void Update()
    {
        // Verificar input manualmente
        float moveX = 0f;

        // Flechas o WASD
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }

        // Aplicar fuerza si hay input
        if (moveX != 0f)
        {
            Vector3 force = new Vector3(moveX * moveForce, 0, 0);
            rb.AddForce(force, ForceMode.Force);

            // Rotación visual
            transform.Rotate(0, 0, -moveX * 5f);
        }

        // DEBUG: Verificar si cae
        if (Time.frameCount % 60 == 0) // Cada segundo aproximadamente
        {
            Debug.Log("Velocidad Y: " + rb.linearVelocity.y + " | Posición Y: " + transform.position.y);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisión con: " + collision.gameObject.name);
    }
}