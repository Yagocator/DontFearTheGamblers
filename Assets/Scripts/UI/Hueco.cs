using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hueco : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public float rotationOffset;

    [Header("Indicador visual")]
    public GameObject circuloIndicador;

    private DragDrop pinchoActual;

    private static readonly List<Hueco> todosLosHuecos = new List<Hueco>();

    [Header("Audio")]
    public AudioClip pinchoConfirmedSFX; 
    private void Awake()
    {
        if (circuloIndicador != null)
        {
            circuloIndicador.SetActive(false);

            Image image = circuloIndicador.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = false;
                image.color = Color.white;
            }
        }
    }

    private void OnEnable()
    {
        if (!todosLosHuecos.Contains(this))
        {
            todosLosHuecos.Add(this);
        }
    }

    private void OnDisable()
    {
        todosLosHuecos.Remove(this);
    }

    public void Init(int slotIndex, float rotationOffset)
    {
        this.slotIndex = slotIndex;
        this.rotationOffset = rotationOffset;

        if (circuloIndicador != null)
        {
            circuloIndicador.SetActive(false);
        }
    }

    public static void MostrarIndicadores(bool mostrar)
    {
        for (int i = 0; i < todosLosHuecos.Count; i++)
        {
            if (mostrar && todosLosHuecos[i] != null && todosLosHuecos[i].pinchoActual == null) todosLosHuecos[i].MostrarIndicador(mostrar);                
            else if (!mostrar && todosLosHuecos[i] != null) todosLosHuecos[i].MostrarIndicador(mostrar);
          }
    }

    private void MostrarIndicador(bool mostrar)
    {
        if (circuloIndicador != null)
        {
            circuloIndicador.SetActive(mostrar);
        }
    }

    public bool EstaOcupado()
    {
        return pinchoActual != null;
    }

    public DragDrop GetPinchoActual()
    {
        return pinchoActual;
    }

    public void Liberar(DragDrop pincho)
    {
        if (pinchoActual == pincho)
        {
            pinchoActual = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        GameObject pinchoObject = eventData.pointerDrag;

        DragDrop dragDrop = pinchoObject.GetComponent<DragDrop>();
        if (dragDrop == null) return;

        if (!dragDrop.ArrastreActivo())
        {
            return;
        }

        if (RuletaController.Instance != null && !RuletaController.Instance.PuedeMoverPincho(dragDrop))
        {
            return;
        }

        if (pinchoActual != null && pinchoActual != dragDrop) return;
    
        Hueco huecoAnterior = dragDrop.GetHuecoActual();

        if (huecoAnterior != null && huecoAnterior != this)
        {
            huecoAnterior.Liberar(dragDrop);
        }

        pinchoActual = dragDrop;

        RectTransform pinchoRect = pinchoObject.GetComponent<RectTransform>();
        RectTransform huecoRect = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();

        if (pinchoRect != null && huecoRect != null)
        {
            if (canvas != null)
            {
                pinchoRect.SetParent(canvas.transform, true);
            }

            pinchoRect.position = huecoRect.position;
            pinchoRect.rotation = huecoRect.rotation;
        }
        else
        {
            pinchoObject.transform.position = transform.position;
            pinchoObject.transform.rotation = transform.rotation;
        }

        pinchoObject.transform.SetAsLastSibling();

        CanvasGroup canvasGroup = pinchoObject.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        PinchosScript pinchosScript = pinchoObject.GetComponent<PinchosScript>();

        if (pinchosScript != null)
        {
            pinchosScript.setCasilla(slotIndex);
            pinchosScript.setRotation(rotationOffset);
        }

        dragDrop.SetHuecoActual(this);
        dragDrop.ConfirmarPosicionamientoValida();
        GameManager.Instance.PlaySFX(pinchoConfirmedSFX);
        MostrarIndicadores(false);
    }

    public void setPinchoConfirmedSFX(AudioClip sound) {pinchoConfirmedSFX = sound; }
}