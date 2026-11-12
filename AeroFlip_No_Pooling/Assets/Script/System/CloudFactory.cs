using UnityEngine;

// Baseline: every cloud is constructed and destroyed. There is no reuse or preallocation.
public class CloudFactory : MonoBehaviour
{
    [SerializeField] private GameObject[] cloudPrefabs;

    public GameObject CreateCloud()
    {
        if (cloudPrefabs == null || cloudPrefabs.Length == 0)
            return null;

        GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        return prefab != null ? Instantiate(prefab, transform) : null;
    }

    public void DestroyCloud(GameObject cloud)
    {
        if (cloud != null)
            Destroy(cloud);
    }
}
