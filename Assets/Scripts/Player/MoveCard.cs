using System.Collections.Generic;

/// <summary>
/// 모든 카드의 기본이 되는 추상 클래스
/// 각 카드는 이 클래스를 상속받아 구현
/// </summary>
public abstract class MoveCard
{
    public string CardName;

    /// <summary>
    /// 이 카드를 사용했을 때의 이동 명령 시퀀스 반환
    /// </summary>
    public abstract List<MoveCommand> GetMovementSequence();

    /// <summary>
    /// 이 카드를 사용한 후 플레이어가 최종적으로 바라볼 방향 계산
    /// </summary>
    /// <param name="currentDir">현재 바라보는 방향</param>
    /// <returns>최종 방향</returns>
    public abstract Direction GetFinalDirection(Direction currentDir);
}

// ===== 카드 1: 러닝맨 (앞으로 3칸) =====
/// <summary>
/// 러닝맨 카드: 앞으로 3칸 직진
/// </summary>
public class CardRunningMan : MoveCard
{
    public CardRunningMan()
    {
        CardName = "러닝맨";
    }

    public override List<MoveCommand> GetMovementSequence()
    {
        return new List<MoveCommand>
        {
            new MoveCommand(RelativeDirection.Forward),
            new MoveCommand(RelativeDirection.Forward),
            new MoveCommand(RelativeDirection.Forward)
        };
    }

    public override Direction GetFinalDirection(Direction currentDir)
    {
        // 마지막 이동이 Forward이므로 방향 변화 없음
        return currentDir;
    }
}

// ===== 카드 2: 유턴 (앞2칸 → 우2칸 → 뒤3칸) =====
/// <summary>
/// 유턴 카드: 앞 2칸, 오른쪽 2칸, 뒤 3칸 (U자 경로)
/// 최종 방향: 마지막 이동 방향 (뒤) = 시작의 반대편
/// </summary>
public class CardUTurn : MoveCard
{
    public CardUTurn()
    {
        CardName = "유턴";
    }

    public override List<MoveCommand> GetMovementSequence()
    {
        return new List<MoveCommand>
        {
            new MoveCommand(RelativeDirection.Forward),
            new MoveCommand(RelativeDirection.Forward),
            new MoveCommand(RelativeDirection.Right),
            new MoveCommand(RelativeDirection.Right),
            new MoveCommand(RelativeDirection.Back),
            new MoveCommand(RelativeDirection.Back),
            new MoveCommand(RelativeDirection.Back)
        };
    }

    public override Direction GetFinalDirection(Direction currentDir)
    {
        // 마지막 이동 명령의 방향으로 회전
        var movements = GetMovementSequence();
        var lastMove = movements[movements.Count - 1];
        return lastMove.GetAbsoluteDirection(currentDir);
    }
}

// ===== 카드 3: 바닷게 (좌좌좌 or 우우우) =====
/// <summary>
/// 바닷게 카드: 왼쪽 3칸 또는 오른쪽 3칸 (플레이어 선택)
/// 최종 방향: 변화x
/// </summary>
public class CardCrab : MoveCard
{
    private bool _turnLeft = true;

    public CardCrab()
    {
        CardName = "바닷게";
    }

    /// <summary>
    /// 이동 방향 선택 (왼쪽 or 오른쪽)
    /// </summary>
    /// <param name="isLeft">true면 왼쪽, false면 오른쪽</param>
    public void SetChoice(bool isLeft)
    {
        _turnLeft = isLeft;
    }

    public override List<MoveCommand> GetMovementSequence()
    {
        RelativeDirection dir = _turnLeft ? 
            RelativeDirection.Left : RelativeDirection.Right;

        return new List<MoveCommand>
        {
            new MoveCommand(dir),
            new MoveCommand(dir),
            new MoveCommand(dir)
        };
    }

    public override Direction GetFinalDirection(Direction currentDir)
    {
        return currentDir;
    }
}

// ===== 카드 4: 앵커 (방향만 전환, 이동 없음) =====
/// <summary>
/// 앵커 카드: 제자리에서 원하는 방향으로 회전만 함
/// </summary>
public class CardAnchor : MoveCard
{
    private Direction _chosenDirection = Direction.Up;

    public CardAnchor()
    {
        CardName = "앵커";
    }

    /// <summary>
    /// 회전할 방향 선택
    /// </summary>
    /// <param name="newDir">입력할 방향 (절대방향)</param>
    public void SetChoice(Direction newDir)
    {
        _chosenDirection = newDir;
    }

    public override List<MoveCommand> GetMovementSequence()
    {
        return new List<MoveCommand>(); // 이동 없음
    }

    public override Direction GetFinalDirection(Direction currentDir)
    {
        return _chosenDirection; // 선택한 방향으로 변경
    }
}