using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyInit : MonoBehaviour
{
    public TextMeshProUGUI nameEnemy;
    public TextMeshProUGUI vidaEnemy;
    public EnemySO enemySO;
    public TextMeshProUGUI ShildEnemy;
    public Image ImageEnemy;


    
    void Start()
    {
        nameEnemy.text = enemySO.name;
        vidaEnemy.text = enemySO.vida.ToString();
        ShildEnemy.text = enemySO.currentShild.ToString();
        ImageEnemy.sprite = enemySO.spriteEnemy;
    }
}
