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
        direction.SetActive(true);
    }
}
