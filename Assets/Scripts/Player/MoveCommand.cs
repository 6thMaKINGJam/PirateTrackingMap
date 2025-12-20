/// <summary>
/// 하나의 이동 명령을 나타내는 클래스
/// 상대 방향을 절대 방향으로 변환하는 역할
/// </summary>
public class MoveCommand
{
    private readonly RelativeDirection _direction; // 상대 방향 (앞, 뒤, 좌, 우)

    /// <summary>
    /// MovementCommand 생성자
    /// </summary>
    /// <param name="dir">상대 방향</param>
    public MoveCommand(RelativeDirection dir)
    {
        _direction = dir;
    }

    /// <summary>
    /// 플레이어가 현재 바라보는 방향을 고려해서 절대 방향으로 변환
    /// 예: 플레이어가 동쪽 보는 중 + "왼쪽" 명령 → 북쪽 반환
    /// </summary>
    /// <param name="currentFacing">현재 플레이어가 바라보는 방향</param>
    /// <returns>실제 이동할 절대 방향</returns>
    public Direction GetAbsoluteDirection(Direction currentFacing)
    {
        int facing = (int)currentFacing; // 0~3

        switch (_direction)
        {
            case RelativeDirection.Forward:
                return currentFacing; // 그대로
            case RelativeDirection.Back:
                return (Direction)((facing + 2) % 4); // 180도 회전
            case RelativeDirection.Left:
                return (Direction)((facing + 3) % 4); // 반시계 90도
            case RelativeDirection.Right:
                return (Direction)((facing + 1) % 4); // 시계 90도
            default:
                return currentFacing;
        }
    }
}