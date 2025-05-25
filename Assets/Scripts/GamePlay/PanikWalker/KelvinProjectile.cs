using UnityEngine;

public class KelvinProjectile : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float hitThreshold = 0.5f;
    [SerializeField] private float lifeTime = 10f;

    private Transform targetTransform;
    private EnemyHealth targetHealth;
    private PanicWalkerTargetMarker markerRef;

    public void Init(EnemyHealth target, Transform marker) {
        targetHealth = target;
        targetTransform = marker;
        markerRef = marker.GetComponent<PanicWalkerTargetMarker>();
        Destroy(gameObject, lifeTime); 
    }

    private void Update() {
        if (targetTransform == null) {
            transform.position += transform.forward * speed * Time.deltaTime;
            return;
        }

        Vector3 dir = (targetTransform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, targetTransform.position);
        if (distance <= hitThreshold) {
            TryApplyDamageAndDestroy();
        }
    }

    private void TryApplyDamageAndDestroy() {
        if (targetHealth != null) {
            targetHealth.TakeDamage(1);
        }

        if (markerRef != null) {
            markerRef.UnlinkAndDestroy();
        }

        Destroy(gameObject);
    }
}