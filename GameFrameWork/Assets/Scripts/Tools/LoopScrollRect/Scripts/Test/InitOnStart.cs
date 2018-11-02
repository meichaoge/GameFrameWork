using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStart : MonoBehaviour
{
    public static InitOnStart Instance;
    public int m_DataCount = 50;
    LoopScrollRect m_LoopScrollRect;

    public Dictionary<int, CustomerScrollRectItem> m_AllItems = new Dictionary<int, CustomerScrollRectItem>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_LoopScrollRect = GetComponent<LoopScrollRect>();
        m_LoopScrollRect.m_OnItemCreateEvent += OnItemCreate;
        m_LoopScrollRect.m_OnItemRemoveEvent += OnItemRemove;
        m_LoopScrollRect.m_OnItemViewFlushEvent += OnItemFlush;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_LoopScrollRect.RefillCells(m_DataCount);
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            m_LoopScrollRect.ClearCells();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            m_LoopScrollRect.RefreshCells();
        }

    }


    private void OnItemCreate(RectTransform item, int index)
    {
        //Debug.Log("OnItemCreate  " + index);
        CustomerScrollRectItem customer = item .GetAddComponent<CustomerScrollRectItem>();
        //        customer.InitialedScrollCellItem(index);
        if (m_AllItems.ContainsKey(index))
            m_AllItems[index] = customer;
        else
        m_AllItems.Add(index, customer);
    }
    private void OnItemFlush(int index)
    {
        Debug.Log("OnItemFlush  " + index);
        m_AllItems[index].InitialedScrollCellItem(index);
    }

    private void OnItemRemove(RectTransform trans, int index)
    {
    //    CustomerScrollRectItem customer = trans.GetComponent<CustomerScrollRectItem>();
        m_AllItems.Remove(index);

    }

}

#endif