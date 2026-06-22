using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PJSO", menuName = "Scriptable Objects/PJSO")]
public class PJSO : ScriptableObject
{
    public int vida;
    public int vidaMax;
    public int shild;
    public int handSize;

    public int maximumPlayablePinchos; 
    public List<PinchoSO> baraja;
    public List<ObjetoSo> objetos;
}
