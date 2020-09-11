using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Button button;

    public AudioSource source;
    
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            Debug.Log("player");
            source.PlayOneShot(source.clip);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
