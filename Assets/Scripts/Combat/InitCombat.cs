using UnityEngine;

public class InitCombat : MonoBehaviour
{
    public TierEnmey DificultadDelNivel;
    public int nivel;
    public GameObject[] Enemys;

    public EnemySO[] posibleEnemySos;
        void Start()
    {
        int cuantEnemys = Random.Range(1, Enemys.Length);

        for (int i = 0; i < cuantEnemys; i++)
        {
            Enemys[i].SetActive(true);
            while (true)
            {
                int indexEnemySO = Random.Range(0, posibleEnemySos.Length);
                if (posibleEnemySos[indexEnemySO].tierEnemy == DificultadDelNivel)
                {
                    Enemys[i].GetComponent<EnemyInit>().enemySO = posibleEnemySos[indexEnemySO];
                    break;
                }
            }

        }
    }
}
