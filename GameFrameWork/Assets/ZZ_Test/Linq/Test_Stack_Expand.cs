using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test_Stack_Expand : MonoBehaviour {
    public List<int> m_AllData = new List<int>();
    public List<int> m_AllDataRemove = new List<int>();


    public Stack<int> m_Sources = new Stack<int>();
    // Use this for initialization
    void Start () {
        Debug.Log(m_Sources.Count);
    }
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.A))
        {
            m_Sources = new Stack<int>(m_AllData);  //相当于一个个push 
            Debug.Log(m_Sources.Pop());
            foreach (var item in m_Sources)
            {
                Debug.Log(">>" + item);
            }
        }  //倒序加入

        if (Input.GetKeyDown(KeyCode.B))
        {
            m_AllData.Reverse();
            m_Sources = new Stack<int>(m_AllData);
            foreach (var item in m_Sources)
            {
                Debug.Log(">>" + item);
            }
        } //顺序加入 注意处理

        if (Input.GetKeyDown(KeyCode.D))
        {
            List<int> result = m_Sources.ToList();
            foreach (var item in result)
            {
                Debug.Log(">>" + item);
            }
        } //顺序加入 注意处理


        if (Input.GetKeyDown(KeyCode.C))
        {
            Stack<int> result = m_Sources.DeleteElements<int>((a)=> {
                if (m_AllDataRemove.Contains(a))
                    return true;
                return false;
            });

            foreach (var item in result)
            {
                Debug.Log(">>" + item);
            }


        }


    }
}
