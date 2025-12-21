using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathSpot : MonoBehaviour
{
    public Transform path;
    public GameObject spot;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    /// <summary>
    /// 플레이어 가는 길에 점선 생성
    /// </summary>
    /// <param name="pos"></param>
    public void MakePath(Vector3 pos)
    {
        GameObject go = Instantiate(spot, path);
        go.GetComponent<RectTransform>().position = pos;
    }
}
