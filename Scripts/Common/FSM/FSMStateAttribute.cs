using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eFSMStateAttributeType
{
    Hero,
    Soldier,
    Customer,
    Player
}

[AttributeUsage(AttributeTargets.Class)]
public class FSMStateAttribute : Attribute
{
    public eFSMStateAttributeType attrType { get; set; }
    public FSMStateAttribute(eFSMStateAttributeType attrType)
    {
        this.attrType = attrType;
    }
}
