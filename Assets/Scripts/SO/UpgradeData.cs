using UnityEngine;
using RetroHunter2;

[CreateAssetMenu(menuName = "RetroHunter2/Upgrade")]
public class UpgradeData : ScriptableObject {
    public UpgradeType upgradeType;
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
}