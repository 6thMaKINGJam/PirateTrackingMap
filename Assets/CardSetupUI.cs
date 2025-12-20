using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSetupUI : MonoBehaviour
{
    [Header("Card UI")]
    public Button[] cardButtons;      // Card0~Card3의 Button
    public Image[] cardImages;        // Card0~Card3의 Image
    public TMP_Text[] countTexts;     // 각 카드의 장수 텍스트

    [Header("Control Buttons")]
    public Button plusButton;
    public Button minusButton;
    public Button startButton;

    [Header("Start Button Visual")]
    public CanvasGroup startButtonCanvasGroup;

    private const int CARD_COUNT = 4;
    private const int TOTAL_LIMIT = 10;

    private int[] counts = new int[CARD_COUNT];
    private int selectedIndex = 0;

    public Outline[] cardOutlines;

    void Start()
    {
        // 카드 선택 버튼 연결
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i; // 클로저 방지
            cardButtons[i].onClick.AddListener(() => SelectCard(idx));
        }

        plusButton.onClick.AddListener(IncreaseSelected);
        minusButton.onClick.AddListener(DecreaseSelected);
        startButton.onClick.AddListener(ConfirmCardSelection);

        SelectCard(0);
        RefreshUI();
    }

    void SelectCard(int idx)
    {
        selectedIndex = idx;

        for (int i = 0; i < cardImages.Length; i++)
        {
            // 기본 색
            cardImages[i].color = new Color(0.85f, 0.85f, 0.85f, 1f);

            // 테두리 끄기
            if (cardOutlines[i] != null)
                cardOutlines[i].enabled = false;
        }

        // 선택된 카드
        cardImages[selectedIndex].color = Color.white;

        if (cardOutlines[selectedIndex] != null)
            cardOutlines[selectedIndex].enabled = true;
    }


    void IncreaseSelected()
    {
        if (GetTotal() >= TOTAL_LIMIT) return;
        counts[selectedIndex]++;
        RefreshUI();
    }

    void DecreaseSelected()
    {
        if (counts[selectedIndex] <= 0) return;
        counts[selectedIndex]--;
        RefreshUI();
    }

    int GetTotal()
    {
        int sum = 0;
        for (int i = 0; i < counts.Length; i++)
            sum += counts[i];
        return sum;
    }

    void RefreshUI()
    {
        for (int i = 0; i < countTexts.Length; i++)
            countTexts[i].text = counts[i].ToString();

        bool canStart = (GetTotal() == TOTAL_LIMIT);
        startButton.interactable = canStart;

        if (startButtonCanvasGroup != null)
            startButtonCanvasGroup.alpha = canStart ? 1.0f : 0.4f;
    }

    // =========================
    // 외부(다음 씬)에서 쓰는 핵심 함수
    // =========================
    public void ConfirmCardSelection()
    {
        if (GetTotal() != TOTAL_LIMIT) return;

        GameSettings.SetInitialCardCounts(counts);
        // 씬 전환은 GameScene 담당자가 처리
    }

    // (선택) 테스트 / 디버그용
    public int[] GetSelectedCardCounts()
    {
        int[] copy = new int[counts.Length];
        counts.CopyTo(copy, 0);
        return copy;
    }
}
