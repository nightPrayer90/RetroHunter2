using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Uses NavMesh to wander randomly on a walkable surface.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PanicWalkerNavMesh : MonoBehaviour {
    [SerializeField] private float wanderRadius = 3f;

    private NavMeshAgent agent;

    private float destinationCooldown = 0.5f;
    private float cooldownTimer = 0f;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        PickNewDestination();
        agent.avoidancePriority = Random.Range(30, 100);
    }

    private void Update() {
        cooldownTimer -= Time.deltaTime;

        if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled) {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && cooldownTimer <= 0f) {
                PickNewDestination();
                cooldownTimer = destinationCooldown;
            }
        }
    }

    private void PickNewDestination() {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + transform.position;
        randomDirection.y = transform.position.y;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas)) {
          
            if (Vector3.Distance(agent.transform.position, hit.position) > 1f) {
                agent.SetDestination(hit.position);
            }
        }
    }

    public void PauseAndRestartMovement(float delay = 1f) {
        agent.Warp(transform.position);
        StartCoroutine(PauseAgentRoutine(delay));
    }

    private IEnumerator PauseAgentRoutine(float delay) {
        if (agent == null) yield break;

        agent.Warp(transform.position);
        agent.isStopped = true;

        yield return new WaitForSeconds(delay);

        agent.isStopped = false;

        PickNewDestination();
        cooldownTimer = destinationCooldown;
    }
}