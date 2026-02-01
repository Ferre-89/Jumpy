using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private float heightAbovePlayer = 8f;
    [SerializeField] private float smoothSpeed = 5f;

    private bool isFollowing = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        // Save EXACT position and rotation from editor - this is the menu view
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void LateUpdate()
    {
        // Only follow Y when playing, keep X/Z and rotation FIXED
        if (!isFollowing || player == null) return;

        float targetY = player.position.y + heightAbovePlayer;
        float newY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);

        // Only change Y position, keep X and Z from editor
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

        // Keep rotation EXACTLY as set in editor - never change it
        transform.rotation = initialRotation;
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Menu:
                isFollowing = false;
                // Reset to exact initial position and rotation
                transform.position = initialPosition;
                transform.rotation = initialRotation;
                break;

            case GameManager.GameState.Playing:
                isFollowing = true;
                break;

            case GameManager.GameState.GameOver:
                isFollowing = false;
                break;
        }
    }
}
