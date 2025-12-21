using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Telescope : MonoBehaviour
{
    private Vector2 _playerPos;
    private Vector2 _treasurePos;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        UseTelescope();
    }

    public void UseTelescope()
    {
        _playerPos = GameManager.Instance.playerPosition;
        _treasurePos = GameManager.Instance.treasurePosition;
        
        Vector2 direction = new Vector2(_treasurePos.x - _playerPos.x, _playerPos.y - _treasurePos.y).normalized;
        //Vector2 direction = (_treasurePos - _playerPos).normalized;
        
        // 위치 설정
        Vector2 arrowPosition = _playerPos + direction * 150.0f;
        gameObject.GetComponent<RectTransform>().anchoredPosition = arrowPosition;
        
        // 회전 설정
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        SoundManager.Instance.PlaySFX(SFXName.방향카드화살표);
    }
    

}
