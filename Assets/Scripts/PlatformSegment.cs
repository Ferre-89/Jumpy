using UnityEngine;

public class PlatformSegment : MonoBehaviour
{
    public bool isDeathPlatform = false;
    public Material normalMaterial;
    public Material deathMaterial;

    private Renderer platformRenderer;
    private Collider platformCollider;
    private bool hasBeenHit = false;

    void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();

        // Asignar material seg�n tipo
        if (isDeathPlatform)
        {
            platformRenderer.material = deathMaterial;
        }
        else if (normalMaterial != null)
        {
            platformRenderer.material = normalMaterial;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Solo reaccionar si colisiona con la bola
        if (collision.gameObject.name == "PlayerBall" && !hasBeenHit)
        {
            hasBeenHit = true;

            if (isDeathPlatform)
            {
                Debug.Log("�PLATAFORMA MORTAL! Game Over");
                // Aqu� luego llamaremos al Game Over
            }
            else
            {
                Debug.Log("Plataforma normal - Rebote");

                // Aplicar fuerza de rebote hacia arriba
                Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
                if (ballRb != null)
                {
                    // Fuerza de rebote
                    ballRb.linearVelocity = new Vector3(ballRb.linearVelocity.x, 12f, 0);

                    // Efecto visual: cambiar color
                    platformRenderer.material.color = Color.gray;

                    // Opcional: desactivar collider despu�s del primer toque
                    // platformCollider.enabled = false;
                }
            }
        }
    }
}