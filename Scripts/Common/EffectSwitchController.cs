using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSwitchController : MonoBehaviour
{
    [SerializeField] private List<EffectControllerData> listData;
    
    public void SwitchEffect(string strEffectName, bool bOn)
    {
        foreach (var data in listData)
        {
            if (data != default && data.strResourceName == strEffectName)
            {
                if (bOn)
                {
                    if (data.goEff == default)
                    {
                        data.goEff = ResMgr.Ins.GetResourceInstantiate(strEffectName, data.transParent, ResouceType.Effect);
                        data.goEff.transform.localPosition = data.localPos;
                        data.goEff.SetActive(true);
                    }
                    else
                    {
                        if (!data.goEff.activeSelf)
                        {
                            data.goEff.transform.localPosition = data.localPos;
                            data.goEff.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (data.goEff != default)
                        data.goEff.SetActive(false);
                }
            }
        }
    }
    
    [Serializable]
    private class EffectControllerData
    {
        public string strResourceName;
        public Transform transParent;
        public Vector3 localPos = Vector3.zero;
        [HideInInspector] public GameObject goEff;
    }
}
