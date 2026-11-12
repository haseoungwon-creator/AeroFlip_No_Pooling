using System.Collections.Generic;
using UnityEngine;

// Baseline: UI warnings are created and destroyed, never reused.
public class MissileWarningFactory : MonoBehaviour
{
    [SerializeField] private GameObject warningPrefab;
    private readonly List<GameObject> activeWarnings = new List<GameObject>();

    public GameObject CreateWarning()
    {
        if (warningPrefab == null)
            return null;

        GameObject warning = Instantiate(warningPrefab, transform);
        activeWarnings.Add(warning);
        return warning;
    }

    public void DestroyWarning(GameObject warning)
    {
        if (warning == null)
            return;

        if (activeWarnings.Remove(warning))
            Destroy(warning);
    }

    public void DestroyAllWarnings()
    {
        foreach (GameObject warning in activeWarnings)
        {
            if (warning != null)
                Destroy(warning);
        }

        activeWarnings.Clear();
    }
}
