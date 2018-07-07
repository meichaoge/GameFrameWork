using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestUIButton : MonoBehaviour {
    public EventTrigger m_TestEventTrigger;
    // Use this for initialization
    void Start () {
        EventTrigger.Entry test01 = new EventTrigger.Entry();
        test01.callback .AddListener( OnPointDown);
        test01.eventID = EventTriggerType.PointerDown;


        EventTrigger.Entry test02 = new EventTrigger.Entry();
        test02.callback.AddListener(OnPointUp);
        test02.eventID = EventTriggerType.PointerUp;


        EventTrigger.Entry test03 = new EventTrigger.Entry();
        test03.callback.AddListener(OnPointEnter);
        test03.eventID = EventTriggerType.PointerEnter;

        EventTrigger.Entry test04 = new EventTrigger.Entry();
        test04.callback.AddListener(OnPointExit);
        test04.eventID = EventTriggerType.PointerExit;

        m_TestEventTrigger.triggers.Add(test01);
        m_TestEventTrigger.triggers.Add(test02);
        m_TestEventTrigger.triggers.Add(test03);
        m_TestEventTrigger.triggers.Add(test04);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_IsPointEnter && m_IsPointDown)
            Debug.Log("XXXXXXXXXXXXX");
	}

    private bool m_IsPointDown = false;
    private bool m_IsPointEnter = false;

    private void OnPointDown(BaseEventData eventData)
    {
        m_IsPointDown = true;
        Debug.Log("AAAA");
    }


    private void OnPointUp(BaseEventData eventData)
    {
        m_IsPointDown = false;
        Debug.Log("bbbbb");
    }

    private void OnPointEnter(BaseEventData eventData)
    {
        m_IsPointEnter = true;
        Debug.Log("cccccc");
    }

    private void OnPointExit(BaseEventData eventData)
    {
        m_IsPointDown = false;
        m_IsPointEnter = false;
        Debug.Log("eeeeeeeeeeee");

    }


   

}
