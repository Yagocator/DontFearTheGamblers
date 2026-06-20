using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitCombat : MonoBehaviour
{
    public TierEnmey DificultadDelNivel;
    public int nivel;
    public GameObject[] Enemys;

    public EnemySO[] posibleEnemySos;

    public TextMeshProUGUI pjVida;
    public TextMeshProUGUI pjShild;
    public TurnsManager TurnsManager;
    void Start()
    {
        InitPJ();
        InitEnemy();
    }
    private void InitEnemy()
    {
        int cuantEnemys = Random.Range(1, 4); // da 1, 2 o 3

        for (int i = 0; i < Enemys.Length; i++)
        {
            if (i < cuantEnemys)
            {
                Enemys[i].SetActive(true);

                EnemyUI enemyUI = Enemys[i].GetComponent<EnemyUI>();

                while (true)
                {
                    int indexEnemySO = Random.Range(0, posibleEnemySos.Length);
                    if (posibleEnemySos[indexEnemySO].tierEnemy == DificultadDelNivel)
                    {
                        enemyUI.InitializeEnemy(posibleEnemySos[indexEnemySO]);
                        TurnsManager.enemigosUI.Add(enemyUI);
                        TurnsManager.enemigosSO.Add(posibleEnemySos[indexEnemySO]);
                        break;
                    }
                }
            }
            else
            {
                Enemys[i].SetActive(false); // esto es lo que faltaba
            }
        }
    }

    private void InitPJ()
    {
        pjVida.text = GameManager.Instance.playerSO.vida.ToString();
        pjShild.text = GameManager.Instance.playerSO.shild.ToString();
    }
}
