using JetBrains.Annotations;
using UnityEngine;



public class PinchosScript : MonoBehaviour
{
    public PinchoSO pinchoSO;
    private int casilla = -1;
    private float rotation;

    public void init(int type, int suma, int factorEscala)
    {
        pinchoSO.type = (PinchosType)type;
        pinchoSO.suma = suma;
        pinchoSO.factorEscala = factorEscala;
        pinchoSO.state = (Estado)0;
    }

    public void setPinchosType(int type)
    {
        pinchoSO.type = (PinchosType)type;
    }

    public PinchosType getType()
    {
        return pinchoSO.type;
    }

    public int getSuma()
    {
        return pinchoSO.suma;
    }

    public int getFactorEscala()
    {
        return pinchoSO.factorEscala;
    }

    public void setSuma(int suma)
    {
        pinchoSO.suma = suma;
    }

    public void setFactorEscala(int factorEscala)
    {
        pinchoSO.factorEscala = factorEscala;
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
