using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventController : MonoBehaviour
{
    public static GlobalEventController Instance;
    public List<EventSystemListener> listeners;
    public List<EventSystemTrigger> triggers;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
