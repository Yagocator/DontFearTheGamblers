using System;
using UnityEngine;
public enum colores
{
    NEGRO, ROJO, VERDE
}
public enum TipoMejora
{
    MULTI, SUMA
}
[CreateAssetMenu(fileName = "ObjetoSo", menuName = "Scriptable Objects/ObjetoSo")]
public class ObjetoSo : ScriptableObject
{
    public String nombre;
    public String desc;

    public Sprite sprite;

    [Header("Mejorar Numero")]
    public bool ActivarMejNum;
    public int[] numsAfectadosNumberMej;
    public TipoMejora tipoNumberMej;
    public int powerNumberMej;
    [Header("Mejora de Colores")]
    public bool ActivarMeColores;
    public colores colorAfectado;
    public int powerColoresMej;
    public TipoMejora tipoColoresMej;
    [Header("Mejora intercamvio de color")]
    public bool ActivarMejoraIntercamvioColor;
    public colores colorOriguinal;
    public colores colorAcomvertir;
}
