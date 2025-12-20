using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 이동을 담당하는 클래스
/// 카드 명령을 받아서 실제로 움직이고, 이동 관련 이벤트를 발생시킴
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ===== 이벤트 =====
    /// <summary>
    /// 플레이어가 한 칸을 밟을 때마다 발생하는 이벤트
    /// GridManager가 이 이벤트를 구독해서 방문 기록을 저장함
    /// </summary>
    public event System.Action<Vector2Int> OnCellVisited;

    /// <summary>
    /// 모든 이동이 완료되었을 때 발생하는 이벤트
    /// NumberDisplay가 이 이벤트를 구독해서 숫자를 업데이트함
    /// </summary>
    public event System.Action OnMovementComplete;

    // ===== 필드 =====
    private Vector2Int _currentGridPosition;    // 현재 그리드 좌표
    private Direction _facingDirection = Direction.Up; // 현재 바라보는 방향

    [SerializeField] private float moveSpeed = 5f; // 이동 속도 (칸/초)

    private Queue<MoveCommand> _movementQueue = new Queue<MoveCommand>(); // 이동 명령 큐
    private bool _isMoving = false; // 현재 이동 중인지 여부

    // ===== 공개 메서드 =====

    /// <summary>
    /// 플레이어의 시작 위치 설정
    /// 외부 GameManager에서 플레이어가 시작 칸을 선택하면 호출됨
    /// </summary>
    /// <param name="startPos">시작 위치 좌표</param>
    public void SetStartPosition(Vector2Int startPos)
    {
        _currentGridPosition = startPos;
        transform.position = GridToWorldPosition(startPos);

        // 시작 위치도 방문으로 처리
        OnCellVisited?.Invoke(startPos);
    }

    /// <summary>
    /// 카드를 사용해서 플레이어 이동 시작
    /// </summary>
    /// <param name="card">사용할 카드</param>
    public void UseCard(MoveCard card)
    {
        // 이미 이동 중이면 무시
        if (_isMoving)
        {
            Debug.Log("Already moving!");
            return;
        }

        // 카드로부터 이동 명령 리스트 가져오기
        List<MoveCommand> commands = card.GetMovementSequence();

        // 큐에 추가
        foreach (var cmd in commands)
        {
            _movementQueue.Enqueue(cmd);
        }

        // 최종 방향 미리 계산
        Direction finalDir = card.GetFinalDirection(_facingDirection);

        // 이동 코루틴 시작
        StartCoroutine(ExecuteMovementSequence(finalDir));
    }

    // ===== 내부 메서드 =====

    /// <summary>
    /// 이동 명령 큐를 순차적으로 실행하는 코루틴
    /// </summary>
    /// <param name="finalDirection">모든 이동 후 최종 방향</param>
    private IEnumerator ExecuteMovementSequence(Direction finalDirection)
    {
        _isMoving = true;

        // 큐가 빌 때까지 반복
        while (_movementQueue.Count > 0)
        {
            // 다음 명령 꺼내기
            MoveCommand cmd = _movementQueue.Dequeue();

            // 상대 방향 → 절대 방향 변환
            Direction absoluteDir = cmd.GetAbsoluteDirection(_facingDirection);

            // 다음 칸 좌표 계산
            Vector2Int nextPos = GetNextGridPosition(_currentGridPosition, absoluteDir);

            // 범위 체크
            if (!IsInBounds(nextPos))
            {
                Debug.Log("Out of bounds!");
                continue; // 이동 스킵
            }

            // 이동 애니메이션 실행 (끝날 때까지 대기)
            yield return StartCoroutine(MoveToPosition(nextPos));

            // 현재 위치 갱신
            _currentGridPosition = nextPos;

            // 방문 이벤트 발생 (GridManager가 받음)
            OnCellVisited?.Invoke(nextPos);
        }

        // 모든 이동 완료 → 최종 방향으로 회전
        _facingDirection = finalDirection;
        RotatePlayerSprite(finalDirection);

        _isMoving = false;

        // 완료 이벤트 발생 (NumberDisplay가 받음)
        OnMovementComplete?.Invoke();
    }

    /// <summary>
    /// 현재 위치에서 특정 방향으로 한 칸 이동했을 때의 다음 좌표 계산
    /// </summary>
    /// <param name="current">현재 좌표</param>
    /// <param name="dir">이동 방향</param>
    /// <returns>다음 좌표</returns>
    private Vector2Int GetNextGridPosition(Vector2Int current, Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return current + Vector2Int.up;    // (x, y+1)
            case Direction.Down:
                return current + Vector2Int.down;  // (x, y-1)
            case Direction.Left:
                return current + Vector2Int.left;  // (x-1, y)
            case Direction.Right:
                return current + Vector2Int.right; // (x+1, y)
            default:
                return current;
        }
    }

    /// <summary>
    /// 좌표가 그리드 범위 내에 있는지 확인
    /// </summary>
    /// <param name="pos">확인할 좌표</param>
    /// <returns>범위 내면 true</returns>
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 16 && pos.y >= 0 && pos.y < 16;
    }

    /// <summary>
    /// 목표 위치까지 부드럽게 이동하는 애니메이션 코루틴
    /// </summary>
    /// <param name="targetGridPos">목표 그리드 좌표</param>
    private IEnumerator MoveToPosition(Vector2Int targetGridPos)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = GridToWorldPosition(targetGridPos);

        float elapsed = 0f;
        float duration = 1f / moveSpeed; // 예: 5칸/초 → 0.2초/칸

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 선형 보간으로 부드럽게 이동
            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 정확한 위치로 스냅
        transform.position = endPos;
    }

    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환
    /// </summary>
    /// <param name="gridPos">그리드 좌표</param>
    /// <returns>월드 좌표</returns>
    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        // 1칸 = 1 Unity unit으로 가정
        return new Vector3(gridPos.x, gridPos.y, 0);
    }

    /// <summary>
    /// 플레이어 스프라이트를 특정 방향으로 회전
    /// </summary>
    /// <param name="dir">회전할 방향</param>
    private void RotatePlayerSprite(Direction dir)
    {
        float angle = 0f;

        switch (dir)
        {
            case Direction.Up:    angle = 0f; break;
            case Direction.Right: angle = -90f; break;
            case Direction.Down:  angle = 180f; break;
            case Direction.Left:  angle = 90f; break;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}