// === DirectionSelector.cs ===
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// 플레이어 주변에 방향 선택 화살표를 표시하고 입력을 받는 클래스
/// </summary>
public class DirectionSelector : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject arrowButtonPrefab; // 화살표 버튼 프리팹
    [SerializeField] private Canvas directionCanvas; // 방향 선택 UI 캔버스
    
    [Header("Arrow Settings")]
    [SerializeField] private float arrowDistance = 100f; // 플레이어로부터 화살표 거리
    
    private List<GameObject> _activeArrows = new List<GameObject>();
    private Action<Direction> _onDirectionSelected; // 방향 선택 시 콜백
    private Action<bool> _onChoiceSelected; // bool 선택 시 콜백 (바닷게용)
    
    private void Awake()
    {
        // 시작 시 UI 숨기기
        if (directionCanvas != null)
            directionCanvas.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 4방향 화살표 표시 (앵커 카드용)
    /// </summary>
    public void ShowDirectionChoice(Action<Direction> callback)
    {
        _onDirectionSelected = callback;
        directionCanvas.gameObject.SetActive(true);
        
        // 4방향 화살표 생성
        CreateArrow(Direction.Up);
        CreateArrow(Direction.Down);
        CreateArrow(Direction.Left);
        CreateArrow(Direction.Right);
    }
    
    /// <summary>
    /// 좌우 화살표만 표시 (바닷게 카드용)
    /// </summary>
    public void ShowLeftRightChoice(Action<bool> callback)
    {
        _onChoiceSelected = callback;
        directionCanvas.gameObject.SetActive(true);
        
        // 좌우 화살표만 생성
        CreateArrowForChoice(true);  // 왼쪽
        CreateArrowForChoice(false); // 오른쪽
    }
    
    /// <summary>
    /// 방향 화살표 버튼 생성
    /// </summary>
    private void CreateArrow(Direction dir)
    {
        GameObject arrow = Instantiate(arrowButtonPrefab, directionCanvas.transform);
        
        // 위치 설정
        Vector2 offset = GetOffsetForDirection(dir);
        arrow.GetComponent<RectTransform>().anchoredPosition = offset;
        
        // 회전 설정
        float angle = GetAngleForDirection(dir);
        arrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);
        
        // 버튼 이벤트 연결
        Button button = arrow.GetComponent<Button>();
        button.onClick.AddListener(() => OnDirectionButtonClicked(dir));
        
        _activeArrows.Add(arrow);
    }
    
    /// <summary>
    /// 좌우 선택용 화살표 생성
    /// </summary>
    private void CreateArrowForChoice(bool isLeft)
    {
        GameObject arrow = Instantiate(arrowButtonPrefab, directionCanvas.transform);
        
        // 위치 설정 (좌우만)
        Vector2 offset = isLeft ? 
            new Vector2(-arrowDistance, 0) : 
            new Vector2(arrowDistance, 0);
        arrow.GetComponent<RectTransform>().anchoredPosition = offset;
        
        // 회전 설정
        float angle = isLeft ? 90f : -90f;
        arrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);
        
        // 버튼 이벤트 연결
        Button button = arrow.GetComponent<Button>();
        button.onClick.AddListener(() => OnChoiceButtonClicked(isLeft));
        
        _activeArrows.Add(arrow);
    }
    
    /// <summary>
    /// 방향 버튼 클릭 시 호출
    /// </summary>
    private void OnDirectionButtonClicked(Direction dir)
    {
        HideArrows();
        _onDirectionSelected?.Invoke(dir);
    }
    
    /// <summary>
    /// 좌우 선택 버튼 클릭 시 호출
    /// </summary>
    private void OnChoiceButtonClicked(bool isLeft)
    {
        HideArrows();
        _onChoiceSelected?.Invoke(isLeft);
    }
    
    /// <summary>
    /// 모든 화살표 제거 및 UI 숨기기
    /// </summary>
    private void HideArrows()
    {
        foreach (var arrow in _activeArrows)
        {
            Destroy(arrow);
        }
        _activeArrows.Clear();
        
        directionCanvas.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 방향에 따른 화살표 위치 오프셋 계산
    /// </summary>
    private Vector2 GetOffsetForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:    return new Vector2(0, arrowDistance);
            case Direction.Down:  return new Vector2(0, -arrowDistance);
            case Direction.Left:  return new Vector2(-arrowDistance, 0);
            case Direction.Right: return new Vector2(arrowDistance, 0);
            default: return Vector2.zero;
        }
    }
    
    /// <summary>
    /// 방향에 따른 화살표 회전 각도 계산
    /// </summary>
    private float GetAngleForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:    return 0f;
            case Direction.Right: return -90f;
            case Direction.Down:  return 180f;
            case Direction.Left:  return 90f;
            default: return 0f;
        }
    }
}