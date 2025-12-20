using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    private int health = 1;

    private Animator _animator;
    //public Text resultText;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 몬스터 만났을 때
    /// 외부 호출
    /// </summary>
    public void FaceMonster()
    {
        StartCoroutine("AttackPlayer");
    }
    
    private IEnumerator AttackPlayer()
    {
        
        if (GameManager.Instance.playerHealth >= health)
        {
            // 플레이어 공격 성공 -> 몬스터 죽음
            //resultText.text = "공격 성공!";
            Debug.Log("공격 성공");
            GameManager.Instance.playerHealth -= health;
            //StartCoroutine("ShowText");
            gameObject.SetActive(false);
            
            yield return new WaitForSeconds(1.0f);
            Dead();
        }
        else
        {
            // 플레이어 공격 실패 -> 게임 오버
            //resultText.text = "공격 실패 ..";
            //StartCoroutine("ShowText");
            Debug.Log("공격 실패");
            yield return new WaitForSeconds(1.0f);
            GameManager.Instance.GameOver();
        }
    }

    private void Dead()
    {
        _animator.SetBool("Dead", true);
    }
    
    private IEnumerator ShowText()
    {
        //resultText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);
        
        //resultText.gameObject.SetActive(false);
    }
}
