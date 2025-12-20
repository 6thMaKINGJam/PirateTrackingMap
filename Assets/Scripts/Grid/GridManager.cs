using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 16x16 그리드의 데이터를 관리하는 클래스
/// 각 칸의 방문 여부, 오브젝트 존재 여부 등을 저장하고 제공
/// </summary>
public class GridManager : MonoBehaviour
{
    private const int GRID_SIZE = 16; // 그리드 크기

    private GridCell[,] grid = new GridCell[GRID_SIZE, GRID_SIZE]; // 16x16 그리드 배열
    private List<Vector2Int> visitedPath = new List<Vector2Int>(); // 방문 경로 (순서대로)

    /// <summary>
    /// 그리드 초기화 - 모든 칸의 GridCell 생성
    /// 오브젝트 배치는 외부에서 SetObject() 호출로 처리
    /// </summary>
    public void Initialize()
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                grid[x, y] = new GridCell(x, y);
            }
        }
    }

    /// <summary>
    /// 특정 칸에 오브젝트 배치 (외부 GameManager에서 호출)
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    public void SetObject(int x, int y)
    {
        if (IsInBounds(x, y))
        {
            grid[x, y].HasObject = true;
        }
    }

    /// <summary>
    /// 특정 칸을 방문했다고 표시
    /// PlayerController의 OnCellVisited 이벤트로 호출됨
    /// </summary>
    /// <param name="pos">방문한 칸의 좌표</param>
    public void MarkCellVisited(Vector2Int pos)
    {
        if (!IsInBounds(pos.x, pos.y))
            return;

        GridCell cell = grid[pos.x, pos.y];

        // 중복 방문 체크 (같은 칸을 여러 번 밟을 수 있음)
        if (!cell.IsVisited)
        {
            cell.MarkAsVisited();
            visitedPath.Add(pos);
        }
    }

    /// <summary>
    /// 좌표가 그리드 범위 내에 있는지 확인
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    /// <returns>범위 내면 true, 아니면 false</returns>
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < GRID_SIZE && y >= 0 && y < GRID_SIZE;
    }

    /// <summary>
    /// 특정 좌표의 GridCell 가져오기
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    /// <returns>해당 칸의 GridCell, 범위 밖이면 null</returns>
    public GridCell GetCell(int x, int y)
    {
        if (IsInBounds(x, y))
            return grid[x, y];
        return null;
    }

    /// <summary>
    /// 플레이어가 방문한 경로 리스트 반환 (순서 보장)
    /// PathVisualizer가 선을 그릴 때 사용
    /// </summary>
    /// <returns>방문한 칸들의 좌표 리스트</returns>
    public List<Vector2Int> GetVisitedPath()
    {
        return visitedPath;
    }

    /// <summary>
    /// 방문한 모든 칸의 좌표 반환 (순서 무관)
    /// NumberDisplay가 숫자를 표시할 위치를 찾을 때 사용
    /// </summary>
    /// <returns>방문한 모든 칸의 좌표 리스트</returns>
    public List<Vector2Int> GetAllVisitedCells()
    {
        List<Vector2Int> visited = new List<Vector2Int>();

        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (grid[x, y].IsVisited)
                    visited.Add(new Vector2Int(x, y));
            }
        }

        return visited;
    }
    
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.red;
    
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                if (grid[x, y] != null && grid[x, y].HasObject)
                {
                    Vector3 pos = new Vector3(x, y, 0);
                    Gizmos.DrawWireCube(pos, Vector3.one * 0.8f);
                }
            }
        }
    }
}