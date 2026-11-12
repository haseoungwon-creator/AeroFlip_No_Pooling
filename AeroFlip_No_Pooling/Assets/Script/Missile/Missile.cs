using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] float speed = 100f;

    private MissileFactory missileFactory;
    private Vector3 direction;
    private float travelDistance;
    private float movedDistance;

    public void Initialize(MissileFactory factory,Vector3 direction,float travelDistance)
    {
        missileFactory = factory;
        this.direction = direction.normalized;
        this.travelDistance = travelDistance;
        movedDistance = 0f;
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        transform.position += direction * moveDistance;
        movedDistance += moveDistance;

        if(movedDistance >= travelDistance)
            Despawn();
    }

    public void Despawn()
    {
        if (missileFactory != null)
            missileFactory.DestroyMissile(gameObject);
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.GameOver();
        Despawn();
    }
}
