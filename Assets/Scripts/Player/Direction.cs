/// <summary>
/// 절대 방향 (월드 기준)
/// 북쪽을 0으로 시작해서 시계방향으로 증가
/// </summary>
public enum Direction
{
    Up = 0,    // 북쪽
    Right = 1, // 동쪽
    Down = 2,  // 남쪽
    Left = 3   // 서쪽
}

/// <summary>
/// 상대 방향 (플레이어 기준)
/// 카드의 이동 명령에 사용됨
/// </summary>
public enum RelativeDirection
{
    Forward, // 플레이어가 보는 방향 (앞)
    Back,    // 플레이어 뒤쪽
    Left,    // 플레이어 왼쪽
    Right    // 플레이어 오른쪽
}