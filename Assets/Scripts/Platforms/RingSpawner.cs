using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    [Header("Ring Prefab")]
    [SerializeField] private GameObject ringPrefab;

    [Header("Spawning")]
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float ringSpacing = 3f;
    [SerializeField] private float firstRingY = -3f;
    [SerializeField] private float spawnAheadDistance = 30f;

    [Header("Ring Configuration")]
    [SerializeField] private int segmentsPerRing = 8;
    [SerializeField] private int minGaps = 2;
    [SerializeField] private int maxGaps = 3;
    [SerializeField] private int minDangerZones = 1;
    [SerializeField] private int maxDangerZones = 3;

    [Header("Difficulty")]
    [SerializeField] private int safePlatformsCount = 5;

    [Header("Materials")]
    [SerializeField] private Material safeMaterial;
    [SerializeField] private Material dangerMaterial;

    [Header("Rotation Control")]
    [SerializeField] private float rotationSpeed = 280f;

    private Queue<GameObject> ringPool;
    private List<GameObject> activeRings;
    private float lastSpawnY;
    private bool isSpawning = false;
    private int ringCount = 0;

    // Rotation
    private Transform ringsParent;
    private float targetRotation = 0f;
    private float currentRotation = 0f;

    private void Start()
    {
        // Create a parent object for all rings (to rotate them together)
        GameObject parent = new GameObject("RingsParent");
        ringsParent = parent.transform;
        ringsParent.position = Vector3.zero;

        InitializePool();

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

    private void InitializePool()
    {
        ringPool = new Queue<GameObject>();
        activeRings = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject ring = Instantiate(ringPrefab, ringsParent);
            ring.SetActive(false);
            ringPool.Enqueue(ring);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        HandleInput();
        UpdateRotation();

        if (isSpawning)
        {
            RecycleRings();
            SpawnRingsAhead();
        }
    }

    private void HandleInput()
    {
        float rotateDirection = 0f;

        // Touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float screenMiddle = Screen.width / 2f;

            if (touch.position.x < screenMiddle)
            {
                rotateDirection = 1f; // Rotate left
            }
            else
            {
                rotateDirection = -1f; // Rotate right
            }
        }
        // Mouse input for testing
        else if (Input.GetMouseButton(0))
        {
            float screenMiddle = Screen.width / 2f;

            if (Input.mousePosition.x < screenMiddle)
            {
                rotateDirection = 1f;
            }
            else
            {
                rotateDirection = -1f;
            }
        }
        // Keyboard input for testing
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rotateDirection = 1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rotateDirection = -1f;
        }

        targetRotation += rotateDirection * rotationSpeed * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * 10f);
        ringsParent.rotation = Quaternion.Euler(0f, currentRotation, 0f);
    }

    private void RecycleRings()
    {
        for (int i = activeRings.Count - 1; i >= 0; i--)
        {
            if (!activeRings[i].activeInHierarchy)
            {
                ringPool.Enqueue(activeRings[i]);
                activeRings.RemoveAt(i);
            }
        }
    }

    private void SpawnRingsAhead()
    {
        float targetY = Camera.main != null
            ? Camera.main.transform.position.y - spawnAheadDistance
            : lastSpawnY - spawnAheadDistance;

        while (lastSpawnY > targetY && ringPool.Count > 0)
        {
            SpawnRing(lastSpawnY);
            lastSpawnY -= ringSpacing;
        }
    }

    private void SpawnRing(float yPosition)
    {
        if (ringPool.Count == 0) return;

        ringCount++;

        GameObject ring = ringPool.Dequeue();
        ring.transform.localPosition = new Vector3(0f, yPosition, 0f);
        ring.transform.localRotation = Quaternion.identity;
        ring.SetActive(true);
        activeRings.Add(ring);

        bool enableDanger = ringCount > safePlatformsCount;
        bool isFirstRing = ringCount == 1;
        int[] pattern = GeneratePattern(enableDanger, isFirstRing);

        RingPlatform ringPlatform = ring.GetComponent<RingPlatform>();
        if (ringPlatform != null)
        {
            ringPlatform.Initialize(pattern, enableDanger, safeMaterial, dangerMaterial);
        }
    }

    private int[] GeneratePattern(bool enableDanger, bool isFirstRing = false)
    {
        int[] pattern = new int[segmentsPerRing];

        // Start with all safe zones
        for (int i = 0; i < segmentsPerRing; i++)
        {
            pattern[i] = 1; // Safe
        }

        if (isFirstRing)
        {
            // First ring: gap at player's starting position (angle 0°, segments 0 and 7)
            pattern[0] = 0;
            pattern[segmentsPerRing - 1] = 0;
        }
        else
        {
            // Other rings: create a paired gap (2 adjacent segments) at random position
            int gapStart = Random.Range(0, segmentsPerRing);
            pattern[gapStart] = 0;
            pattern[(gapStart + 1) % segmentsPerRing] = 0; // Adjacent segment (wraps around)
        }

        // Add danger zones if enabled (after safe platforms count)
        if (enableDanger)
        {
            // Find segments that are still safe (value 1)
            List<int> safeIndices = new List<int>();
            for (int i = 0; i < segmentsPerRing; i++)
            {
                if (pattern[i] == 1) safeIndices.Add(i);
            }

            int numDanger = Random.Range(minDangerZones, maxDangerZones + 1);
            for (int i = 0; i < numDanger && safeIndices.Count > 0; i++)
            {
                int idx = Random.Range(0, safeIndices.Count);
                pattern[safeIndices[idx]] = 2; // Danger
                safeIndices.RemoveAt(idx);
            }
        }

        return pattern;
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Menu:
                StopSpawning();
                ClearAllRings();
                ResetRotation();
                break;

            case GameManager.GameState.Playing:
                StartSpawning();
                break;

            case GameManager.GameState.GameOver:
                StopSpawning();
                break;
        }
    }

    private void StartSpawning()
    {
        ClearAllRings();
        ringCount = 0;
        isSpawning = true;
        ResetRotation();

        lastSpawnY = firstRingY;

        // Spawn initial rings
        for (int i = 0; i < 12; i++)
        {
            SpawnRing(lastSpawnY);
            lastSpawnY -= ringSpacing;
        }
    }

    private void StopSpawning()
    {
        isSpawning = false;
    }

    private void ClearAllRings()
    {
        foreach (var ring in activeRings)
        {
            ring.SetActive(false);
            ringPool.Enqueue(ring);
        }
        activeRings.Clear();
    }

    private void ResetRotation()
    {
        targetRotation = 0f;
        currentRotation = 0f;
        if (ringsParent != null)
        {
            ringsParent.rotation = Quaternion.identity;
        }
    }
}
