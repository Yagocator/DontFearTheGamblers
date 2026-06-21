using System;
using UnityEngine;
public enum colores
{
    NEGRO, ROJO, VERDE
}
[CreateAssetMenu(fileName = "ObjetoSo", menuName = "Scriptable Objects/ObjetoSo")]
public class ObjetoSo : ScriptableObject
{
    public String nombre;
    public String desc;

    public Sprite sprite;

    [Header("Mejorar Numero")]
    public bool ActivarMejNum;
    public int[] numsAfectados;
    [Header("Mejora de Colores")]
    public bool ActivarMeColores;
    public colores colorAfectado;


}
