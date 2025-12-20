using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardChoiceUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelCardList;
    public GameObject panelConfirm;
    public GameObject panelMessage;

    [Header("Card List UI")]
    public Button[] cardButtons;        // 0~3
    public TMP_Text[] remainTexts;      // 각 카드 남은 장수 표시

    [Header("Confirm UI")]
    public TMP_Text selectedNameText;
    public TMP_Text selectedDescText;   // 없으면 비워도 됨
    public TMP_Text selectedRemainText;
    public Button yesButton;
    public Button noButton;

    [Header("Message UI")]
    public TMP_Text messageText;
    public Button okButton;

    [Header("Card Info (optional)")]
    public string[] cardNames = new string[4] { "Runner's High", "U Turn", "Side Step", "Anchor" };
    [TextArea] public string[] cardDescs = new string[4] { "", "", "", "" };

    private int selectedIndex = -1;

    void Start()
    {   
        // 카드 버튼 연결
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i;
            cardButtons[i].onClick.AddListener(() => OnPickCard(idx));
        }
        
        yesButton.onClick.AddListener(OnYesUse);
        noButton.onClick.AddListener(OnNoUse);
        okButton.onClick.AddListener(OnOk);

        ShowCardList();
        if (!GameSettings.isSet)
        {
            GameSettings.SetInitialCardCounts(new int[] { 3, 3, 2, 2 }); //아직 Set 안 됐으면 임시로 넣음      
        }
        
    }
    

    void ShowCardList()
    {
        panelCardList.SetActive(true);
        panelConfirm.SetActive(false);
        panelMessage.SetActive(false);

        RefreshRemainUI();
        selectedIndex = -1;
    }

    void ShowConfirm()
    {
        panelCardList.SetActive(false);
        panelConfirm.SetActive(true);
        panelMessage.SetActive(false);

        string name = SafeGet(cardNames, selectedIndex, $"Card {selectedIndex}");
        if (selectedNameText != null) selectedNameText.text = name;

        if (selectedDescText != null)
            selectedDescText.text = SafeGet(cardDescs, selectedIndex, "");

        int remain = GameSettings.GetCardCount(selectedIndex);
        if (selectedRemainText != null)
            selectedRemainText.text = $"left cards: {remain}";

        // 남은 장수 0이면 Yes 비활성
        if (yesButton != null)
            yesButton.interactable = (remain > 0);
    }

    void ShowMessage(string msg)
    {
        panelCardList.SetActive(false);
        panelConfirm.SetActive(false);
        panelMessage.SetActive(true);

        if (messageText != null)
            messageText.text = msg;
    }

    void OnPickCard(int idx)
    {
        selectedIndex = idx;
        ShowConfirm();
    }

    void OnYesUse()
    {
        if (selectedIndex < 0) return;

        bool success = GameSettings.UseCard(selectedIndex);
        string name = SafeGet(cardNames, selectedIndex, $"Card {selectedIndex}");
        
        if (success)
            ShowMessage($"You used 1 {name}");
        else
            ShowMessage($"You cannot use {name}");
    }

    void OnNoUse()
    {
        // 다시 카드 선택 화면으로
        ShowCardList();
    }

    void OnOk()
    {
        // 여기서 “패널 닫기”가 목표면 SetActive(false)로 꺼도 됨
        // 지금은 다시 카드 선택으로 돌아가게 해두자
        ShowCardList();
    }

    void RefreshRemainUI()
    {
        for (int i = 0; i < remainTexts.Length; i++)
        {
            if (remainTexts[i] != null)
                remainTexts[i].text = GameSettings.GetCardCount(i).ToString();
        }
    }

    string SafeGet(string[] arr, int idx, string fallback)
    {
        if (arr == null || idx < 0 || idx >= arr.Length) return fallback;
        return string.IsNullOrEmpty(arr[idx]) ? fallback : arr[idx];
    }

    // 외부에서 턴 시작할 때 패널 열고 싶으면 이걸 호출하면 됨
    public void OpenPanel()
    {
        gameObject.SetActive(true);
        ShowCardList();
    }

    // 외부에서 닫고 싶으면
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}

