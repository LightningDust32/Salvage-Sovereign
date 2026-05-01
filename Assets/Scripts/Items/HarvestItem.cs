using UnityEngine;

[CreateAssetMenu(fileName = "HarvestItem", menuName = "Combat/HarvestItem")]
public class HarvestItem : ScriptableObject
{
    public string itemName;
    public int sellValue;
    public float damageChange;
    public float speedChange;
    public DamageType damageTypeChange;
    public bool changeDamageType;
}
