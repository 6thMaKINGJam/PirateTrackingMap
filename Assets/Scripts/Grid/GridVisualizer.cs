using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        // 세로선
        for (int x = 0; x <= 16; x++)
        {
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, 16, 0));
        }

        // 가로선
        for (int y = 0; y <= 16; y++)
        {
            Gizmos.DrawLine(new Vector3(0, y, 0), new Vector3(16, y, 0));
        }
    }
}