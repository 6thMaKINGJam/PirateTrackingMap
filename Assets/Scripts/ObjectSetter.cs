using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 보물, 몬스터 등 오브젝트를 들고 있는 매니저
/// </summary>
public class ObjectSetter : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    
    public GameObject treasurePref;
    public GameObject monsterPref;
    public GameObject playerPref;
    
    public GameObject treasure;
    public List<GameObject> monsters;
    public GameObject player;
    
    private RectTransform canvasRT;

    private void Awake()
    {
        GameManager.Instance.objectSetter = this;
    }

    private void Start()
    {
        canvasRT = GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();
    }

    private void InstantiateObjects()
    {
        // 보물, 몬스터 오브젝트 생성
        if (treasurePref != null && treasure == null)
        {
            treasure = Instantiate(treasurePref, gameObject.transform);
        }

        if (monsterPref != null && monsters.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.monsterCount; i++)
            {
                GameObject mon = Instantiate(monsterPref, gameObject.transform);
                monsters.Add(mon);
            }  
        }

    }
    
    /// <summary>
    /// 보물, 몬스터 랜덤 배치
    /// </summary>
    public void SpawnObjects(Vector2Int treasurePos, List<Vector2Int> monsterPos)
    {
        if (treasure == null || monsters.Count == 0)
        {
            InstantiateObjects();
        }
        
        SetPosition(player, GameManager.Instance.playerPosition);
        player.SetActive(true);
        
        SetPosition(treasure, treasurePos);
        treasure.SetActive(false);

        for (int i = 0; i < monsterPos.Count; i++)
        {
            if (monsters[i] != null)
            {
                SetPosition(monsters[i], monsterPos[i]);
                monsters[i].SetActive(false);
            }
        }
    }
    
    private void SetPosition(GameObject go, Vector2Int pos)
    {
        int index = pos.y * 16 + pos.x;
        RectTransform gridCell = gridLayoutGroup.transform.GetChild(index) as RectTransform;
            
        go.GetComponent<RectTransform>().position = gridCell.position;
    }

    /// <summary>
    /// 보물 위치 반환 (canvas 기준)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetTreasurePosition()
    {
        return treasure.GetComponent<RectTransform>().anchoredPosition;
    }

    /// <summary>
    /// i번 몬스터 위치 반환 (canvas 기준)
    /// </summary>
    /// <param name="index"></param>
    /// <returns>몬스터 번호</returns>
    public Vector2 GetMonsterPosition(int index)
    {
        return monsters[index].GetComponent<RectTransform>().anchoredPosition;
    }
}
