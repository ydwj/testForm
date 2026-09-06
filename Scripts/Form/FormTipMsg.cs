using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormTipMsg : Form
{
    public Text txtMsg;
    public Animation anim;
    public RectTransform rt;

    protected override void OnShow()
    {
        
    }

    public void ShowMsg(string strMsg)
    {
        gameObject.SetActive(true);
        txtMsg.text = strMsg;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        anim.Stop();
        anim.Play();
    }
}
