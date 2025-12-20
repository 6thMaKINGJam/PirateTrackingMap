using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보물, 몬스터 등 오브젝트를 들고 있는 매니저
/// </summary>
public class ObjectSetter : MonoBehaviour
{
    public GameObject treasurePref;
    public GameObject monsterPref;
    
    public GameObject treasure;
    public List<GameObject> monsters;
    
    private void InstantiateObjects()
    {
        // 보물, 몬스터 오브젝트 생성
        if (treasurePref != null && treasure == null)
        {
            treasure = Instantiate(treasurePref, gameObject.transform);
            treasure.SetActive(false);
        }

        if (monsterPref != null && monsters.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.monsterCount; i++)
            {
                GameObject mon = Instantiate(monsterPref, gameObject.transform);
                mon.SetActive(false);
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
        
        SetPosition(treasure, treasurePos);
        treasure.SetActive(true);

        for (int i = 0; i < monsterPos.Count; i++)
        {
            if (monsters[i] != null)
            {
                SetPosition(monsters[i], monsterPos[i]);
                monsters[i].SetActive(true);
            }
        }
    }

    private void SetPosition(GameObject go, Vector2Int pos)
    {
        int posIndexX = pos.x - 8;
        int posIndexY = pos.y - 8;
        
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(posIndexX * GameManager.CELL_SIZE, posIndexY * GameManager.CELL_SIZE);
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
