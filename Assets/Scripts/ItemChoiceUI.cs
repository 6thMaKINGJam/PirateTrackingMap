using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemChoiceUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelYesNo;
    public GameObject panelItemSelect;
    public GameObject panelMessage;

    [Header("Yes/No Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Item Buttons")]
    public Button metalButton;
    public Button scopeButton;

    [Header("Item Visuals")]
    public Image metalImage;
    public Image scopeImage;
    public Outline metalOutline;
    public Outline scopeOutline;

    [Header("Message")]
    public TMP_Text messageText;
    public Button okButton; // 없으면 비워도 됨

    private int selectedItem = -1; // 0=metal, 1=scope

    void Start()
    {
        // 초기 화면: Yes/No만
        ShowYesNo();

        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(OnNo);

        metalButton.onClick.AddListener(() => OnSelectItem(0));
        scopeButton.onClick.AddListener(() => OnSelectItem(1));

        if (okButton != null)
            okButton.onClick.AddListener(ShowYesNo);
    }

    void ShowYesNo()
    {
        panelYesNo.SetActive(true);
        panelItemSelect.SetActive(false);
        panelMessage.SetActive(false);

        selectedItem = -1;
        SetItemHighlight(-1);
        
        GameManager.Instance.cardSetupUI.ItemSelectUI.SetActive(false);
    }

    void OnYes()
    {
        panelYesNo.SetActive(false);
        panelItemSelect.SetActive(true);
        panelMessage.SetActive(false);

        // 아이템 선명하게
        SetItemEnabledVisual(true);
        SetItemHighlight(-1);
    }

    void OnNo()
    {
        panelYesNo.SetActive(false);
        panelItemSelect.SetActive(false);
        panelMessage.SetActive(true);

        if (messageText != null)
            messageText.text = "Attack power increased by +1.";

        // 여기서 나중에 공격력+1 로직 연결하면 됨
    }

    void OnSelectItem(int itemIndex)
    {
        selectedItem = itemIndex;
        SetItemHighlight(selectedItem);

        // 여기서 "바로 아이템 사용" 처리 (나중에 연결)
        // 예: UseItem(selectedItem);

        // 지금은 테스트로 메시지 띄우고 다시 돌아가도 됨
        panelItemSelect.SetActive(false);
        panelMessage.SetActive(true);
        if (messageText != null)
            messageText.text = (selectedItem == 0)
                ? "You used metal detector."
                : "You used telescope.";
    }

    void SetItemHighlight(int idx)
    {
        if (metalOutline != null) metalOutline.enabled = (idx == 0);
        if (scopeOutline != null) scopeOutline.enabled = (idx == 1);
    }

    void SetItemEnabledVisual(bool enabled)
    {
        // 흐릿/선명: 색으로 처리 (원하면 alpha만 조절해도 됨)
        if (metalImage != null)
            metalImage.color = enabled ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        if (scopeImage != null)
            scopeImage.color = enabled ? Color.white : new Color(1f, 1f, 1f, 0.4f);
    }
}
