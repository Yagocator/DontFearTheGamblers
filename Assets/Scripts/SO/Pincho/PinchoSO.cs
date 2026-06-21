using System;
using Unity.VisualScripting;
using UnityEngine;

public enum Estado
{
    Disponible_para_robar,
    Mano,
    Descarte

}
public enum PinchosType
{
    Attack,
    Defense,
    Heal
}

[CreateAssetMenu(fileName = "PinchoSO", menuName = "Scriptable Objects/PinchoSO")]
public class PinchoSO : ScriptableObject
{
    public int suma;
    public int factorEscala;
    public PinchosType type;

    public Estado state;
    public Sprite sprite;

    public String nombre;
    public String desc;

}
