using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;


using TooSimpleFramework.UI;

[RequireComponent(typeof(Button),typeof(Image))]
public class Tab : MonoBehaviour
{
    public Image img_bg;
    [HideInInspector]
    public Image imgTab;
    [HideInInspector]
    public Button btnTab;
    [HideInInspector]
    public TabGroup tabGroup;

    public Text txtTitle;
    public GameObject goSelected ,unSelected;
    public Color cSeleted = Color.white;
    public Color cUnSeleted = Color.white;


    float fUnSeletedPosY, fSeletedPosY;
    Vector3 vUnSeletedScale, vSeletedScale;
    [HideInInspector]
    public int nIndex;

    public void Init(TabGroup tabGroup, int nIndex)
    {
        this.tabGroup = tabGroup;
        this.nIndex = nIndex;
        imgTab = GetComponent<Image>();
        btnTab = GetComponent<Button>();
        fUnSeletedPosY = transform.localPosition.y;
        fSeletedPosY = transform.localPosition.y + tabGroup.fMovefSeletedPosYOffset;
        vUnSeletedScale = Vector3.one;
        vSeletedScale = new Vector3(tabGroup.fScaleSeletedScaleOffset, tabGroup.fScaleSeletedScaleOffset, 1);
        btnTab.onClick.AddListener(() =>
        {
            tabGroup.SwitchTab(nIndex);
        });
    }

    public void Select(bool bSelect)
    {
        if (tabGroup.bMoveTab)
        {
            transform.DOLocalMoveY(bSelect ? fSeletedPosY : fUnSeletedPosY, 0.2f);
        }
        if (tabGroup.bScaleTab)
        {
            transform.DOScale(bSelect ? vSeletedScale : vUnSeletedScale, 0.2f);
        }
        if (tabGroup.bChangeSprite)
        {
            imgTab.sprite = bSelect ? tabGroup.spSeleted : tabGroup.spUnSeleted;
            img_bg?.gameObject.SetActive(!bSelect);
            //imgTab.SetNativeSize();
        }
        if (tabGroup.bResetNativeSize)
        {
            imgTab.SetNativeSize();
        }
        if (txtTitle != null)
        {
            txtTitle.color = bSelect ? cSeleted : cUnSeleted;
        }
       
        if (goSelected != null)
            goSelected.SetActive(bSelect);
        
        if (unSelected != null)
            unSelected.SetActive(!bSelect);
    }
}
