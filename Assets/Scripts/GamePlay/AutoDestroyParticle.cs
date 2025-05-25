using UnityEngine;

/// <summary>
/// Plays the main ParticleSystem on start and destroys the GameObject once all particles (including children) are done.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticle : MonoBehaviour {
    private ParticleSystem mainParticleSystem;

    private void Awake() {
        mainParticleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable() {
        mainParticleSystem.Play();
    }

    private void Update() {
        // Check if all particles (including children) are done
        if (!mainParticleSystem.IsAlive(true)) {
            Destroy(gameObject);
        }
    }
}