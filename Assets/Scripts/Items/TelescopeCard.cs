using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelescopeCard : MonoBehaviour
{
    public GameObject direction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTelescopeCard()
    {
        StartCoroutine("ShowDirection");
    }

    private IEnumerator ShowDirection()
    {
        direction.SetActive(true);
        
        yield return new WaitForSeconds(2.0f);
        
        direction.SetActive(false);
    }
}
