using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestVerticalLoopCircleFixDistance : MonoBehaviour {
    public float offset = 10;
    public float tweetime = 10;

    public VertialLoopCircleFixDistance mVertialLoopCircleFixDistance;

    private float distance = 0;

    public float data;
    void Start () {
        distance = mVertialLoopCircleFixDistance.ItemSize.y + mVertialLoopCircleFixDistance.ItemSpace.y; 

    }
	
	void Update () {
        data = Random.Range(0, 2f);
        if (data >= 1)
            Down();
        else
            Up();
        return;
        if (Input.GetKeyDown(KeyCode.D))
        {
            Down();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Up();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
        //    mVertialLoopCircleFixDistance.
        }
    }


    private void Down()
    {
        if (offset > 0)
            offset = -1 * offset;
        mVertialLoopCircleFixDistance.RollView(LoopCircleDirection.Down, offset * distance, tweetime);
    }


    private void Up()
    {
        if (offset < 0)
            offset = -1 * offset;
        mVertialLoopCircleFixDistance.RollView(LoopCircleDirection.Up, offset * distance, tweetime);
    }


}
