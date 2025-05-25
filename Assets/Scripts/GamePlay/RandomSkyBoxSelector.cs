using UnityEngine;
using System.Collections.Generic;

public class RandomSkyboxSelector : MonoBehaviour {
    [Header("Skybox Setup")]
    [Tooltip("List of skybox materials to choose from.")]
    [SerializeField] private List<Material> skyboxes = new();

    [Tooltip("List of matching directional light filter colors.")]
    [SerializeField] private List<Color> directionalLightColors = new();

    [Tooltip("The directional light to apply the color to.")]
    [SerializeField] private Light directionalLight;

    private void Start() {
        if (skyboxes.Count == 0 || directionalLightColors.Count == 0) {
            Debug.LogWarning("Skyboxes or light colors not set in RandomSkyboxSelector.");
            return;
        }

        if (skyboxes.Count != directionalLightColors.Count) {
            Debug.LogWarning("Skybox and color list must have the same length.");
            return;
        }

        int index = Random.Range(0, skyboxes.Count);

        // Set skybox
        RenderSettings.skybox = skyboxes[index];
        DynamicGI.UpdateEnvironment();

        // Set directional light color
        if (directionalLight != null) {
            directionalLight.color = directionalLightColors[index];
        }
    }
}