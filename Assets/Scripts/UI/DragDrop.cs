using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Canvas canvas;
    public Transform centroRuleta;
    private float offsetRotacion = 90f;
    public bool droppedOnSlot = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Transform initialParent;
    private Vector2 initialAnchoredPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    private Transform lastValidParent;
    private Vector3 lastValidWorldPosition;
    private Quaternion lastValidRotation;

    private Hueco huecoActual;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    public void InitDragDrop(Canvas canvas, Transform centroRuleta)
    {
        this.canvas = canvas;
        this.centroRuleta = centroRuleta;

        GuardarPosicionInicial();
    }

    private void GuardarPosicionInicial()
    {
        initialParent = transform.parent;
        initialAnchoredPosition = rectTransform.anchoredPosition;
        initialRotation = rectTransform.rotation;
        initialScale = transform.localScale;

        lastValidParent = initialParent;
        lastValidWorldPosition = rectTransform.position;
        lastValidRotation = rectTransform.rotation;

        droppedOnSlot = false;
        huecoActual = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        droppedOnSlot = false;

        Hueco.MostrarIndicadores(true);

        if (canvas != null)
        {
            transform.SetParent(canvas.transform, true);
            transform.SetAsLastSibling();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }

        RotarHaciaCentroRuleta();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (canvas == null) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        RotarHaciaCentroRuleta();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!droppedOnSlot)
        {
            VolverAUltimaPosicionValida();
        }
        
        Hueco.MostrarIndicadores(false);
    }

    private void RotarHaciaCentroRuleta()
    {
        if (canvas == null || rectTransform == null) return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null) return;

        Camera canvasCamera = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = canvas.worldCamera;
        }

        Vector2 centroRuletaEnCanvas;

        if (centroRuleta != null)
        {
            Camera camaraMundo = Camera.main;

            if (camaraMundo == null)
            {
                return;
            }

            Vector2 centroRuletaEnPantalla = RectTransformUtility.WorldToScreenPoint(
                camaraMundo,
                centroRuleta.position
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                centroRuletaEnPantalla,
                canvasCamera,
                out centroRuletaEnCanvas
            );
        }
        else centroRuletaEnCanvas = Vector2.zero;
           
        Vector2 pinchoEnCanvas = canvasRect.InverseTransformPoint(rectTransform.position);

        Vector2 direccion = centroRuletaEnCanvas - pinchoEnCanvas;

        if (direccion.sqrMagnitude < 0.0001f) return;

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        rectTransform.rotation = Quaternion.Euler(0f, 0f, angulo + offsetRotacion);
    }

    public void VolverAPosicionOriginal()
    {
        if (huecoActual != null)
        {
            huecoActual.Liberar(this);
            huecoActual = null;
        }

        transform.SetParent(initialParent, false);
        rectTransform.anchoredPosition = initialAnchoredPosition;
        rectTransform.rotation = initialRotation;
        transform.localScale = initialScale;

        lastValidParent = initialParent;
        lastValidWorldPosition = rectTransform.position;
        lastValidRotation = rectTransform.rotation;

        droppedOnSlot = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        PinchosScript pinchosScript = GetComponent<PinchosScript>();

        if (pinchosScript != null)
        {
            pinchosScript.ResetCasilla();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            VolverAPosicionOriginal();
        }
    }

    public Hueco GetHuecoActual()
    {
        return huecoActual;
    }

    public void SetHuecoActual(Hueco nuevoHueco)
    {
        huecoActual = nuevoHueco;
    }

    public void ConfirmarPosicionamientoValida()
    {
        droppedOnSlot = true;

        lastValidParent = transform.parent;
        lastValidWorldPosition = rectTransform.position;
        lastValidRotation = rectTransform.rotation;
    }

    private void VolverAUltimaPosicionValida()
    {
        transform.SetParent(lastValidParent, true);
        rectTransform.position = lastValidWorldPosition;
        rectTransform.rotation = lastValidRotation;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
}