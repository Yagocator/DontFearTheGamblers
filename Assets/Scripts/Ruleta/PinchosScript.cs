using UnityEngine;

public enum PinchosType
{
    Attack,
    Defense,
    Heal
}

public class PinchosScript : MonoBehaviour
{
    public int suma;
    public int factorEscala;
    public PinchosType type;

    private int casilla = -1;
    private float rotation;

    public void init(int type, int suma, int factorEscala)
    {
        this.type = (PinchosType)type;
        this.suma = suma;
        this.factorEscala = factorEscala;
    }

    public void setPinchosType(int type)
    {
        this.type = (PinchosType)type;
    }

    public PinchosType getType()
    {
        return type;
    }

    public int getSuma()
    {
        return suma;
    }

    public int getFactorEscala()
    {
        return factorEscala;
    }

    public void setSuma(int suma)
    {
        this.suma = suma;
    }

    public void setFactorEscala(int factorEscala)
    {
        this.factorEscala = factorEscala;
    }

    public void setCasilla(int casilla)
    {
        this.casilla = casilla;
    }

    public int getCasilla()
    {
        return casilla;
    }

    public void setRotation(float rotation)
    {
        this.rotation = rotation;
    }

    public float getRotation()
    {
        return rotation;
    }

    public bool EstaColocado()
    {
        return casilla >= 0;
    }

    public void ResetCasilla()
    {
        casilla = -1;
        rotation = 0f;
    }

    public int casillaPincho()
    {
        float parte = 360f / 37f;
        float halfParte = parte / 2f;

        float z = transform.localEulerAngles.z;
        int valor = (int)((z + halfParte) / parte);

        return valor;
    }
}