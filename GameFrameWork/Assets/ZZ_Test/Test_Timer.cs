using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Timer : MonoBehaviour {
    public float m_TimerScale = 1;
    public int m_Number = 0;
    private float time;

    public float m_TimeDelatal;
    public float m_TimerSin;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale = m_TimerScale;
        }
    //    Debug.Log("" + m_TimerSin);

        m_TimeDelatal = Time.deltaTime;
        m_TimerSin = Time.realtimeSinceStartup;
        if (Time.time-time>=1)
        {
            time = Time.time;
            ++m_Number;
        }


	}


    private void FixedUpdate()
    {
        Debug.Log("AAAAAAAAAAAAAA" + m_TimerSin);
    }






}
