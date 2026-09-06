////////////////////////////////////////////////////////////////////////////////////////////////
//formMgr form管理类，用于管理所有form的打开，关闭等操作
//创建者：LXR
////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class FormMgr : UnitySingleton<FormMgr>
{
    ////根form
    private GameObject mRoot = null;

    ////show form stack
    private Stack<Form> mFormStack = new Stack<Form>();

    ////加载过的object的字典，减少重复加载的消耗
    private Dictionary<string, GameObject> mCacheForms = new Dictionary<string, GameObject>();

    private int mGuideID=-1 ; //是否在Form关闭后强制触发主线任务对应引导
    FormTipMsg formTipsMsg = null;


    protected override void Awake()
    {
        base.Awake();
        mRoot = GameObject.Find("UIRoot");
        if (mRoot == null)
        {
            Debuger.Log("there is no gameUIRoot here!");
            return;
        }
        
     
    }

    /// <summary>
    /// 只保留该弹窗
    /// </summary>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Form OpenForm_Replace<T>(params object[] param) where T : Form
    {
        while (mFormStack.Count > 0)
        {
            PopForm();
        }

        return pushForm<T>(param);
    }

    public Form pushForm<T>(params object[] param) where T : Form
    {
        string formName = typeof(T).ToString();
        Form form = GetFormFromStack<T>();
        if (form == null)
        {
            form = GetFreeInCache("Form/" + formName);
            if (form == null)
                return null;
            var topForm = GetTopForm();
            if (topForm != null)
            {
                if (topForm.isPopForm)
                {
                    PopForm();
                }
                else
                {
                    GetTopForm().Show(false);
                }
            }

            mFormStack.Push(form);
        }

        PopFormUntilName<T>();
        form.Show(false);
        form.param = param;
        form.transform.SetAsLastSibling();
        form.Show(true);
        return form;
    }

    //弹窗
    public Form pushPopForm<T>(params object[] param) where T : Form
    {
        string formName = typeof(T).ToString();
        Form form = GetFormFromStack<T>();
        if (mFormStack.Contains(form) && form.gameObject.activeSelf) 
            return default;
        if(mFormStack.Contains(form))
        {
            foreach (var item in mFormStack)
            {
                if(item == form)
                {
                    item.param = param;
                    item.Dirty(true);
                    break;
                }
            }
            return form;
        }
        if (form == null)
        {
            form = GetFreeInCache("Form/" + formName);
            if (form == null)
                return null;
            form.isPopForm = true;
            form.transform.localPosition = new Vector3(0, 0, 0);
            mFormStack.Push(form);
        }

        form.gameObject.SetActive(false);
        form.param = param;
        form.Show(true);
        form.transform.SetAsLastSibling();
        if (formName != "FormMain") GameSoundMgr.Ins.PlaySound("sound_ui_click2");
        //if (formName != "FormMain" && formName != "FormTask")
        //    MsgMgr.Ins.Publish(GameMsg.Main_out);

        return form;
    }

    public Form GetFormFromStack<T>()
    {
        return GetFormFromStackByName(typeof(T).ToString());
    }

    public Form GetFormFromStackByName(string strName)
    {
        string formName = "Form/" + strName;
        if (mFormStack.Count > 0)
        {
            foreach (Form form in mFormStack)
            {
                if (form.name.Equals(formName) == true)
                    return form;

                foreach (Form formChild in form.mListChildForm)
                {
                    if (formChild != null && formChild.name.Equals(formName) == true)
                        return formChild;
                }
            }
        }

        return null;
    }

    private void PopFormUntilName<T>()
    {
        while (!IsTopFormInStack<T>())
        {
            Form f = mFormStack.Pop();
            if (f != null)
            {
                f.Show(false);
            }

            if (mFormStack.Count == 0)
            {
                Debuger.LogError("#=====ERROR:FormStack is empty!======");
                break;
            }
        }
    }

    public bool IsTopFormInStack<T>()
    {
        if (mFormStack.Count > 0)
        {
            if (typeof(T) == mFormStack.Peek().GetType())
                return true;
        }

        return false;
    }

    public Form PopForm()
    {
        if (mFormStack.Count == 0)
        {
            return null;
        }

        Form f = mFormStack.Pop();
        if (f != null)
        {
            //Debuger.Log(mFormStack.Count);
            f.Show(false);
            Form topForm = GetTopForm();
            if (topForm != null)
                topForm.Show(true);
            
            return f;
        }

        return null;
    }

    public void PopForm(Type typeForm)
    {
        //if (IsTopFormInStackByName<T>())
        //{
        //    PopForm();
        //}
        Form formTarget = null;
        Stack<Form> stackTmp = new Stack<Form>();

        int nCount = mFormStack.Count;
        for (int i = 0; i < nCount; i++)
        {
            Form f = mFormStack.Pop();
            if (f.GetType() == typeForm)
            {
                formTarget = f;
                break;
            }
            else
                stackTmp.Push(f);
        }

        if (formTarget != null)
            formTarget.Show(false);
        while (stackTmp.Count > 0)
        {
            mFormStack.Push(stackTmp.Pop());
        }

   
    }

    public Form GetTopForm()
    {
        return mFormStack.Count > 0 ? mFormStack.Peek() : null;
    }

    public Form GetFreeInCache(string cellPrefabName)
    {
        GameObject gameObj = null;
        if (!mCacheForms.TryGetValue(cellPrefabName, out gameObj))
        {
            GameObject objPrefab = Resources.Load<GameObject>("Prefab/" + cellPrefabName);
            if (objPrefab == null)
                return null;
            gameObj = Instantiate(objPrefab, mRoot.transform);
            if (gameObj == null)
                return null;
            gameObj.name = cellPrefabName;
            mCacheForms[cellPrefabName] = gameObj;
        }

        gameObj.SetActive(false);
        return gameObj.GetComponent<Form>();
    }

    /// <summary>
    /// 从缓存池中移除并卸载资源
    /// </summary>
    /// <param name="strPrefabName"></param>
    /// <returns></returns>
    public void UnloadInCache(Form form)
    {
        Type t = form.GetType();
        string strName = t.ToString();
        if (mCacheForms.TryGetValue("Form/" + strName, out GameObject gameObj))
        {
            PopForm(t);
            mCacheForms.Remove(strName);
            Destroy(gameObj);
        }
        else
        {
            Destroy(form.gameObject);
        }
    }

    public void SendUIEvent(GameMsg eventType, params object[] param)
    {
        int i = 0;
        while (i < mFormStack.Count)
        {
            var ui = mFormStack.ToArray()[mFormStack.Count - 1 - i];
            Dictionary<int, Callback<object[]>> callback = ui.CtorEvent();
            if (callback != null)
            {
                if (callback.ContainsKey((int) eventType))
                    callback[(int) eventType](param);
            }

            foreach (var childUI in ui.mListChildForm)
            {
                if (childUI == null)
                    continue;
                Dictionary<int, Callback<object[]>> childCallback = childUI.CtorEvent();
                if (childCallback != null)
                {
                    if (childCallback.ContainsKey((int) eventType))
                        childCallback[(int) eventType](param);
                }
            }

            i++;
        }
    }

    //Tips
    public void ShowTipsMsg(string msg, string colorHtml = "#FFFFFF")
    {
        if (formTipsMsg == null)
        {
            formTipsMsg = GetFreeInCache("Form/FormTipMsg") as FormTipMsg;
            if (formTipsMsg == null)
                return;
            formTipsMsg.transform.localPosition = new Vector3(0, 0, -500);
        }

        formTipsMsg.ShowMsg(msg);
        formTipsMsg.transform.SetAsLastSibling();
    }

    //TustLoss 

    public bool IsFormActive<T>()
    {
        Form form = GetFormFromStack<T>();
        if (!form)
            return false;

        return form.gameObject.activeSelf;
    }



    //添加触发新手引导ID
    public void SetGuideID(int id  )
    {
        mGuideID = id;
    }
}