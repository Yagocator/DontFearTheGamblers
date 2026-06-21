using UnityEngine;
using TMPro; 

public enum Colores
{
    VERDE, NEGRO, ROJO, 
}

public class SlootScript : MonoBehaviour
{
    private int valor;

    private Colores color; 

    private int suma = 0; 

    private int factorEscala = 1; 

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpriteRenderer spriteRenderer;

    public TextMeshProUGUI text; 

    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void setValor(int v) { 
        valor = v; 
        paintValorModified();
    }
    public int getValor() { return valor; }

    public int getValorModifiedSloot() { return (valor + suma) * factorEscala;}
    public void setColor(int c) { 
        color = (Colores)c; 
        paintSloot(); 
    }
    public void setColor(Colores c) {
        color = c;   
        paintSloot();  
    }
    public Colores getColor() { return color; }

    private void paintSloot()
    {
        if (color == Colores.VERDE) spriteRenderer.color = Color.green; 
        else if (color == Colores.NEGRO) spriteRenderer.color = Color.black; 
        else if (color == Colores.ROJO) spriteRenderer.color = Color.red; 
    }

    private void paintValorModified()
    {
        text.text = ((valor + suma) * factorEscala).ToString(); 
    }
    public void initSloot()
    {
        paintSloot(); 
        text.text = valor.ToString();
    }

    public void increaseValor(int incremento)
    {
        suma += incremento; 
        text.color = Color.yellow; 
        paintValorModified(); 
    }
    public void multiplyValor(int mult)
    {
        factorEscala *= mult; 
        text.color = Color.yellow; 
        paintValorModified(); 
    }
}
