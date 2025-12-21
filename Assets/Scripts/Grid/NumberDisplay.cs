using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 지뢰찾기 스타일의 숫자를 표시하는 클래스
/// 방문한 칸 주변의 미방문 칸에 "주변 8칸의 오브젝트 개수"를 표시함
/// </summary>
public class NumberDisplay : MonoBehaviour
{
    // ===== 필드 =====
    private GridManager _gridManager; // GridManager 참조
    private ObjectSetter _objectSetter;

    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject numberSpritePrefab; // 스프라이트 프리팹
    [SerializeField] private Sprite[] numberSprites; // 1~8 스프라이트 배열

    // 생성된 숫자 오브젝트들 (좌표 → GameObject)
    private Dictionary<Vector2Int, GameObject> _numberObjects = 
        new Dictionary<Vector2Int, GameObject>();

    // 8방향 오프셋 (상하좌우 + 대각선)
    private readonly Vector2Int[] _directions = new Vector2Int[]
    {
        new Vector2Int(-1, 1),  new Vector2Int(0, 1),  new Vector2Int(1, 1),  // 상단 3칸
        new Vector2Int(-1, 0),                         new Vector2Int(1, 0),  // 좌우 2칸
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)  // 하단 3칸
    };

    private void Start()
    {
        _objectSetter = GameManager.Instance.objectSetter;
    }
    // ===== 공개 메서드 =====

    /// <summary>
    /// NumberDisplay 초기화
    /// </summary>
    /// <param name="manager">GridManager 참조</param>
    public void Initialize(GridManager manager)
    {
        _gridManager = manager;
    }

    /// <summary>
    /// 숫자 업데이트
    /// PlayerController의 OnMovementComplete 이벤트로 호출됨
    /// </summary>
    public void UpdateNumbers()
    {
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not set!");
            return;
        }

        // 1단계: 방문한 칸들의 인접 미방문 칸 찾기
        HashSet<Vector2Int> candidateCells = FindAdjacentUnvisitedCells();

        // 2단계: 각 후보 칸의 주변 오브젝트 개수 세기
        foreach (Vector2Int pos in candidateCells)
        {
            int count = CountObjectsAround(pos);

            // 0이 아니면 숫자 표시
            if (count > 0)
            {
                ShowNumber(pos, count);
            }
        }
    }

    // ===== 내부 메서드 =====

    /// <summary>
    /// 방문한 칸들의 인접 미방문 칸 찾기
    /// </summary>
    /// <returns>인접 미방문 칸들의 좌표 집합</returns>
    private HashSet<Vector2Int> FindAdjacentUnvisitedCells()
    {
        HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();

        // 모든 방문한 칸에 대해
        List<Vector2Int> visitedCells = _gridManager.GetAllVisitedCells();

        foreach (Vector2Int visitedPos in visitedCells)
        {
            // 8방향 인접 칸 체크
            foreach (Vector2Int offset in _directions)
            {
                Vector2Int adjacentPos = visitedPos + offset;

                // 그리드 내부이고 미방문인지 확인
                GridCell cell = _gridManager.GetCell(adjacentPos.x, adjacentPos.y);

                if (cell != null && !cell.IsVisited)
                {
                    candidates.Add(adjacentPos);
                }
            }
        }

        return candidates;
    }

    /// <summary>
    /// 특정 칸 주변 8칸의 오브젝트 개수 세기
    /// </summary>
    /// <param name="pos">중심 칸 좌표</param>
    /// <returns>주변 오브젝트 개수 (0~8)</returns>
    private int CountObjectsAround(Vector2Int pos)
    {
        int count = 0;

        // 8방향 체크
        foreach (Vector2Int offset in _directions)
        {
            Vector2Int checkPos = pos + offset;
            GridCell cell = _gridManager.GetCell(checkPos.x, checkPos.y);

            // 칸이 존재하고 오브젝트가 있으면 카운트
            if (cell != null && cell.HasObject)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// 특정 위치에 숫자 텍스트 표시
    /// </summary>
    /// <param name="pos">표시할 위치</param>
    /// <param name="number">표시할 숫자</param>
    private void ShowNumber(Vector2Int pos, int number)
    {
        if (_numberObjects.ContainsKey(pos))
        {
            // 스프라이트 변경
            Image image = _numberObjects[pos].GetComponent<Image>();
            image.sprite = numberSprites[number - 1];
            /*SpriteRenderer spriteRenderer = _numberObjects[pos].GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = numberSprites[number - 1]; // 배열 인덱스는 0부터*/
        }
        else
        {
            /*// 새로 생성
            Vector3 worldPos = GridToWorldPosition(pos);
            GameObject numberObj = Instantiate(numberSpritePrefab, worldPos, Quaternion.identity);

            SpriteRenderer spriteRenderer = numberObj.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = numberSprites[number - 1]; // 1 → index 0
            spriteRenderer.sortingOrder = 10; // 앞에 표시

            _numberObjects[pos] = numberObj;*/
            // 새로 생성
            GameObject numberObj = Instantiate(numberSpritePrefab, _objectSetter.transform);
            SoundManager.Instance.PlaySFX(SFXName.방향카드화살표);
            
            // RectTransform 설정
            RectTransform numberRT = numberObj.GetComponent<RectTransform>();
            
            // 그리드 셀 위치로 이동
            PlaceAtGridPosition(numberRT, pos);
            
            // Image 컴포넌트 설정
            Image image = numberObj.GetComponent<Image>();
            if (image == null)
            {
                image = numberObj.AddComponent<Image>();
            }
            
            image.sprite = numberSprites[number - 1]; // 1 → index 0
            image.raycastTarget = false; // 클릭 방지
            
            // 오브젝트 이름 설정 (디버깅용)
            numberObj.name = $"Number_{pos.x}_{pos.y}_{number}";
            
            _numberObjects[pos] = numberObj;
        }
    }
    
    /// <summary>
    /// UI 오브젝트를 특정 그리드 위치에 배치
    /// </summary>
    /// <param name="objRect">배치할 오브젝트의 RectTransform</param>
    /// <param name="gridPos">그리드 좌표</param>
    private void PlaceAtGridPosition(RectTransform objRect, Vector2Int gridPos)
    {
        // 그리드 인덱스 계산
        int index = gridPos.y * 16 + gridPos.x;
        
        if (index < 0 || index >= gridLayoutGroup.transform.childCount)
        {
            Debug.LogError($"Invalid grid index: {index} for position ({gridPos.x}, {gridPos.y})");
            return;
        }
        
        // 해당 그리드 셀 가져오기
        RectTransform gridCell = gridLayoutGroup.transform.GetChild(index) as RectTransform;
        
        // 월드 위치 복사
        objRect.position = gridCell.position;
        
        // 크기도 그리드 셀과 동일하게 (선택사항)
        //objRect.sizeDelta = gridCell.sizeDelta;
    }

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환
    /// </summary>
    /// <param name="gridPos">그리드 좌표</param>
    /// <returns>월드 좌표</returns>
    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        /*int index = gridPos.y * 16 + gridPos.x;
        RectTransform gridCell = gridLayoutGroup.transform.GetChild(index) as RectTransform;*/

        // Z축을 -0.5로 해서 그리드보다 앞에 표시
        return new Vector3(gridPos.x, gridPos.y, -0.5f);
    }

    // Text로할때
    /*/// <summary>
    /// 숫자에 따른 색상 반환 (지뢰찾기 스타일)
    /// </summary>
    /// <param name="num">숫자 (1~8)</param>
    /// <returns>해당 숫자의 색상</returns>
    private Color GetColorForNumber(int num)
    {
        switch (num)
        {
            case 1: return Color.blue;
            case 2: return Color.green;
            case 3: return Color.red;
            case 4: return new Color(0, 0, 0.5f);    // 진한 파랑
            case 5: return new Color(0.5f, 0, 0);    // 진한 빨강
            case 6: return new Color(0, 0.5f, 0.5f); // 청록색
            case 7: return Color.black;
            case 8: return Color.gray;
            default: return Color.black;
        }
    }*/
}