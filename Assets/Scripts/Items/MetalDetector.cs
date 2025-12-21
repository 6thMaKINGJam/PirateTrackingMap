using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalDetector : MonoBehaviour
{
    [Header("Scan Settings")]
    [SerializeField] private float scanSpeed = 180f; // 초당 회전 각도 (180도 = 2초에 1바퀴)
    public Image scanBeamImg;
    private Image scanBeam;
    private float currentAngle = 90f; // 시작 각도 (위쪽)
    
    public Text resultText;
    private GameObject player;
    
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        
    }
    
    void Start()
    {
        resultText.gameObject.SetActive(false);
        
        scanBeam = Instantiate(scanBeamImg, player.transform);
        scanBeam.gameObject.SetActive(false);
    }

    public void UseMetalDetector()
    {
        // 범위 탐색
        StartCoroutine("StartScan");
    }

    private IEnumerator StartScan()
    {
        InitScanBeam();

        while (currentAngle >= 90f - 360f)
        {
            UpdateScanBeam();
            yield return null;
        }
        
        // 보물 있는지 확인
        if (CheckDetected())
        {
            // 있다면
            SoundManager.Instance.PlaySFX(SFXName.금속탐지성공);
            resultText.text = "탐지되었습니다!";
        }
        else
        {
            // 없다면
            SoundManager.Instance.PlaySFX(SFXName.금속탐지실패);
            resultText.text = "탐지되지않았습니다.";
        }
        
        StopScan();
        StartCoroutine("ShowText");
    }

    private void InitScanBeam()
    {
        currentAngle = 90f;
        scanBeam.gameObject.SetActive(true);
        UpdateScanBeam();
    }
    private void UpdateScanBeam()
    {
        if (scanBeam == null) return;
        
        currentAngle -= scanSpeed * Time.deltaTime;
        
        // Z축 회전으로 빔 방향 변경
        scanBeam.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }
    
    public void StopScan()
    {
        if (scanBeam != null)
        {
            scanBeam.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 플레이어 2칸 범위 안에 보물이 있는지 확인
    /// </summary>
    /// <returns>true: 보물 있음 false: 보물 없음</returns>
    private bool CheckDetected()
    {
        Vector2Int playerPos = GameManager.Instance.playerPosition;
        Vector2Int treasurePos = GameManager.Instance.treasurePosition;

        int disX = Math.Abs(playerPos.x - treasurePos.x);
        int disY = Math.Abs(playerPos.y - treasurePos.y);

        if (disX <= 2 && disY <= 2)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private IEnumerator ShowText()
    {
        resultText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);
        
        resultText.gameObject.SetActive(false);
    }
}
