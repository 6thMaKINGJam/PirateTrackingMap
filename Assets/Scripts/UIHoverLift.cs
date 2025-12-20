using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class UIHoverLift : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Move")]
    [SerializeField] private float liftY = 8f;        // 위로 이동 픽셀
    [SerializeField] private float duration = 0.08f;  // 이동 시간(초)

    [Header("Behavior")]
    [SerializeField] private bool useUnscaledTime = true; // Time.timeScale=0에서도 동작

    private RectTransform rt;
    private Vector2 basePos;
    private Vector2 targetPos;

    private float t = 1f;
    private bool animating = false;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        basePos = rt.anchoredPosition;
        targetPos = basePos;
    }

    void OnEnable()
    {
        // 비활성/활성 반복될 때 위치 틀어지는 거 방지
        basePos = rt.anchoredPosition;
        targetPos = basePos;
        t = 1f;
        animating = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartMove(basePos + new Vector2(0f, liftY));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartMove(basePos);
    }

    private void StartMove(Vector2 to)
    {
        targetPos = to;
        t = 0f;
        animating = true;
    }

    void Update()
    {
        if (!animating) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        t += (duration <= 0f) ? 1f : dt / duration;

        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, Mathf.Clamp01(t));

        if (t >= 1f)
        {
            rt.anchoredPosition = targetPos;
            animating = false;
        }
    }
}
