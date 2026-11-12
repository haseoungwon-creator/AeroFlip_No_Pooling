using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRewind : MonoBehaviour
{
    [SerializeField] float rewindDuration = 5f;
    [SerializeField] float recordInterval = 0.02f;
    [SerializeField] float rewindPlayDuration = 2f;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] PlayerController playerController;
    [SerializeField] NearMissDetector nearMissDetector;
    [SerializeField] WorldMovement worldMovement;
    [SerializeField] Collider playerCollider;

    private struct PositionData
    {
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Vector3 worldPosition;

        public PositionData(
            Vector3 playerPosition,
            Quaternion playerRotation,
            Vector3 worldPosition)
        {
            this.playerPosition = playerPosition;
            this.playerRotation = playerRotation;
            this.worldPosition = worldPosition;
        }
    }

    private readonly List<PositionData> positionHistory = new List<PositionData>();
    private int maxRecordedPositions;
    private float recordTimer;

    private bool hasUsed;
    private bool isRewinding;

    public bool IsRewinding => isRewinding;
    public bool CanRewind => !hasUsed && !isRewinding;

    private void Awake()
    {
        maxRecordedPositions = Mathf.Max(1, Mathf.CeilToInt(
            rewindDuration / Mathf.Max(0.001f, recordInterval)));
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState != GameStates.Playing)
            return;

        if (isRewinding)
            return;

        RecordPosition();
    }

    private void RecordPosition()
    {
        recordTimer += Time.deltaTime;

        if (recordTimer < recordInterval)
            return;

        recordTimer -= recordInterval;

        Vector3 worldPosition = Vector3.zero;

        if (worldMovement != null)
            worldPosition = worldMovement.transform.position;

        AddPosition(new PositionData(
            transform.position,
            transform.rotation,
            worldPosition));
    }

    public void Rewind()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState != GameStates.Playing)
            return;

        if (!CanRewind)
            return;

        if (positionHistory.Count == 0)
            return;

        AddPosition(new PositionData(
            transform.position,
            transform.rotation,
            worldMovement != null
                ? worldMovement.transform.position
                : Vector3.zero));

        hasUsed = true;

        StartCoroutine(RewindSequence());
    }

    private IEnumerator RewindSequence()
    {
        isRewinding = true;

        playerController.SetControlEnabled(false);

        if (worldMovement != null)
        {
            worldMovement.SetRewinding(true);
            worldMovement.SetMovement(false);
        }

        if (scoreManager != null)
            scoreManager.SetRewinding(true);

        if (nearMissDetector != null)
            nearMissDetector.SetRewinding(true);

        if (playerCollider != null)
            playerCollider.enabled = false;

        int newestIndex = positionHistory.Count - 1;

        float elapsedTime = 0f;

        while (elapsedTime < rewindPlayDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsedTime / rewindPlayDuration);

            float historyPosition =
                t * (positionHistory.Count - 1);

            int offset =
                Mathf.FloorToInt(historyPosition);

            int currentIndex =
                newestIndex - offset;

            int previousIndex = newestIndex - Mathf.Min(
                offset + 1, positionHistory.Count - 1);

            PositionData current =
                positionHistory[currentIndex];

            PositionData previous =
                positionHistory[previousIndex];

            float interpolation =
                historyPosition - offset;

            transform.SetPositionAndRotation(
                Vector3.Lerp(
                    current.playerPosition,
                    previous.playerPosition,
                    interpolation),
                Quaternion.Slerp(
                    current.playerRotation,
                    previous.playerRotation,
                    interpolation));

            if (worldMovement != null)
            {
                worldMovement.transform.position =
                    Vector3.Lerp(
                        current.worldPosition,
                        previous.worldPosition,
                        interpolation);
            }

            yield return null;
        }

        PositionData oldest = positionHistory[0];

        transform.SetPositionAndRotation(
            oldest.playerPosition,
            oldest.playerRotation);

        if (worldMovement != null)
        {
            worldMovement.transform.position =
                oldest.worldPosition;
        }

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (nearMissDetector != null)
            nearMissDetector.SetRewinding(false);

        if (scoreManager != null)
            scoreManager.SetRewinding(false);

        if (worldMovement != null)
        {
            worldMovement.SetRewinding(false);
            worldMovement.SetMovement(true);
        }

        playerController.ResetMovementTarget();
        playerController.SetControlEnabled(true);

        isRewinding = false;
    }


    private void AddPosition(PositionData position)
    {
        // Baseline behavior: shifts the list every time it exceeds the history limit.
        // In the optimized version, a circular buffer would overwrite a slot instead.
        if (positionHistory.Count >= maxRecordedPositions)
            positionHistory.RemoveAt(0);

        positionHistory.Add(position);
    }

    public void ResetRewind()
    {
        positionHistory.Clear();
        recordTimer = 0f;

        hasUsed = false;
        isRewinding = false;
    }
}