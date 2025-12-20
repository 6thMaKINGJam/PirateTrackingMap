using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 16;
    [SerializeField] private int gridHeight = 16;
    [SerializeField] private float cellSize = 1f;

    [Header("Gizmo Settings")]
    [SerializeField] private Color gridColor = Color.green;

    // 이 조건문 제거하고 항상 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        // 세로 선 그리기
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = transform.position + new Vector3(x * cellSize, 0, 0);
            Vector3 end = transform.position + new Vector3(x * cellSize, gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // 가로 선 그리기
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = transform.position + new Vector3(0, y * cellSize, 0);
            Vector3 end = transform.position + new Vector3(gridWidth * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}