using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main_Cards : MonoBehaviour
{
    public List<GameObject> cards;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.cardSetupUI.SelectCounts[i].ToString();
        }
    }
    
    
}
