using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    InputSystem_Actions action;
    Vector2 _touchPosition;

    bool _shouldDrag = false;
    Vector2 _dragOffset;

    [SerializeField] LeanTweenType _tweenType;
    [SerializeField] float _tweenSpeed;

    Canvas _parentCanvas;

    private void Start()
    {
        // Esconde o teclado ao iniciar
        CanvasGroup childCanvas = transform.GetChild(0).GetComponent<CanvasGroup>();
        childCanvas.alpha = 0f;
        childCanvas.blocksRaycasts = false;

        // Opcional: joga um pouco para fora do lado esquerdo se quiser
        Vector3 pos = transform.GetChild(0).localPosition;
        pos.x = -45f;
        transform.GetChild(0).localPosition = pos;
    }


    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
        if (_parentCanvas == null)
            Debug.LogError(" O teclado não está dentro de um Canvas!");

        action = new InputSystem_Actions();
        action.UI.Point.performed += ctx => _touchPosition = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }


    public void OpenCloseKeyboard(bool open)
    {
        if (_shouldDrag) return;

        CanvasGroup childCanvas = transform.GetChild(0).GetComponent<CanvasGroup>();

        LeanTween.alphaCanvas(childCanvas, open ? 1 : 0, _tweenSpeed)
            .setEase(_tweenType)
            .setOnStart(() =>
            {
                LeanTween.moveLocalX(childCanvas.gameObject, open ? 0 : -45, _tweenSpeed)
                .setEase(_tweenType);
            })
            .setOnComplete(() =>
            {
                childCanvas.blocksRaycasts = open;
            })
            .setDelay(0.05f);
    }


    public void DragKeyboard(bool drag)
    {
        if (_parentCanvas == null) return;

        _shouldDrag = drag;

        if (drag)
        {
            RectTransform rect = transform as RectTransform;
            RectTransform canvasRect = _parentCanvas.transform as RectTransform;

            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                _touchPosition,
                _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _parentCanvas.worldCamera,
                out localPos
            );

            _dragOffset = rect.anchoredPosition - localPos;
        }
    }


    void Update()
    {
        if (!_shouldDrag || _parentCanvas == null)
            return;

        RectTransform rect = transform as RectTransform;
        RectTransform canvasRect = _parentCanvas.transform as RectTransform;

        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            _touchPosition,
            _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _parentCanvas.worldCamera,
            out localPos
        );

        rect.anchoredPosition = localPos + _dragOffset;

        ClampPosition(rect, canvasRect);
    }


    void ClampPosition(RectTransform rect, RectTransform canvasRect)
    {
        float screenWidth = canvasRect.rect.width;
        float screenHeight = canvasRect.rect.height;

        Vector2 size = rect.sizeDelta;
        Vector2 pos = rect.anchoredPosition;

        float minX = -screenWidth / 2 + size.x / 2;
        float maxX = screenWidth / 2 - size.x / 2;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        float minY = -screenHeight / 2 + size.y / 2;
        float maxY = screenHeight / 2 - size.y / 2;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rect.anchoredPosition = pos;
    }
}
