using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CardSetupUI : MonoBehaviour
{
    [Header("Card UI")]
    public Button[] cardButtons;      // Card0~Card3의 Button
    public Image[] cardImages;        // Card0~Card3의 Image (선택 강조용)
    public TMP_Text[] countTexts;     // 각 카드의 장수 텍스트

    [Header("Control Buttons")]
    public Button plusButton;
    public Button minusButton;
    public Button startButton;

    [Header("Start Button Visual")]
    public CanvasGroup startButtonCanvasGroup; // 흐릿하게 만들기용 (없으면 추가)

    private int[] counts = new int[4];
    private int selectedIndex = 0;
    private const int TOTAL_LIMIT = 10;

    void Start()
    {
        // 카드 버튼 클릭 연결
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i; // 클로저 방지
            cardButtons[i].onClick.AddListener(() => SelectCard(idx));
        }

        plusButton.onClick.AddListener(IncreaseSelected);
        minusButton.onClick.AddListener(DecreaseSelected);
        startButton.onClick.AddListener(GoNextScene);

        // 초기 표시
        SelectCard(0);
        RefreshUI();
    }

    void SelectCard(int idx)
    {
        selectedIndex = idx;

        // 선택 강조 (테두리 느낌: 색 바꾸기)
        for (int i = 0; i < cardImages.Length; i++)
        {
            cardImages[i].color = (i == selectedIndex) ? new Color(1f, 1f, 1f, 1f) : new Color(0.85f, 0.85f, 0.85f, 1f);
        }
    }

    void IncreaseSelected()
    {
        if (GetTotal() >= TOTAL_LIMIT) return; // 합 10 넘기면 막기
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
        for (int i = 0; i < counts.Length; i++) sum += counts[i];
        return sum;
    }

    void RefreshUI()
    {
        for (int i = 0; i < countTexts.Length; i++)
            countTexts[i].text = counts[i].ToString();

        bool canStart = (GetTotal() == TOTAL_LIMIT);
        startButton.interactable = canStart;

        if (startButtonCanvasGroup != null)
        {
            startButtonCanvasGroup.alpha = canStart ? 1.0f : 0.4f; // 흐릿→선명
            startButtonCanvasGroup.blocksRaycasts = true; // 버튼 자체 클릭판정은 interactable이 막음
        }
    }

    void GoNextScene()
    {
        if (GetTotal() != TOTAL_LIMIT) return;

        // 다음 씬에서 쓰도록 저장
        for (int i = 0; i < 4; i++)
            GameSettings.cardCounts[i] = counts[i];

        SceneManager.LoadScene("GameScene"); // Build Settings에 등록된 씬 이름과 같아야 함
    }
}
