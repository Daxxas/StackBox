using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonScript : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 0;
    }


    public void PlayClicked()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        
    }
}
