using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LineRender : MonoBehaviour
{
    public List<Transform> Points = new List<Transform>();
    public LineRenderer m_TargetRender;
    public int m_Count;
    // Use this for initialization
    void Start()
    {
        m_TargetRender.material = new Material(Shader.Find("Particles/Additive"));
        m_TargetRender.positionCount =Mathf.Min( Points.Count, m_Count);//设置两点
        m_TargetRender.startColor = Color.yellow; //设置直线颜色
        m_TargetRender.endColor = Color.red; //设置直线颜色

        m_TargetRender.startWidth=0.02f;//设置直线宽度
        m_TargetRender.endWidth = 0.01f;//设置直线宽度
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            for (int dex=0;dex< Points.Count&&dex< m_Count; ++dex)
            {
                m_TargetRender.SetPosition(dex, Points[dex].position);
            }

        }
    }
}
