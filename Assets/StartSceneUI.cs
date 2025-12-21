using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneUI : MonoBehaviour
{   
    public string SceneName;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(BGMName.메인);
        SoundManager.Instance.PlaySFX(SFXName.원숭이);
    }

    public void OnStartButtonClicked()
    {
        //SceneManager.LoadScene("CardSelectScene");
        SceneManager.LoadScene(SceneName);
    }
    
}
