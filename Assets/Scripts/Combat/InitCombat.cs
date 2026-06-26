using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitCombat : MonoBehaviour
{
    public GameObject[] Enemys;
    public EnemySO[] posibleEnemySos;
    public TextMeshProUGUI pjVida;
    public TextMeshProUGUI pjShild;
    public TurnsManager TurnsManager;

    // * 5% mas cada nivel
    private const float vidaMult = 1.05f;
    private const float dmgMult = 1.05f;
    private const float shildMult = 1.05f;

    // Cuántos enemigos aparecen: empieza en 1-2 y sube hasta maxEnemigos
    private const int enemigosBase = 1;
    private const int maxEnemigos = 3;      // nunca más de 4 enemigos a la vez
    private const int nivelPorSlot = 2;      // cada 2 niveles se añade un slot
    [Range(0f, 1f)]
    public float baseExtraEnemyChance = 0.5f; // probabilidad base por cada slot extra (ajustable en inspector)

    void Start()
    {
        InitPJ();
        InitEnemy();
    }

    private int Escalar(int valorBase, float multiplicador, int nivel)
    {
        // nivel 1 → valorBase sin cambios; cada nivel extra multiplica
        return Mathf.RoundToInt(valorBase * Mathf.Pow(multiplicador, nivel - 1));
    }

    /// <summary>Devuelve cuántos enemigos pueden aparecer este nivel.</summary>
    private int MaxEnemigosParaNivel(int nivel)
    {
        // sube un slot por cada bloque de nivelPorSlot niveles
        return Mathf.Clamp(enemigosBase + (nivel - 1) / nivelPorSlot, enemigosBase, maxEnemigos);
    }

    private void InitEnemy()
    {
        int nivel = GameManager.Instance.nivelAct;
        int maxSlots = MaxEnemigosParaNivel(nivel);
        int cuantEnemys = GameManager.Instance.notOPEnough ? 3 : ElegirCantidadEnemigos(nivel, maxSlots);
        System.Collections.Generic.HashSet<EnemySO> enemigosUsados = new System.Collections.Generic.HashSet<EnemySO>();

        for (int i = 0; i < Enemys.Length; i++)
        {
            if (i < cuantEnemys)
            {
                Enemys[i].SetActive(true);

                // Elegir base aleatoria según la probabilidad de tiers para el nivel actual
                EnemySO baseOriginal = ElegirEnemyPorNivel(nivel, enemigosUsados);
                enemigosUsados.Add(baseOriginal);

                // Crear instancia única y escalar sus stats
                EnemySO enemyInstance = Instantiate(baseOriginal);
                AplicarEscalado(enemyInstance, baseOriginal, nivel);

                EnemyUI enemyUI = Enemys[i].GetComponent<EnemyUI>();
                enemyUI.InitializeEnemy(enemyInstance);
                TurnsManager.enemigosUI.Add(enemyUI);
                TurnsManager.enemigosSO.Add(enemyInstance);
            }
            else
            {
                Enemys[i].SetActive(false);
            }
        }
    }

    private EnemySO ElegirEnemyPorNivel(int nivel, System.Collections.Generic.HashSet<EnemySO> enemigosUsados = null)
    {
        EnemySO[] pool = posibleEnemySos;

        if (GameManager.Instance.notOPEnough)
        {
            pool = System.Array.FindAll(posibleEnemySos, enemy => enemy != null && enemy.tierEnemy == TierEnmey.TIER4);
        }

        if (enemigosUsados != null)
        {
            EnemySO[] poolSinUsar = System.Array.FindAll(pool, enemy => enemy != null && !enemigosUsados.Contains(enemy));
            if (poolSinUsar.Length > 0)
            {
                pool = poolSinUsar;
            }
        }

        if (pool.Length == 0)
        {
            pool = posibleEnemySos;
        }

        if (GameManager.Instance.notOPEnough && pool.Length > 0)
        {
            return pool[Random.Range(0, pool.Length)];
        }

        float[] tierWeights = GetTierWeightsParaNivel(nivel);
        float totalWeight = 0f;

        foreach (EnemySO enemy in pool)
        {
            int tierIndex = (int)enemy.tierEnemy;
            if (tierIndex >= 0 && tierIndex < tierWeights.Length)
                totalWeight += tierWeights[tierIndex];
        }

        if (totalWeight <= 0f)
        {
            int fallbackIndex = Random.Range(0, pool.Length);
            return pool[fallbackIndex];
        }

        float roll = Random.Range(0f, totalWeight);
        foreach (EnemySO enemy in pool)
        {
            int tierIndex = (int)enemy.tierEnemy;
            if (tierIndex < 0 || tierIndex >= tierWeights.Length)
                continue;

            roll -= tierWeights[tierIndex];
            if (roll <= 0f)
                return enemy;
        }

        return pool[Random.Range(0, pool.Length)];
    }

    private float[] GetTierWeightsParaNivel(int nivel)
    {
        if (nivel <= 4)
            return new float[] { 0.97f, 0.03f, 0f, 0f };
        if (nivel <= 9)
            return new float[] { 0.57f, 0.4f, 0.03f, 0f };
        if (nivel <= 12)
            return new float[] { 0.2f, 0.5f, 0.3f, 0f };
        if (nivel <= 18)
            return new float[] { 0.1f, 0.25f, 0.4f, 0.25f };
        return new float[] { 0f, 0f, 0.4f, 0.6f };
    }

    private int ElegirCantidadEnemigos(int nivel, int maxSlots)
    {
        if (maxSlots <= 1) return 1;

        // La probabilidad de añadir cada slot extra escala ligeramente con el nivel
        float chancePerExtra = Mathf.Clamp01(baseExtraEnemyChance + (nivel - 1) * 0.02f);

        int count = 1;
        for (int s = 2; s <= maxSlots; s++)
        {
            if (Random.value < chancePerExtra)
                count++;
            else
                break; // si fallamos en un slot, dejamos de añadir
        }

        return count;
    }

    private void AplicarEscalado(EnemySO instancia, EnemySO original, int nivel)
    {
        instancia.vidaMax = Escalar(original.vidaMax, vidaMult, nivel);
        instancia.vida = instancia.vidaMax;               // siempre full al entrar
        instancia.dmg = Escalar(original.dmg, dmgMult, nivel);
        instancia.currentShild = Escalar(original.currentShild > 0
                                         ? original.currentShild
                                         : 0, shildMult, nivel);

        // Mantener el tier original del enemigo elegido.
        // El tier ya se decide al seleccionar la plantilla adecuada.
    }

    private void InitPJ()
    {
        pjVida.text = GameManager.Instance.playerSO.vida.ToString();
        pjShild.text = GameManager.Instance.playerSO.shild.ToString();
    }
    public bool HayAlgunEnemigoSeleccionado()
    {
        if (Enemys == null) return false;

        for (int i = 0; i < Enemys.Length; i++)
        {
            if (Enemys[i] == null) continue;
            if (!Enemys[i].activeInHierarchy) continue;

            EnemyUI enemyUI = Enemys[i].GetComponent<EnemyUI>();

            if (enemyUI != null && enemyUI.IsAlive && enemyUI.Fijado)
            {
                return true;
            }
        }

        return false;
    }

}
