using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float gravityScale = 0.3f;
    [SerializeField] private float bounceForce = 6f;
    [SerializeField] private float maxFallSpeed = 4f;

    [Header("Position")]
    [SerializeField] private float ringRadius = 1.25f; // Positive Z side

    private Rigidbody rb;
    private bool isActive = false;
    private Vector3 startPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPosition = transform.position;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }

        SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void FixedUpdate()
    {
        if (!isActive || rb == null) return;

        // Apply extra gravity
        rb.AddForce(Vector3.down * gravityScale * Physics.gravity.magnitude, ForceMode.Acceleration);

        // Clamp fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
        }

        // Keep ball on the ring path (at ringRadius distance from center)
        // The ball stays at a fixed position on the ring while rings rotate around it
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, 0f, Time.fixedDeltaTime * 5f);
        pos.z = Mathf.Lerp(pos.z, ringRadius, Time.fixedDeltaTime * 5f); // Stay at ring radius on Z axis
        transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        if (collision.gameObject.CompareTag("SafeZone"))
        {
            Bounce();
        }
        else if (collision.gameObject.CompareTag("DangerZone"))
        {
            GameManager.Instance?.GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        // Passing through a gap scores a point
        if (other.CompareTag("GapTrigger"))
        {
            GameManager.Instance?.AddScore(1);
        }
    }

    public void Bounce()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0f, bounceForce, 0f);
        }
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Menu:
                SetActive(false);
                ResetPlayer();
                break;

            case GameManager.GameState.Playing:
                ResetPlayer();
                SetActive(true);
                break;

            case GameManager.GameState.GameOver:
                SetActive(false);
                break;
        }
    }

    private void SetActive(bool active)
    {
        isActive = active;

        if (rb != null)
        {
            if (!active)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = !active;

            // Use continuous collision detection to prevent tunneling
            rb.collisionDetectionMode = active ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
        }

        gameObject.SetActive(true);
    }

    private void ResetPlayer()
    {
        // Reset to starting height but at ring radius position
        transform.position = new Vector3(0f, startPosition.y, ringRadius);
        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            // Only set velocity if not kinematic
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
