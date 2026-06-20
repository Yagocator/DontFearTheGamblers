using UnityEngine;
[CreateAssetMenu(fileName = "EnemySO", menuName = "ScriptableObjects/EnemySO", order = 1)]
public class EnemySO : ScriptableObject
{
    public string name;
    public int vida;
    public TierEnmey tierEnemy;
    public int currentShild;
    public Sprite spriteEnemy;
    public Sprite spriteMuerto;
    public int dmg;
    // TODO Debuff probablemente con una lista de otro so Debuff o algo asi
}
public enum TierEnmey
{
    TIER1, TIER2, TIER3, TIER4
}
