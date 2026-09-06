//This script will only work in editor mode. You cannot adjust the scale dynamically in-game!
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR 
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class ParticleScale : MonoBehaviour 
{
    ParticleSystem[] ps;
    public float psScaleFloat = 2f;

    void Reset()
    {
        foreach (var item in transform.GetComponentsInChildren<ParticleSystem>())
        {
            var main = item.main;
            main.scalingMode = ParticleSystemScalingMode.Local;
            item.transform.localScale = new Vector3(psScaleFloat, psScaleFloat, psScaleFloat);
        }
    }	
}