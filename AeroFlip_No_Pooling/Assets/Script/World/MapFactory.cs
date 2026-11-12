using UnityEngine;

// Baseline: maps have no reserve, reuse, or object pool.
public class MapFactory : MonoBehaviour
{
    [SerializeField] private GameObject safeMapPrefab;
    [SerializeField] private GameObject[] mapPrefabs;

    public int MapPrefabsCount => mapPrefabs == null ? 0 : mapPrefabs.Length;

    public GameObject CreateSafeMap()
    {
        return safeMapPrefab != null ? Instantiate(safeMapPrefab, transform) : null;
    }

    public GameObject CreateMap(int mapIndex)
    {
        if (mapPrefabs == null || mapIndex < 0 || mapIndex >= mapPrefabs.Length)
            return null;

        GameObject prefab = mapPrefabs[mapIndex];
        return prefab != null ? Instantiate(prefab, transform) : null;
    }

    public void DestroyMap(GameObject map)
    {
        if (map != null)
            Destroy(map);
    }
}
