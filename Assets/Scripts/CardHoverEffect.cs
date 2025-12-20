using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    [SerializeField] private float hoverScale = 1.2f; // 호버 시 크기 (1.2 = 20% 증가)
    [SerializeField] private float hoverDuration = 0.2f; // 애니메이션 시간
    [SerializeField] private int hoverSortOrderBoost = 10; // 호버 시 추가 sort order
    
    [Header("Position Offset")]
    [SerializeField] private Vector2 hoverOffset = new Vector2(0, 20f); // 호버 시 위치 이동 (위로 20픽셀)
    
    [Header("Hover Detection")]
    [SerializeField] private bool useCustomHoverArea = true; // 커스텀 호버 영역 사용
    [SerializeField] private Vector2 hoverAreaSize = new Vector2(100, 200); // 호버 감지 영역 크기
    [SerializeField] private Vector2 hoverAreaOffset = Vector2.zero; // 호버 영역 오프셋
    
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private GraphicRaycaster raycaster;
    private Image image;
    
    // 원래 상태 저장
    private Vector3 originalScale;
    private Vector2 originalPosition;
    private int originalSortOrder;
    private Canvas cardCanvas; // 각 카드의 개별 Canvas
    
    private Coroutine currentAnimation;
    private bool isHovering = false;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        
        // 원래 상태 저장
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        
        // 부모 Canvas 찾기
        parentCanvas = GetComponentInParent<Canvas>();
        
        // 각 카드에 개별 Canvas 추가 (Sort Order 제어용)
        SetupCardCanvas();
        
        // 커스텀 호버 영역 사용 시 Image Raycast 설정
        if (useCustomHoverArea && image != null)
        {
            // Image의 Raycast Target은 유지하되, 영역만 제한
            image.raycastTarget = true;
        }
    }
    
    private void Update()
    {
        // 커스텀 호버 영역을 사용할 때 수동으로 마우스 체크
        if (useCustomHoverArea)
        {
            CheckCustomHover();
        }
    }
    
    private void CheckCustomHover()
    {
        // 로컬 마우스 위치 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPoint
        );
        
        // 커스텀 영역 내에 있는지 체크
        Vector2 adjustedPoint = localPoint - hoverAreaOffset;
        bool isInside = Mathf.Abs(adjustedPoint.x) <= hoverAreaSize.x / 2f &&
                        Mathf.Abs(adjustedPoint.y) <= hoverAreaSize.y / 2f;
        
        // 상태 변경
        if (isInside && !isHovering)
        {
            OnCustomPointerEnter();
        }
        else if (!isInside && isHovering)
        {
            OnCustomPointerExit();
        }
    }
    
    private void OnCustomPointerEnter()
    {
        isHovering = true;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateHover(true));
    }
    
    private void OnCustomPointerExit()
    {
        isHovering = false;
        
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateHover(false));
    }
    
    private void SetupCardCanvas()
    {
        // 이미 Canvas가 있는지 확인
        cardCanvas = GetComponent<Canvas>();
        
        if (cardCanvas == null)
        {
            cardCanvas = gameObject.AddComponent<Canvas>();
        }
        
        cardCanvas.overrideSorting = true;
        originalSortOrder = cardCanvas.sortingOrder;
        
        // GraphicRaycaster 추가 (없으면)
        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 커스텀 호버 영역 사용 시 이 메서드는 무시
        if (useCustomHoverArea) return;
        
        if (isHovering) return;
        
        isHovering = true;
        
        // 진행 중인 애니메이션 중단
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // 호버 애니메이션 시작
        currentAnimation = StartCoroutine(AnimateHover(true));
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // 커스텀 호버 영역 사용 시 이 메서드는 무시
        if (useCustomHoverArea) return;
        
        if (!isHovering) return;
        
        isHovering = false;
        
        // 진행 중인 애니메이션 중단
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // 원래 상태로 복귀 애니메이션
        currentAnimation = StartCoroutine(AnimateHover(false));
    }
    
    private IEnumerator AnimateHover(bool hover)
    {
        float elapsed = 0f;
        
        Vector3 startScale = rectTransform.localScale;
        Vector2 startPosition = rectTransform.anchoredPosition;
        int startSortOrder = cardCanvas.sortingOrder;
        
        Vector3 targetScale = hover ? originalScale * hoverScale : originalScale;
        Vector2 targetPosition = hover ? originalPosition + hoverOffset : originalPosition;
        int targetSortOrder = hover ? originalSortOrder + hoverSortOrderBoost : originalSortOrder;
        
        // Sort Order는 즉시 변경 (호버 시작할 때만)
        if (hover)
        {
            cardCanvas.sortingOrder = targetSortOrder;
        }
        
        while (elapsed < hoverDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / hoverDuration);
            
            // Ease Out 효과
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            
            // 크기 애니메이션
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
            
            // 위치 애니메이션
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, smoothT);
            
            yield return null;
        }
        
        // 최종 값 설정
        rectTransform.localScale = targetScale;
        rectTransform.anchoredPosition = targetPosition;
        
        // Sort Order 복귀 (호버 종료 시)
        if (!hover)
        {
            cardCanvas.sortingOrder = targetSortOrder;
        }
        
        currentAnimation = null;
    }
    
    // 원래 상태로 즉시 리셋 (필요시 외부에서 호출)
    public void ResetCard()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        isHovering = false;
        rectTransform.localScale = originalScale;
        rectTransform.anchoredPosition = originalPosition;
        cardCanvas.sortingOrder = originalSortOrder;
    }
    
    // Inspector에서 원래 상태 업데이트 (카드 위치 변경 후 사용)
    [ContextMenu("Update Original State")]
    public void UpdateOriginalState()
    {
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        if (cardCanvas != null)
        {
            originalSortOrder = cardCanvas.sortingOrder;
        }
        Debug.Log($"카드 '{gameObject.name}'의 원래 상태가 업데이트되었습니다.");
    }
    
    // Scene View에서 호버 영역 시각화
    private void OnDrawGizmosSelected()
    {
        if (!useCustomHoverArea || rectTransform == null) return;
        
        // 월드 좌표로 변환
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        // 카드 중심점
        Vector3 center = (corners[0] + corners[2]) / 2f;
        center += (Vector3)hoverAreaOffset;
        
        // 호버 영역 그리기
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        
        // 2D이므로 XY 평면에 그리기
        Vector3 size = new Vector3(hoverAreaSize.x, hoverAreaSize.y, 0.1f);
        
        // Canvas의 스케일 고려
        Vector3 lossyScale = rectTransform.lossyScale;
        size.x *= lossyScale.x;
        size.y *= lossyScale.y;
        
        Gizmos.DrawCube(center, size);
        
        // 테두리
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
}
