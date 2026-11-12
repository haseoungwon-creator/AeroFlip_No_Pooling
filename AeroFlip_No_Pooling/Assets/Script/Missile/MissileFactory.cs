using System.Collections.Generic;
using UnityEngine;

// Baseline: track only currently existing missiles for game-state cleanup.
// Every launch calls Instantiate, and every finish calls Destroy.
public class MissileFactory : MonoBehaviour
{
    [SerializeField] private GameObject missilePrefab;
    private readonly List<GameObject> activeMissiles = new List<GameObject>();

    public GameObject CreateMissile()
    {
        if (missilePrefab == null)
            return null;

        GameObject missile = Instantiate(missilePrefab, transform);
        activeMissiles.Add(missile);
        return missile;
    }

    public void DestroyMissile(GameObject missile)
    {
        if (missile == null)
            return;

        if (activeMissiles.Remove(missile))
            Destroy(missile);
    }

    public void DestroyAllMissiles()
    {
        foreach (GameObject missile in activeMissiles)
        {
            if (missile != null)
                Destroy(missile);
        }

        activeMissiles.Clear();
    }

    public bool HasActiveMissiles()
    {
        return activeMissiles.Count > 0;
    }
}
