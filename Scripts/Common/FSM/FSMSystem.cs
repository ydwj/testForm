using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class FSMSystem
{
    private List<FSMState> states;

    // 当前状态
    public eFSMStateID CurrentFsmStateID => currentState?.ID ?? eFSMStateID.NullStateID;

    private FSMState currentState;
    public FSMState CurrentState => currentState;

    //前一状态
    public eFSMStateID PreFsmStateID => preState == default ? eFSMStateID.NullStateID : preState.ID;
    private FSMState preState;
    public FSMState PreState => preState;

    private Action<FSMState> OnStateChange;

    public FSMSystem()
    {
        states = new List<FSMState>();
    }

    protected void RegisterStates(eFSMStateAttributeType fsmStateAttributeType)
    {
        Assembly asm = Assembly.GetAssembly(typeof(FSMStateAttribute));
        Type[] types = asm.GetExportedTypes();
        
        foreach (var type in types)
        {
            var attrs = type.GetCustomAttributes(typeof(FSMStateAttribute));
            foreach (var attr in attrs)
            {
                if (attr is FSMStateAttribute fsmStateAttr && fsmStateAttr.attrType == fsmStateAttributeType)
                {
                    // switch (fsmStateAttributeType)
                    // {
                    //     case eFSMStateAttributeType.Hero:
                    //     case eFSMStateAttributeType.Soldier:
                    //         {
                    //             if (attr is FSMStateAttribute attribute && attribute.attrType == fsmStateAttributeType)
                    //             {
                    //                 ConstructorInfo constructor = type.GetConstructor(new[] { typeof(FSMSystem_BaseMan) });
                    //                 if (constructor != null)
                    //                 {
                    //                     object objIns = constructor.Invoke(new object[] { this });
                    //                     AddState((FSMState_BaseMan)objIns);
                    //                 }
                    //             }
                    //         }
                    //         break;
                    //     case eFSMStateAttributeType.Customer:
                    //         {
                    //             ConstructorInfo constructor = type.GetConstructor(new[] { typeof(FSMSystem_Customrer) });
                    //             if (constructor != null)
                    //             {
                    //                 object objIns = constructor.Invoke(new object[] { this });
                    //                 AddState((FSMState_CustomerBase)objIns);
                    //             }
                    //         }
                    //         break;
                    //     case eFSMStateAttributeType.Player:
                    //         {
                    //             ConstructorInfo constructor = type.GetConstructor(new[] { typeof(FSMSystem_Player) });
                    //             if (constructor != null)
                    //             {
                    //                 object objIns = constructor.Invoke(new object[] { this });
                    //                 AddState((FSMState_PlayerBase)objIns);
                    //             }
                    //         }
                    //         break;
                    //     default:
                    //         break;
                    // }
                }
            }
        }
    }

    public void OnUpdate()
    {
        currentState?.OnUpdate();
        currentState?.Reason();
    }

    public void AddState(FSMState s)
    {
        if (s == null)
        {
            Debuger.LogError("FSM ERROR: Null reference is not allowed");
        }

        // if (states.Count == 0)
        // {
        //     states.Add(s);
        //     currentState = s;
        //     return;
        // }
        foreach (FSMState state in states)
        {
            if (state.ID == s.ID)
            {
                Debuger.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() +
                               " because state has already been added");
                return;
            }
        }

        states.Add(s);
    }

    public void DeleteState(eFSMStateID id)
    {
        if (id == eFSMStateID.NullStateID)
        {
            Debuger.LogError("FSM ERROR: NullStateID is not allowed for a real state");
            return;
        }

        foreach (FSMState state in states)
        {
            if (state.ID == id)
            {
                states.Remove(state);
                return;
            }
        }

        Debuger.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                       ". It was not on the list of states");
    }

    // public void DoTransition(eFSMTransition trans, params object[] objs)
    // {
    //     if (trans == eFSMTransition.NullTransition)
    //     {
    //         Debuger.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
    //         return;
    //     }
    //
    //     if (currentState != default)
    //     {
    //         eFSMStateID id = currentState.GetOutputState(trans);
    //         if (id == eFSMStateID.NullStateID)
    //         {
    //             Debuger.LogError("FSM ERROR: State " + currentState.ID + " does not have a target state " +
    //                            " for transition " + trans);
    //             return;
    //         }
    //
    //         bool bNewState = false;
    //         foreach (FSMState state in states)
    //         {
    //             if (state.ID == id)
    //             {
    //                 currentState?.DoBeforeLeaving();
    //
    //                 preState = currentState;
    //                 currentState = state;
    //             
    //                 currentState.DoBeforeEntering(objs);
    //
    //                 bNewState = true;
    //                 break;
    //             }
    //         }
    //
    //         if (!bNewState)
    //         {
    //             Debuger.LogError("FSM ERROR: State " + id + " does not registered");
    //         }
    //     }
    // }

    public void ChangeState(eFSMStateID newState, params object[] objs)
    {
        foreach (FSMState state in states)
        {
            if (state.ID == newState)
            {
                currentState?.DoBeforeLeaving();

                preState = currentState;
                currentState = state;

                currentState.DoBeforeEntering(objs);
                OnStateChange?.Invoke(currentState);
                break;
            }
        }
    }

    public void AddStateChangeListener(Action<FSMState> OnChange)
    {
        OnStateChange = OnChange;
    }
}