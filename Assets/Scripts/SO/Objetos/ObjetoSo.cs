using System;
using UnityEngine;
public enum TipoMejora
{
    MULTI, SUMA
}
public enum Rareza
{
    Common, Uncommon, Rare, Epic, Legendary, LetItRide
}

[CreateAssetMenu(fileName = "ObjetoSo", menuName = "Scriptable Objects/ObjetoSo")]
public class ObjetoSo : ScriptableObject
{
    public String nombre;
    public String desc;
    public Sprite sprite;


    [Header("Mejorar Numero")]
    public bool mejoraNumero;
    public int[] mejoraNumeroNumsAfectados;
    public TipoMejora mejoraNumeroTipo;
    public int mejoraNumeroIncremento;


    [Header("Mejora de Colores")]
    public bool mejoraColor;
    public Colores mejoraColorColorAfectado;
    public TipoMejora mejoraColorTipo;
    public int mejoraColorIncremento;



    [Header("Mejora intercamvio de color")]
    public bool mejoraIntcvColor;
    public Colores mejoraIntcvColorColorAfectado;
    public Colores mejoraIntcvColorColorNuevo;

    [Header("Rareza")]
    public Rareza rareza; 
}
