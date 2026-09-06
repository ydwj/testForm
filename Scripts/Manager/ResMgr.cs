using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.U2D;

//资源类型
public enum ResouceType
{
    UI,
    Icon,
    PrefabItem,
    Atlas,
    Animation,
    
    Man,
    Skill,
    Effect,
    Temp,
    Count,
    Timeline,
    DialogueGraphNew,
    Other
}

/// <summary>
/// 资源管理器
/// </summary>
public class ResMgr : Singleton<ResMgr>
{
    /// <summary>
	/// 容器类
	/// </summary>
	private class PrefabContainer
    {
        public Dictionary<string, GameObject> mGameObjs = new Dictionary<string, GameObject>();
        public Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
        public Dictionary<string, Texture2D> mTextures = new Dictionary<string, Texture2D>();
    }
    //预设资源缓存数组
    private PrefabContainer[] prefabPool = new PrefabContainer[(int)ResouceType.Count];
    //图集缓存
    Dictionary<string, SpriteAtlas> mDicSpriteAtlas = new Dictionary<string, SpriteAtlas>();
    Dictionary<string, Material> mDicMaterials = new Dictionary<string, Material>();

    //动态缓存Prefab，会被手动清理掉
    private Dictionary<string, GameObject> prefabPoolDynamic = new Dictionary<string, GameObject>();

    //Prefab对象池
    private Dictionary<string, GameObjectPool> mDicPrefabPool = new Dictionary<string, GameObjectPool>();

    /// <summary>
    /// 初始化
    /// </summary>
    public ResMgr()
    {
        for (int i = 0; i < (int)ResouceType.Count; i++)
        {
            prefabPool[i] = new PrefabContainer();
        }
        mDicPrefabPool.Clear();
    }
    
    public Sprite AddSpriteResource(string spriteName, ResouceType type)
    {
        string spritePath = string.Format("Sprite/{0}/{1}", type, spriteName);
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        if (sprite)
            prefabPool[(int)type].mSprites.Add(spriteName, sprite);
        return sprite;
    }
    public Texture2D AddTexture2DResource(string textureName, ResouceType type)
    {
        string texturePath = string.Format("Sprite/{0}/{1}", type, textureName);
        Texture2D texture = Resources.Load<Texture2D>(texturePath);
        if (texture)
            prefabPool[(int)type].mTextures.Add(textureName, texture);
        return texture;
    }

    public void AddPrefabResource(string prefabName, ResouceType type)
    {
        if (!prefabPool[(int) type].mGameObjs.ContainsKey(prefabName))
        {
            string prefabPath = string.Format("Prefab/{0}/{1}", type, prefabName);
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab)
            {
                prefabPool[(int) type].mGameObjs.Add(prefabName, prefab);
            }
        }
    }

    public Material GetMaterial(string strMatName)
    {
        if (!mDicMaterials.ContainsKey(strMatName))
        {
            Material material = Resources.Load<Material>(string.Format($"Materials/{strMatName}"));
            if (material)
                mDicMaterials[strMatName] = material;
        }
        
        if (mDicMaterials.TryGetValue(strMatName, out Material mat))
            return mat;

        return default;
    }

    /// <summary>
    /// 获取预设资源 
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="parent"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetResource(string prefabName, Transform parent, ResouceType type)
    {
        AddPrefabResource(prefabName, type);
        if (prefabPool[(int) type].mGameObjs.TryGetValue(prefabName, out GameObject goPrefab))
        {
            GameObject go = GameObject.Instantiate(goPrefab, parent);
            go.transform.SetParent(parent);
            return go;
        }

        return default;
    }
    /// <summary>
    /// 获取创建资源
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="parent"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetResourceInstantiate(string prefabName, Transform parent, ResouceType type)
    {
        GameObject prefab = GetResource(prefabName, parent, type);
        if (!prefab)
        {
            Debuger.Log("没有找到资源：" + prefabName);
            return default;
        }
        
        prefab.transform.SetParent(parent);
        prefab.transform.localRotation = Quaternion.identity;
        prefab.transform.localPosition = Vector3.zero;

        return prefab;
    }
    /// <summary>
    /// 动态获取创建资源
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="parent"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetResInstantiateDyn(string prefabName, Transform parent, ResouceType type)
    {
        GameObject prefab = GetRes_Dynamic(prefabName, type);
        GameObject prefabObj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);

        prefabObj.transform.localRotation = Quaternion.identity;
        prefabObj.transform.localPosition = Vector3.zero;

        return prefabObj;
    }
    /// <summary>
    /// 获取图片资源
    /// </summary>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public Sprite GetSpriteResource(string spriteName, ResouceType type)
    {
        Sprite sprite;
        if (!prefabPool[(int)type].mSprites.ContainsKey(spriteName))
            sprite = AddSpriteResource(spriteName, type);
        else
            sprite = prefabPool[(int)type].mSprites[spriteName];
        return sprite;
    }


    /// <summary>
    /// 获取图集图片资源
    /// </summary>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public Sprite GetSpriteResource_Atlas(string spriteName, string strAtlasName)
    {
        SpriteAtlas nowAtlas;
        if (mDicSpriteAtlas.ContainsKey(strAtlasName))
            nowAtlas = mDicSpriteAtlas[strAtlasName];
        else
        {
            nowAtlas = Resources.Load<SpriteAtlas>("Atlas/" + strAtlasName);
            mDicSpriteAtlas[strAtlasName] = nowAtlas;
        }
        if (nowAtlas == null)
            return null;

        Sprite sprite = nowAtlas.GetSprite(spriteName);

        if (!prefabPool[(int)ResouceType.Atlas].mSprites.ContainsKey(spriteName))
            prefabPool[(int)ResouceType.Atlas].mSprites.Add(spriteName, sprite);
        else
            sprite = prefabPool[(int)ResouceType.Atlas].mSprites[spriteName];

        return sprite;
    }

    public Texture2D GetTexture2DResource(string textureName, ResouceType type)
    {
        Texture2D texture;
        if (!prefabPool[(int)type].mTextures.ContainsKey(textureName))
            texture = AddTexture2DResource(textureName, type);
        else
            texture = prefabPool[(int)type].mTextures[textureName];
        return texture;
    }

    public ResourceRequest LoadPrefabAsync(string prefabName, ResouceType type)
    {
        string prefabPath = string.Format("Prefab/{0}/{1}", type, prefabName);
        var sync = Resources.LoadAsync<GameObject>(prefabPath);
        return sync;
    }

    //================
    /// <summary>
    /// 获取动态预设资源 
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetRes_Dynamic(string prefabName, ResouceType type)
    {
        GameObject prefab;
        if (!prefabPoolDynamic.ContainsKey(prefabName))
            prefab = AddRes_Dynamic(prefabName, type);
        else
            prefab = prefabPoolDynamic[prefabName];
        return prefab;
    }

    /// <summary>
    /// 添加动态缓存资源
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="type"></param>
    GameObject AddRes_Dynamic(string prefabName, ResouceType type)
    {
        string prefabPath = string.Format("Prefab/{0}/{1}", type, prefabName);
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab)
            prefabPoolDynamic.Add(prefabName, prefab);
        return prefab;
    }

    /// <summary>
    /// 卸载动态缓存资源
    /// </summary>
    public void UnLoadRes_Dynamic()
    {
        foreach (var item in prefabPoolDynamic)
        {
            Resources.UnloadAsset(item.Value);
        }

        prefabPoolDynamic.Clear();
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    public void UnLoadRes(string strResName, ResouceType resouceType)
    {
        if (prefabPool[(int)resouceType].mGameObjs.TryGetValue(strResName, out GameObject go))
            Resources.UnloadAsset(go);
    }
    //===================
    /// <summary>
    /// 获取创建资源
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="parent"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetInstantiateFromPool(string prefabName, Transform parent, ResouceType type)
    {
        if (!mDicPrefabPool.ContainsKey(prefabName))
        {
            AddPrefabResource(prefabName, type);
            if (prefabPool[(int) type].mGameObjs.TryGetValue(prefabName, out GameObject goPrefab))
            {
                mDicPrefabPool[prefabName] = new GameObjectPool(goPrefab, parent);
            }
            else
                return default;
        }
        
        GameObject go = mDicPrefabPool[prefabName].GetPool(parent);
        go.transform.localPosition = Vector3.zero;
        return go;
    }
    
    public void RecycleObj(GameObject obj)
    {
        if (obj == default)
            return;
        
        if (mDicPrefabPool.ContainsKey(obj.name))
        {
            mDicPrefabPool[obj.name].Recycle(obj);
        }
    }
    public void RecycleObj(GameObject obj, float delayTime)
    {
        if (mDicPrefabPool.ContainsKey(obj.name))
        {
            Observable.Timer(TimeSpan.FromSeconds(delayTime)).Subscribe(t =>
            {
                if (obj != null)
                    mDicPrefabPool[obj.name].Recycle(obj);
            });

        }
    }

    public AnimationClip GetAnimationClip(string animName)
    {
        AnimationClip anim = Resources.Load<AnimationClip>($"Animation/{animName}");
        return anim;
    }
    
    public RuntimeAnimatorController GetAnimator(string animName)
    {
        RuntimeAnimatorController animator = Resources.Load<RuntimeAnimatorController>($"Animation/{animName}");
        return animator;
    }

    public void ClearPools()
    {
        foreach (var pool in mDicPrefabPool.Values)
        {
            pool.Release();
        }
    }

  
    public  T Load<T>(string prefabName, ResouceType type) where T : UnityEngine.Object 
    {
        string prefabPath = string.Format("Prefab/{0}/{1}", type, prefabName);
        return Resources.Load<T>(prefabPath);
    }
}
