using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main_Cards : MonoBehaviour
{
    public List<GameObject> cards;
    
    private List<TextMeshProUGUI> countText;

    void Start()
    {
        countText = new List<TextMeshProUGUI>();
        
        for (int i = 0; i < cards.Count; i++)
        {
            countText.Add(cards[i].GetComponentInChildren<TextMeshProUGUI>());
        }
    }
    
    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            countText[i].text = GameManager.Instance.cardSetupUI.SelectCounts[i].ToString();
        }
    }

    private void Update()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            countText[i].text = GameManager.Instance.cardSetupUI.SelectCounts[i].ToString();
        }
    }
}
