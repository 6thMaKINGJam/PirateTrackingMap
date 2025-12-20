using UnityEngine;

/// <summary>
/// 그리드의 각 칸을 나타내는 데이터 클래스
/// MonoBehaviour를 상속하지 않는 순수 데이터 클래스
/// </summary>
public class GridCell
{
    public bool IsVisited;      // 플레이어가 방문했는지 여부
    public bool HasObject;      // 이 칸에 오브젝트(장애물)가 있는지 여부
    public Vector2Int Position; // 그리드 상의 좌표 (0~15)

    /// <summary>
    /// GridCell 생성자
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    public GridCell(int x, int y)
    {
        Position = new Vector2Int(x, y);
        IsVisited = false;
        HasObject = false;
    }

    /// <summary>
    /// 이 칸을 방문했다고 표시
    /// </summary>
    public void MarkAsVisited()
    {
        IsVisited = true;
    }
}