// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Tool.Database;
// using UnityEngine;
// using UnityEditor;
//
// public static class MyHelper_Bath
// {
//     [MenuItem("Assets/CreateDevicePrefabs")]
//     public static void CreateDevicePrefabs()
//     {
//         DeviceLvlConfigDatabase database = new DeviceLvlConfigDatabase();
//         database.Load();
//         var allDeviceConfigs = database.FindAll(_ => _.ID % 10000 > 1000);
//
//         string strPrefabPath = "Assets/Resources/Prefab/Device";
//         var allAssetPathGUIDs = AssetDatabase.FindAssets("t:prefab", new string[] {strPrefabPath});
//         List<Object> allAssets = new List<Object>();
//         
//         foreach (var guid in allAssetPathGUIDs)
//         {
//             var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
//             allAssets.Add(asset);
//         }
//         
//         Object templatePrefab = allAssets.FirstOrDefault(_ => _.name == "Device_1003_lvl_1");
//         string strPath = AssetDatabase.GetAssetPath(templatePrefab);
//
//         foreach (var config in allDeviceConfigs)
//         {
//             string strPrefabName = config.prefabName;
//             
//             if (allAssets.FirstOrDefault(_ => _.name == strPrefabName) != default)
//                 continue;
//             
//             AssetDatabase.CopyAsset(strPath, string.Format("{0}/Map_02/{1}.prefab", strPrefabPath, strPrefabName));
//         }
//         
//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//     }
//     
//     [MenuItem("Assets/关联美术预制体")]
//     public static void RefreshArtDevicePrefabs()
//     {
//         string strPrefabPath = "Assets/Resources/Prefab/Device";
//         string strArtPrefabPath = "Assets/Art/Scenes/Map_02/Prefab/Device";
//         string strTemplatePrefabName = "Device_2103_lvl_1";
//         
//         DeviceLvlConfigDatabase database = new DeviceLvlConfigDatabase();
//         database.Load();
//         var allDeviceLvlConfigs = database.FindAll(_ => _.ID % 10000 > 1000); //Map_02
//         
//         var allAssetPathGUIDs = AssetDatabase.FindAssets("t:prefab", new string[] {strPrefabPath});
//         var allArtAssetPathGUIDs = AssetDatabase.FindAssets("t:prefab", new string[] {strArtPrefabPath});
//         
//         List<GameObject> allAssets = new List<GameObject>();
//         List<GameObject> allArtAssets = new List<GameObject>();
//         foreach (var guid in allAssetPathGUIDs)
//         {
//             var asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
//             allAssets.Add(asset);
//         }
//         foreach (var guid in allArtAssetPathGUIDs)
//         {
//             var asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
//             allArtAssets.Add(asset);
//         }
//
//         foreach (GameObject gameObject in allAssets.Where(_ => allDeviceLvlConfigs.Select(c=> c.prefabName).Contains(_.name)))
//         {
//             // bool bConinue = false;
//             // foreach (Transform child in gameObject.transform)
//             // {
//             //     //已经包含了美术预制体
//             //     if (child.name == gameObject.name)
//             //     {
//             //         bConinue = true;
//             //         break;
//             //     }
//             // }
//             //
//             // if (bConinue)
//             //     continue;
//             
//             for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
//             {
//                 GameObject goChild = gameObject.transform.GetChild(i).gameObject;
//                 //if (goChild.name != "ServicePt" && goChild.GetComponent<Animator>() == default && goChild.GetComponentInChildren<SkinnedMeshRenderer>() == default)
//                 if (goChild.name.StartsWith("Device_"))
//                 {
//                     GameObject.DestroyImmediate(goChild, true);
//                 }
//             }
//
//             GameObject goArtPrefab = allArtAssets.FirstOrDefault(_ => _.name == gameObject.name);
//             if (goArtPrefab == null && gameObject.GetComponent<Device_Technician>() == default && gameObject.GetComponentInChildren<SkinnedMeshRenderer>() == default)
//             {
//                 goArtPrefab = allArtAssets.FirstOrDefault(_ => _.name == strTemplatePrefabName);
//             }
//             if (goArtPrefab == null)
//                 continue;
//
//             GameObject goArtChild = PrefabUtility.InstantiatePrefab(goArtPrefab) as GameObject;
//             goArtChild.name = gameObject.name;
//
//             GameObject goNew = GameObject.Instantiate(gameObject);
//             goArtChild.transform.SetParent(goNew.transform);
//             goArtChild.transform.localPosition = Vector3.zero;
//             goArtChild.transform.localRotation = Quaternion.Euler(0, 0, 0);
//             goArtChild.transform.localScale = Vector3.one;
//             goArtChild.SetActive(true);
//
//             BoxCollider collider = goNew.GetOrAddComponent<BoxCollider>();
//             Renderer renderer = goArtChild.transform.GetComponent<Renderer>();
//             collider.center = renderer.bounds.center;
//             collider.size = renderer.bounds.size;
//
//             PrefabUtility.SaveAsPrefabAsset(goNew, string.Format("{0}/{1}.prefab", strPrefabPath, gameObject.name));
//             GameObject.DestroyImmediate(goNew);
//         }
//         
//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//     }
//     
//     // [MenuItem("Assets/重新关联预制体")]
//     // public static void ReBindPrefab()
//     // {
//     //     foreach (GameObject gameObject in Selection.gameObjects)
//     //     {
//     //         if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
//     //         {
//     //             // 获取预制体资源
//     //             var prefabAsset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
//     //                             
//     //             GameObject goPrefab = PrefabUtility.InstantiatePrefab(prefabAsset, gameObject.transform.parent) as GameObject;
//     //             goPrefab.name = gameObject.name;
//     //             goPrefab.transform.localPosition = gameObject.transform.localPosition;
//     //             GameObject.DestroyImmediate(gameObject, true);
//     //         }
//     //     }
//     //
//     //     AssetDatabase.SaveAssets();
//     //     AssetDatabase.Refresh();
//     // }
//     
//     [MenuItem("GameObject/去掉节点下所有MeshCollider", false, priority = 2)]
//     public static void RemoveAllMeshCollider()
//     {
//         GameObject[] selectGos = Selection.gameObjects;
//         
//         foreach (GameObject go in selectGos)
//         {
//             MeshCollider[] allMeshCollider = go.transform.GetComponentsInChildren<MeshCollider>();
//
//             for (int i = allMeshCollider.Length - 1; i >= 0; i--)
//             {
//                 GameObject goSelf = allMeshCollider[i].gameObject;
//                 Object.DestroyImmediate(allMeshCollider[i], true);
//                 EditorUtility.SetDirty(goSelf);
//             }
//             
//             EditorUtility.SetDirty(go);
//         }
//         
//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//     }
//     
//     [MenuItem("Assets/更改材质为GD_Toon")]
//     public static void ChangeShader_Toon()
//     {
//         Material[] selectedObjs = Selection.GetFiltered<Material>(SelectionMode.DeepAssets);
//
//         foreach (Material mat in selectedObjs)
//         {
//             mat.shader = Shader.Find("GD/Mobile/Toon");
//             mat.SetFloat("_Brightness", 1.2f);
//             mat.SetFloat("_Contrast", 1.05f);
//             mat.SetFloat("_OutlineWidth", 0f);
//         }
//         
//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//     }
//     
//     [MenuItem("Assets/更改材质为GD_Diffuse")]
//     public static void ChangeShader_Diffuse()
//     {
//         Material[] selectedObjs = Selection.GetFiltered<Material>(SelectionMode.DeepAssets);
//
//         foreach (Material mat in selectedObjs)
//         {
//             mat.shader = Shader.Find("GD/Mobile/Diffuse");
//             mat.SetFloat("_Brightness", 1.4f);
//             mat.SetFloat("_Contrast", 1.08f);
//         }
//         
//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//     }
// }