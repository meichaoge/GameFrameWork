using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestLoopCircleItem : MonoBehaviour {
    public Text ShowText;
    private LoopCircleItem m_LoopCircleItem;
    public Button mConfirmButton;

    private System.Action<LoopCircleItem> ClickCallback;
    private void Awake()
    {
        ShowText = transform.Find("Text").GetComponent<Text>();
        mConfirmButton= transform.Find("ConfirmButton").GetComponent<Button>();
        m_LoopCircleItem = transform.GetComponent<LoopCircleItem>();
        mConfirmButton.onClick.AddListener(OnButtonClick);
    }

    public void Show(System.Action<LoopCircleItem> callback)
    {
        ClickCallback = callback;
     //   Debug.Log(string.Format("item={0}  index={1}", gameObject.name, m_LoopCircleItem.DataIndex));

        ShowText.text = LooCircledDataManager.Instance. GetDateByIndex(m_LoopCircleItem.DataIndex) .ToString();
    }


    void OnButtonClick()
    {

       LooCircledDataManager.Instance.DeleteDataByIndex(m_LoopCircleItem.DataIndex);
        if (ClickCallback != null)
            ClickCallback(m_LoopCircleItem);
    }


}
