using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemChoiceUI : MonoBehaviour
{   
    public string texttext;
    public string texttext1;
    public string texttext2;

    private Button telescopeBtn;
    private Button metalBtn;


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

    // void Awake()
    // {
    //     messageText = GetComponent<TMP_Text>();
    // }

    void Start()
    {
        // 초기 화면: Yes/No만
        //ShowYesNo();

        yesButton.onClick.AddListener(OnYes);
        noButton.onClick.AddListener(OnNo);

        metalButton.onClick.AddListener(() => OnSelectItem(0));
        scopeButton.onClick.AddListener(() => OnSelectItem(1));

        if (okButton != null)
            okButton.onClick.AddListener(ShowYesNo);

        // MainGame 버튼들 찾아두기 (Hierarchy 경로)
        telescopeBtn = GameObject.Find("Canvas/MainGame/Telescope")?.GetComponent<Button>();
        metalBtn = GameObject.Find("Canvas/MainGame/MetalDetector")?.GetComponent<Button>();

        if (telescopeBtn == null) Debug.LogError("Telescope Button 못 찾음: Canvas/MainGame/Telescope 확인");
        if (metalBtn == null) Debug.LogError("MetalDetector Button 못 찾음: Canvas/MainGame/MetalDetector 확인");
    
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

        /*if (telescopeBtn != null) telescopeBtn.interactable = true;
        if (metalBtn != null) metalBtn.interactable = true;*/

    }

    void OnNo()
    {
        panelYesNo.SetActive(false);
        panelItemSelect.SetActive(false);
        panelMessage.SetActive(true);

        if (messageText != null)
            messageText.text = texttext;
            
        GameManager.Instance.playerHealth++;
        // 여기서 나중에 공격력+1 로직 연결하면 됨

        if (telescopeBtn != null) telescopeBtn.interactable = false;
        if (metalBtn != null) metalBtn.interactable = false;

    }



    void OnSelectItem(int itemIndex)
    {
        switch (itemIndex)
        {
            case 0:
                metalBtn.interactable = true;
                telescopeBtn.interactable = false;
                break;
            case 1:
                metalBtn.interactable = false;
                telescopeBtn.interactable = true;
                break;
        }
        
        selectedItem = itemIndex;
        SetItemHighlight(selectedItem);

        // 여기서 "바로 아이템 사용" 처리 (나중에 연결)
        // 예: UseItem(selectedItem);

        // 지금은 테스트로 메시지 띄우고 다시 돌아가도 됨
        panelItemSelect.SetActive(false);
        panelMessage.SetActive(true);
        if (messageText != null)
            messageText.text = (selectedItem == 0)
                ? texttext1
                : texttext2;
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
