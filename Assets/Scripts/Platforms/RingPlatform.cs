using UnityEngine;

public class RingPlatform : MonoBehaviour
{
    [Header("Ring Settings")]
    [SerializeField] private float innerRadius = 0.5f;
    [SerializeField] private float outerRadius = 2f;
    [SerializeField] private float height = 0.3f;

    [Header("Materials")]
    private Material safeMaterial;
    private Material dangerMaterial;

    [Header("Settings")]
    [SerializeField] private float despawnDistance = 20f;

    private Transform mainCamera;

    public void Initialize(int[] pattern, bool enableDanger, Material safeMat, Material dangerMat)
    {
        safeMaterial = safeMat;
        dangerMaterial = dangerMat;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        CreateSegments(pattern, enableDanger);
    }

    private void CreateSegments(int[] pattern, bool enableDanger)
    {
        int numSegments = pattern.Length;
        float anglePerSegment = 360f / numSegments;

        for (int i = 0; i < numSegments; i++)
        {
            int type = pattern[i];

            if (type == 0)
            {
                CreateGapTrigger(i, anglePerSegment);
                continue;
            }

            if (type == 2 && !enableDanger)
            {
                CreateGapTrigger(i, anglePerSegment);
                continue;
            }

            CreateArcSegment(i, anglePerSegment, type);
        }
    }

    private void CreateArcSegment(int index, float anglePerSegment, int type)
    {
        GameObject segment = new GameObject($"Segment_{index}");
        segment.transform.SetParent(transform);
        segment.transform.localPosition = Vector3.zero;
        segment.transform.localRotation = Quaternion.identity;

        MeshFilter meshFilter = segment.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = segment.AddComponent<MeshRenderer>();

        Mesh mesh = CreateArcMesh(index * anglePerSegment, anglePerSegment);
        meshFilter.mesh = mesh;

        // NO MeshCollider - they don't work with procedural meshes
        // Instead, create invisible BoxCollider children for collision

        string tag = type == 1 ? "SafeZone" : "DangerZone";

        // Create BoxColliders along the arc (invisible, only for collision)
        float startAngle = index * anglePerSegment;
        float midRadius = (innerRadius + outerRadius) / 2f;
        float colliderDepth = outerRadius - innerRadius;

        int numColliders = 2; // 2 box colliders per segment
        float subAngle = anglePerSegment / numColliders;

        for (int i = 0; i < numColliders; i++)
        {
            float angle = (startAngle + subAngle * (i + 0.5f)) * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * midRadius;
            float z = Mathf.Cos(angle) * midRadius;
            float colliderWidth = midRadius * subAngle * Mathf.Deg2Rad;

            GameObject colChild = new GameObject($"Collider_{i}");
            colChild.transform.SetParent(segment.transform);
            colChild.transform.localPosition = new Vector3(x, 0f, z);
            colChild.transform.localRotation = Quaternion.Euler(0f, startAngle + subAngle * (i + 0.5f), 0f);
            colChild.tag = tag;

            BoxCollider box = colChild.AddComponent<BoxCollider>();
            box.size = new Vector3(colliderWidth * 0.95f, height + 0.1f, colliderDepth);

        }

        // Set material (visual only)
        if (type == 1)
        {
            if (safeMaterial != null) meshRenderer.material = safeMaterial;
            segment.tag = "SafeZone";
        }
        else if (type == 2)
        {
            if (dangerMaterial != null) meshRenderer.material = dangerMaterial;
            segment.tag = "DangerZone";
        }
    }

    private void CreateGapTrigger(int index, float anglePerSegment)
    {
        GameObject trigger = new GameObject($"GapTrigger_{index}");
        trigger.transform.SetParent(transform);

        float midAngle = (index * anglePerSegment + anglePerSegment / 2f) * Mathf.Deg2Rad;
        float midRadius = (innerRadius + outerRadius) / 2f;

        float x = Mathf.Sin(midAngle) * midRadius;
        float z = Mathf.Cos(midAngle) * midRadius;

        trigger.transform.localPosition = new Vector3(x, -0.5f, z);

        BoxCollider col = trigger.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.5f, 1f, 1.5f);

        trigger.tag = "GapTrigger";
    }

    private Mesh CreateArcMesh(float startAngle, float arcAngle)
    {
        Mesh mesh = new Mesh();

        int arcSegments = 10;
        int vertCount = (arcSegments + 1) * 4;

        Vector3[] vertices = new Vector3[vertCount];
        float angleStep = arcAngle / arcSegments;

        for (int i = 0; i <= arcSegments; i++)
        {
            float angle = (startAngle + i * angleStep) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            int baseIdx = i * 4;

            vertices[baseIdx] = new Vector3(sin * innerRadius, -height / 2, cos * innerRadius);
            vertices[baseIdx + 1] = new Vector3(sin * outerRadius, -height / 2, cos * outerRadius);
            vertices[baseIdx + 2] = new Vector3(sin * innerRadius, height / 2, cos * innerRadius);
            vertices[baseIdx + 3] = new Vector3(sin * outerRadius, height / 2, cos * outerRadius);
        }

        int[] triangles = new int[arcSegments * 24];
        int triIdx = 0;

        for (int i = 0; i < arcSegments; i++)
        {
            int baseIdx = i * 4;
            int nextBaseIdx = (i + 1) * 4;

            triangles[triIdx++] = baseIdx + 2;
            triangles[triIdx++] = nextBaseIdx + 2;
            triangles[triIdx++] = baseIdx + 3;
            triangles[triIdx++] = baseIdx + 3;
            triangles[triIdx++] = nextBaseIdx + 2;
            triangles[triIdx++] = nextBaseIdx + 3;

            triangles[triIdx++] = baseIdx;
            triangles[triIdx++] = baseIdx + 1;
            triangles[triIdx++] = nextBaseIdx;
            triangles[triIdx++] = nextBaseIdx;
            triangles[triIdx++] = baseIdx + 1;
            triangles[triIdx++] = nextBaseIdx + 1;

            triangles[triIdx++] = baseIdx + 1;
            triangles[triIdx++] = baseIdx + 3;
            triangles[triIdx++] = nextBaseIdx + 1;
            triangles[triIdx++] = nextBaseIdx + 1;
            triangles[triIdx++] = baseIdx + 3;
            triangles[triIdx++] = nextBaseIdx + 3;

            triangles[triIdx++] = baseIdx;
            triangles[triIdx++] = nextBaseIdx;
            triangles[triIdx++] = baseIdx + 2;
            triangles[triIdx++] = baseIdx + 2;
            triangles[triIdx++] = nextBaseIdx;
            triangles[triIdx++] = nextBaseIdx + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main?.transform;
        }

        if (mainCamera != null && transform.position.y > mainCamera.position.y + despawnDistance)
        {
            gameObject.SetActive(false);
        }
    }

}
