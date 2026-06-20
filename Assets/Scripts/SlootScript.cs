using UnityEngine;
using TMPro; 
public class SlootScript : MonoBehaviour
{
    private int valor;

    private int color; 

    private int suma; 

    private int factorEscala; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpriteRenderer spriteRenderer;

    public TextMeshProUGUI text; 

    void Start()
    {   
        factorEscala = 1; 
        suma = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void setValor(int v) { valor = v; }
    public int getValor() { return (valor + suma) * factorEscala; }
    public void setColor(int c) { color = c; }
    public int getColor() { return color; }

    private void paintSloot()
    {
        if (color == 0) spriteRenderer.color = Color.green; 
        else if (color == 1) spriteRenderer.color = Color.black; 
        else if (color == 2) spriteRenderer.color = Color.red; 
    }
    public void initSloot()
    {
        paintSloot(); 
        text.text = valor.ToString();
    }


}
