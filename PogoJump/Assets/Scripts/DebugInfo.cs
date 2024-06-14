using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    //references to on-screen text components so it doesn't clutter the main scripts, 
    //as they are only used while debugging
    
    public TextMeshProUGUI forceApplied;
    public TextMeshProUGUI bodyVelocity;
    public TextMeshProUGUI angularVelocity;
    public TextMeshProUGUI bodyRotation;
    public TextMeshProUGUI calculatedRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
