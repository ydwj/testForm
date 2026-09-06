using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
// public enum eFSMTransition
// {
//     NullTransition = 0,
// }
 
public enum eFSMStateID
{
    NullStateID = 0, 
    //=============Common
    Man_Idle,
    Man_MoveToTargetEnemy,
    Man_MoveToTargetPos,
    Man_NormalAttack,
    Man_DoSkill,
    Man_Patrol,
    //=============Soldier
    // Soldier_Idle,            
    // Soldier_Patrol,          
    // Soldier_MoveToTargetEnemy,
    // Soldier_Attack,

    //==============Hero
    // Hero_Idle,
    // Hero_MoveToTargetEnemy,
    // Hero_Attack,


    //=============Customer
    Customer_ToTheEntrance,           //Idle
    Customer_Leave,     //离开部落，回收顾客
    Customer_FindMarket,    //寻找市场
    Customer_Stroll,          //闲逛
    Customer_Buying,
    Customer_WaitInStrollPoint,
    Cusotmer_DoBehaviour,

    //====MainRole 
    Player_idle,
    Player_Move,
    Player_GetItem,
}
 
public abstract class FSMState
{
    protected FSMSystem fsm;
 
    protected Dictionary<int, eFSMStateID> map = new Dictionary<int, eFSMStateID>();
    protected eFSMStateID fsmStateID;
    public eFSMStateID ID { get { return fsmStateID; } }
    
    public FSMState(FSMSystem fsm)
    {
        this.fsm = fsm;
    }
 
    // public void AddTransition(eFSMTransition trans, eFSMStateID id)
    // {
    //     if (trans == eFSMTransition.NullTransition)
    //     {
    //         Debuger.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
    //         return;
    //     }
    //
    //     if (id == eFSMStateID.NullStateID)
    //     {
    //         Debuger.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
    //         return;
    //     }
    //     if (map.ContainsKey((int)trans))
    //     {
    //         Debuger.LogError("FSMState ERROR: State " + fsmStateID.ToString() + " already has transition " + trans.ToString() +
    //                        "Impossible to assign to another state");
    //         return;
    //     }
    //
    //     map.Add((int)trans, id);
    // }
    //
    // public void DeleteTransition(eFSMTransition trans)
    // {
    //     if (trans == eFSMTransition.NullTransition)
    //     {
    //         Debuger.LogError("FSMState ERROR: NullTransition is not allowed");
    //         return;
    //     }
    //     
    //     if (map.ContainsKey((int)trans))
    //     {
    //         map.Remove((int)trans);
    //         return;
    //     }
    //     Debuger.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + fsmStateID.ToString() +
    //                    " was not on the state's transition list");
    // }
    //
    // public eFSMStateID GetOutputState(eFSMTransition trans)
    // {
    //     if (map.ContainsKey((int)trans))
    //     {
    //         return map[(int)trans];
    //     }
    //     return eFSMStateID.NullStateID;
    // }
    
    public virtual void DoBeforeEntering(params object[] objs) { }
    
    public virtual void DoBeforeLeaving() { }
 
    /// <summary>
    /// 这个方法决定在当前状态的执行
    /// </summary>
    public abstract void OnUpdate();
    /// <summary>
    /// 这个方法决定状态是否应该转换到列表中的另一个状态
    /// </summary>
    public abstract void Reason();
 
}