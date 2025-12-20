using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("NextMessage")]
    public TMP_Text messageText;   // 화면에 보여줄 텍스트
    
    [TextArea(3, 10)]
    public string[] messages;  // 문장 배열

    private int index = 0;
    public string sceneChangeTo;


    [Header("MessageEffect")]
    public float fadeSpeed = 1f;   // 페이드 진행 속도

    TMP_Text tmpText;
    Mesh mesh;
    Color32[] colors;

    private float fadeT = 0f;
    public float maxfadeTime = 6f;

    public int endMessageStartIndex = 4;

    int fadeStartChar = 0;
    int fadeEndChar = 0;

    public bool isGameFinished;

    int currentStartIndex = 0;




    // Start is called before the first frame update
    void Start()
    {
        // 시작할 때 첫 문장 표시
        if (messages.Length > 0)
        {
            //messageText.text = messages[index];
            messageText.text = messages[0];
            tmpText.ForceMeshUpdate();

            StartCutscene();

        }
    }

    public void ClickForCutscene()
    {
       

        StartCutscene();
    }

    public void NextMessage()
    {
        index++;
        fadeT = 0f;

       

        if (index < messages.Length)
        {
            messageText.text += "\n" + messages[index];
        }
        // 다 봤으면 씬 이동
        else
        {
            SceneManager.LoadScene(sceneChangeTo);
        }

        fadeStartChar = tmpText.textInfo.characterCount;


        tmpText.ForceMeshUpdate();

        fadeEndChar = tmpText.textInfo.characterCount;
    }
    ///다음 메세지 구현

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }
    void OnEnable()
    {
        fadeT = 0f;
        tmpText.ForceMeshUpdate();
    }

    void Update()
    {
        if(fadeT<maxfadeTime)
        FadeUp();

       
        
    }

    void StartCutscene()
    {
        index = currentStartIndex;

        messageText.text = messages[index];
        tmpText.ForceMeshUpdate();

        fadeT = 0f;
        fadeStartChar = 0;
        fadeEndChar = tmpText.textInfo.characterCount;
    }

    void FadeUp()
    {
        tmpText.ForceMeshUpdate();
        var textInfo = tmpText.textInfo;

        if (textInfo.characterCount == 0) return;

        // 텍스트 전체 높이 범위
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = fadeStartChar; i < fadeEndChar; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            int vIndex = charInfo.vertexIndex;

            for (int v = 0; v < 4; v++)
            {
                float y = verts[vIndex + v].y;
                minY = Mathf.Min(minY, y);
                maxY = Mathf.Max(maxY, y);
            }
        }

        float height = maxY - minY;

        // 각 vertex 알파 조절
        for (int i = fadeStartChar; i < fadeEndChar; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vIndex = charInfo.vertexIndex;

            var colors = textInfo.meshInfo[matIndex].colors32;
            var verts = textInfo.meshInfo[matIndex].vertices;


            for (int v = 0; v <4; v++)
            {
                float y = verts[vIndex + v].y;
                float normalizedY = Mathf.InverseLerp(minY, maxY, y);

                float alpha = Mathf.Clamp01(fadeT - (1f - normalizedY));
                colors[vIndex + v].a = (byte)(alpha * 255);
            }
        }

        fadeT += Time.deltaTime * fadeSpeed;
        

        // 적용
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }






}
