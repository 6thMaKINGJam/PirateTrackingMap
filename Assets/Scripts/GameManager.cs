using System;using UnityEngine;
using System.Collections.Generic;using Random = UnityEngine.Random;

public enum GridState
{
    None,
    보물,
    몬스터,
    플레이어
}
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    // 그리드 설정
    public const int GRID_WIDTH = 16;
    public const int GRID_HEIGHT = 16;
    public const float CELL_SIZE = 50.0f;

    // 플레이어 체력
    public int playerHealth = 3;
    public Vector2Int playerPosition;

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
    private GridState[,] grid = new GridState[GRID_WIDTH, GRID_HEIGHT];

    public ObjectSetter objectSetter;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        //InitializeGame();
    }

    /// <summary>
    /// 게임 초기 설정
    /// 플레이어 체력, 보물/몬스터 위치 재배치
    /// </summary>
    public void InitializeGame()
    {
        // 플레이어 체력 초기화
        playerHealth = 3;
        playerPosition = new Vector2Int(5, 5);

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
        
        // 보물, 몬스터 오브젝트 위치 배치
        objectSetter.SpawnObjects(treasurePosition, monsterPositions);
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
            GameOver();
        }
    }

    /// <summary>
    /// 게임 오버
    /// </summary>
    private void GameOver()
    {
        Debug.Log("게임 오버");
        
        // 게임 오버 처리 로직
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        InitializeGame();
    }
}