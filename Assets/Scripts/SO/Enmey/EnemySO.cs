using UnityEditor.Animations;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemySO", menuName = "ScriptableObjects/EnemySO", order = 1)]
public class EnemySO : ScriptableObject
{
    public string name;
    public int vida;
    public int vidaMax;
    public TierEnmey tierEnemy;
    public int currentShild;
    public RuntimeAnimatorController AnimatorEnemy;
    public Sprite spriteMuerto;
    public int dmg;

    public override string ToString()
    {
        return $"Enemy: {name} | Tier: {tierEnemy} | Vida: {vida}/{vidaMax} | Escudo: {currentShild} | Daño: {dmg} | Animator: {(AnimatorEnemy != null ? AnimatorEnemy.name : "null")} | Sprite Muerto: {(spriteMuerto != null ? spriteMuerto.name : "null")}";
    }
}
public enum TierEnmey
{
    TIER1, TIER2, TIER3, TIER4
}
