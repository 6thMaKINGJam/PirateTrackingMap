using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSceneUI : MonoBehaviour
{
    public GameObject turnOverImage;
    public GameObject monsterOverImage;

    // 시작 화면 씬 이름으로 바꿔서 쓰기
    public string startSceneName = "StartScene";

    void Start()
    {
        // 기본 off
        turnOverImage.SetActive(false);
        monsterOverImage.SetActive(false);

        var gm = GameManager.Instance;
        if (gm == null)
        {
            // 혹시 GameManager가 없으면 안전하게 기본(턴 초과)로
            turnOverImage.SetActive(true);
            return;
        }

        if (gm.LastGameOverType == GameOverType.ByMonster)
            monsterOverImage.SetActive(true);
        else
            turnOverImage.SetActive(true);
    }

    public void RestartToStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }
}

