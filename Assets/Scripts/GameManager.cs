using System;using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum GridState
{
    None,
    보물,
    몬스터,
    플레이어
}
public enum GameOverType
{
    ByMonster,
    ByTurnLimit
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    // 그리드 설정
    public const int GRID_WIDTH = 16;
    public const int GRID_HEIGHT = 16;

    // 플레이어 체력
    public int playerHealth = 3;
    public Vector2Int playerPosition;

    // 게임 오버 타입
    public GameOverType LastGameOverType { get; private set; } = GameOverType.ByTurnLimit;

    /// <summary>
    /// 보물 위치 (grid 기준) (ex. {3, 10}
    /// </summary>
    public Vector2Int treasurePosition;

    // 몬스터
    public int monsterCount;
    /// <summary>
    /// 몬스터 n마리 위치 리스트 (gird 기준) (ex. [{1, 2}, {4, 8}, {10, 11}]
    /// </summary>
    public List<Vector2Int> monsterPositions = new List<Vector2Int>();

    // 그리드 상태 
    public GridState[,] grid = new GridState[GRID_WIDTH, GRID_HEIGHT];

    public ObjectSetter objectSetter;
    private DirectionSelector _directionSelector;
    public CardSetupUI cardSetupUI;

    // ===== 이동 시스템 연결 =====
    [Header("Movement System")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] public PlayerController playerController;
    private NumberDisplay _numberDisplay;
    
    // ===== 카드 리스트 =====
    private List<MoveCard> availableCards;
    
    // 턴 수 
    public int CountTurn = 10;
    
    // 엔딩 씬 이름
    public string successEndingName;
    public string monsterEndingName;
    public string noTreasureEndingName;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // NumberDisplay 자동 찾기
        _numberDisplay = gridManager?.GetComponent<NumberDisplay>();
        
        // DirectionSelector 자동 찾기
        _directionSelector = FindObjectOfType<DirectionSelector>();
        if (_directionSelector == null)
        {
            Debug.LogWarning("DirectionSelector를 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        // 카드 생성
        InitializeCards();

        // 게임 초기화
        InitializeGame();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cardSetupUI = FindObjectOfType<CardSetupUI>();
        /*playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        objectSetter = FindObjectOfType<ObjectSetter>();*/
        gridManager = FindObjectOfType<GridManager>();
    }
    
    /// <summary>
    /// 이동 카드 초기화
    /// </summary>
    private void InitializeCards()
    {
        availableCards = new List<MoveCard>
        {
            new CardRunningMan(),
            new CardUTurn(),
            new CardCrab(),
            new CardAnchor()
        };
    }

    /// <summary>
    /// 게임 초기 설정
    /// 플레이어 체력, 보물/몬스터 위치 재배치
    /// </summary>
    public void InitializeGame()
    {
        // 플레이어 체력 초기화
        playerHealth = 0;
        playerPosition = new Vector2Int(0, 15);
        
        // 그리드 초기화
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                grid[x, y] = GridState.None;
            }
        }

        // 사용된 위치를 추적하기 위한 리스트
        List<Vector2Int> usedPositions = new List<Vector2Int>();

        // 보물 위치 랜덤 생성
        treasurePosition = GetRandomPosition(usedPositions);
        usedPositions.Add(treasurePosition);
        grid[treasurePosition.x, treasurePosition.y] = GridState.보물;

        // 몬스터 3개 위치 랜덤 생성
        monsterPositions.Clear();
        for (int i = 0; i < monsterCount; i++)
        {
            Vector2Int monsterPos = GetRandomPosition(usedPositions);
            monsterPositions.Add(monsterPos);
            usedPositions.Add(monsterPos);
            grid[monsterPos.x, monsterPos.y] = GridState.몬스터;
        }
        
        // 플레이어, 보물, 몬스터 오브젝트 위치 배치
        objectSetter.SpawnObjects(treasurePosition, monsterPositions);
        
        // ===== 이동 시스템 초기화 =====
        InitializeMovementSystem();
        
        // 턴 수 초기화
        CountTurn = 10;
    }
    
    /// <summary>
    /// 이동 시스템 초기화
    /// GridManager, PlayerController, NumberDisplay 연결
    /// </summary>
    private void InitializeMovementSystem()
    {
        if (gridManager == null || playerController == null)
        {
            Debug.LogError("GridManager 또는 PlayerController가 할당되지 않았습니다!");
            return;
        }

        // 1. GridManager 초기화
        gridManager.Initialize();

        // 2. 보물과 몬스터 위치를 GridManager에 오브젝트로 등록
        gridManager.SetObject(treasurePosition.x, treasurePosition.y);
        foreach (Vector2Int monsterPos in monsterPositions)
        {
            gridManager.SetObject(monsterPos.x, monsterPos.y);
        }

        // 3. NumberDisplay 초기화
        if (_numberDisplay != null)
        {
            _numberDisplay.Initialize(gridManager);
        }
        else
        {
            Debug.LogWarning("NumberDisplay를 찾을 수 없습니다!");
        }

        // 4. 이벤트 연결
        playerController.OnCellVisited += OnPlayerCellVisited;
        playerController.OnMovementComplete += OnPlayerMovementComplete;

        // 5. 플레이어 시작 위치 설정
        playerController.SetStartPosition(playerPosition);

        // 6. 초기 숫자 표시
        if (_numberDisplay != null)
        {
            _numberDisplay.UpdateNumbers();
        }
    }
    
    /// <summary>
    /// 플레이어가 칸을 밟았을 때 호출
    /// </summary>
    /// <param name="pos">밟은 칸의 위치</param>
    private void OnPlayerCellVisited(Vector2Int pos)
    {
        // GridManager에 방문 기록
        gridManager.MarkCellVisited(pos);

        // 플레이어 위치 업데이트
        playerPosition = pos;

        /*// 해당 위치의 상태 확인
        CheckCellState(pos);*/
    }

    /// <summary>
    /// 플레이어 이동이 완료되었을 때 호출
    /// </summary>
    private void OnPlayerMovementComplete()
    {
        // 숫자 업데이트
        if (_numberDisplay != null)
        {
            _numberDisplay.UpdateNumbers();
        }
    }

    /// <summary>
    /// 특정 칸의 상태 확인 및 처리
    /// </summary>
    /// <param name="pos">확인할 칸의 위치</param>
    private void CheckCellState(Vector2Int pos)
    {
        GridState state = grid[pos.x, pos.y];

        switch (state)
        {
            case GridState.보물:
                Debug.Log("보물 발견! 게임 클리어!");
                // TODO: 게임 클리어 처리
                break;

            case GridState.몬스터:
                Debug.Log("몬스터 발견! 데미지!");
                TakeDamage(1);
                // 몬스터는 한 번만 데미지 주도록 상태 변경
                grid[pos.x, pos.y] = GridState.None;
                break;

            case GridState.None:
                // 아무것도 없음
                break;
        }
    }

    /// <summary>
    /// 랜덤 위치 1개 생성
    /// </summary>
    /// <param name="usedPositions"></param>
    /// <returns>위치 Vector</returns>
    private Vector2Int GetRandomPosition(List<Vector2Int> usedPositions)
    {
        Vector2Int newPosition;
        int attempts = 0;
        int maxAttempts = 1000;

        do
        {
            int x = Random.Range(0, GRID_WIDTH);
            int y = Random.Range(0, GRID_HEIGHT);
            newPosition = new Vector2Int(x, y);
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogError("랜덤 위치 생성 실패!");
                break;
            }
        }
        while (usedPositions.Contains(newPosition));

        return newPosition;
    }

    /// <summary>
    /// 플레이어 데미지 처리
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log($"체력: {playerHealth}");

        if (playerHealth <= 0)
        {
            GameOver(GameOverType.ByMonster);
        }
    }

    /// <summary>
    /// 게임 오버
    /// </summary>
    public void GameOver()
    {
        // 게임 오버 처리 로직
        GameOver(GameOverType.ByTurnLimit);
        
        // 카드 수 초기화
        cardSetupUI.ResetCardSelection();
    }

    public void GameOver(GameOverType type)
    {
        LastGameOverType = type;
        Debug.Log("게임 오버: " + type);

        // 카드 수 초기화
        cardSetupUI.ResetCardSelection();
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
    }

    /// <summary>
    /// 게임 성공
    /// </summary>
    public void GameSuccess()
    {
        // 카드 수 초기화
        cardSetupUI.ResetCardSelection();
        
        SceneManager.LoadScene("GameWinScene");
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        InitializeGame();
        NewTurn();
    }

    /// <summary>
    /// 새로운 턴 시작
    /// </summary>
    public void NewTurn()
    {
        CountTurn--;
        cardSetupUI.ItemSelectUI.SetActive(true);
    }
    
    
    // 방향선택구현완료시 
    // ===== 테스트용 UI 버튼 카드 사용 함수들 =====

    /*/// <summary>
    /// 러닝맨 카드 사용 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardRunningMan()
    {
        Debug.Log("러닝맨 카드 사용!");
        playerController.UseCard(new CardRunningMan());
    }

    /// <summary>
    /// 유턴 카드 사용 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardUTurn()
    {
        Debug.Log("유턴 카드 사용!");
        playerController.UseCard(new CardUTurn());
    }

    /// <summary>
    /// 바닷게 카드 사용 (UI 버튼에서 호출)
    /// 방향 선택 UI 표시
    /// </summary>
    public void UseCardCrab()
    {
        if (_directionSelector == null)
        {
            Debug.LogError("DirectionSelector가 없습니다!");
            return;
        }
    
        Debug.Log("바닷게 카드 선택 - 왼쪽/오른쪽을 선택하세요");
    
        _directionSelector.ShowLeftRightChoice((isLeft) => 
        {
            Debug.Log($"바닷게 카드 사용 ({(isLeft ? "왼쪽" : "오른쪽")})");
            CardCrab card = new CardCrab();
            card.SetChoice(isLeft);
            playerController.UseCard(card);
        });
    }

    /// <summary>
    /// 앵커 카드 사용 (UI 버튼에서 호출)
    /// 방향 선택 UI 표시
    /// </summary>
    public void UseCardAnchor()
    {
        if (_directionSelector == null)
        {
            Debug.LogError("DirectionSelector가 없습니다!");
            return;
        }

        Debug.Log("앵커 카드 선택 - 방향을 선택하세요");

        _directionSelector.ShowDirectionChoice((direction) =>
        {
            Debug.Log($"앵커 카드 사용 ({direction})");
            CardAnchor card = new CardAnchor();
            card.SetChoice(direction);
            playerController.UseCard(card);
        });
    }*/
    
    
    // ===== 테스트용 UI 버튼 카드 사용 함수들 =====

    private bool CanUseCard(int index)
    {
        if (Instance.cardSetupUI.SelectCounts[index] > 0)
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// 러닝맨 카드 사용 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardRunningMan()
    {
        if (CanUseCard(0))
        {
            Debug.Log("러닝맨 카드 사용!");
            Instance.playerController.UseCard(new CardRunningMan());
            Instance.cardSetupUI.SelectCounts[0]--;
        }
    }

    /// <summary>
    /// 유턴 카드 사용 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardUTurn()
    {
        if (CanUseCard(1))
        {
            Debug.Log("유턴 카드 사용!");
            Instance.playerController.UseCard(new CardUTurn());
            Instance.cardSetupUI.SelectCounts[1]--;      
        }
    }

    /// <summary>
    /// 바닷게 카드 사용 - 왼쪽 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardCrabLeft()
    {
        if (CanUseCard(2))
        {
            Debug.Log("바닷게 카드 사용 (왼쪽)");
            CardCrab card = new CardCrab();
            card.SetChoice(true); // 왼쪽
            Instance.playerController.UseCard(card);
            Instance.cardSetupUI.SelectCounts[2]--;
        }
    }

    /// <summary>
    /// 바닷게 카드 사용 - 오른쪽 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardCrabRight()
    {
        Debug.Log("바닷게 카드 사용 (오른쪽)");
        CardCrab card = new CardCrab();
        card.SetChoice(false); // 오른쪽
        Instance.playerController.UseCard(card);
        Instance.cardSetupUI.SelectCounts[2]--;
    }

    /// <summary>
    /// 앵커 카드 사용 - 위 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardAnchorUp()
    {
        Debug.Log("앵커 카드 사용 (위)");
        CardAnchor card = new CardAnchor();
        card.SetChoice(Direction.Up);
        Instance.playerController.UseCard(card);
        Instance.cardSetupUI.SelectCounts[3]--;
    }

    /// <summary>
    /// 앵커 카드 사용 - 오른쪽 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardAnchorRight()
    {
        if (CanUseCard(3))
        {
            Debug.Log("앵커 카드 사용 (오른쪽)");
            CardAnchor card = new CardAnchor();
            card.SetChoice(Direction.Right);
            Instance.playerController.UseCard(card);
            Instance.cardSetupUI.SelectCounts[3]--;
        }
    }

    /// <summary>
    /// 앵커 카드 사용 - 아래 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardAnchorDown()
    {
        Debug.Log("앵커 카드 사용 (아래)");
        CardAnchor card = new CardAnchor();
        card.SetChoice(Direction.Down);
        Instance.playerController.UseCard(card);
        Instance.cardSetupUI.SelectCounts[3]--;
    }

    /// <summary>
    /// 앵커 카드 사용 - 왼쪽 (UI 버튼에서 호출)
    /// </summary>
    public void UseCardAnchorLeft()
    {
        Debug.Log("앵커 카드 사용 (왼쪽)");
        CardAnchor card = new CardAnchor();
        card.SetChoice(Direction.Left);
        Instance.playerController.UseCard(card);
        Instance.cardSetupUI.SelectCounts[3]--;
    }
}