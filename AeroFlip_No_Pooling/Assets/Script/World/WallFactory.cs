using UnityEngine;

// Baseline: a new transition wall is instantiated each time, then destroyed.
public class WallFactory : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    private GameObject currentWall;

    public GameObject CreateWall()
    {
        DestroyWall();
        if (wallPrefab == null)
            return null;

        currentWall = Instantiate(wallPrefab, transform);
        return currentWall;
    }

    public void DestroyWall()
    {
        if (currentWall == null)
            return;

        Destroy(currentWall);
        currentWall = null;
    }
}
