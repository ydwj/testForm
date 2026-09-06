// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;
// using UnityEditor;
//
// public class SpriteExportToPNG
// {
//     [MenuItem("Tools/导出精灵")]
//     static void SaveSprite()
//     {
//         string resourcesPath = "Assets/Resources/";
//         string outPath = $"F:/PsOutput";
//
//         System.IO.Directory.CreateDirectory(outPath);
//
//         UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
//         for (int i = 0; i < selectedAsset.Length; i++)
//         {
//             Texture2D tex = selectedAsset[i] as Texture2D;
//             TextureImporter ti = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(tex)) as TextureImporter;
//
//             var importSetting = new TextureImporterSettings();
//             ti.ReadTextureSettings(importSetting);
//             importSetting.readable = true;
//             ti.SetTextureSettings(importSetting);
//
//             var platformSetting = ti.GetDefaultPlatformTextureSettings();
//             platformSetting.format = TextureImporterFormat.RGBA32;
//             ti.SetPlatformTextureSettings(platformSetting);
//
//             AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tex));
//         }
//
//         Object[] selectedObjs = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
//         foreach (Object obj in selectedObjs)
//         {
//             string selectionPath = AssetDatabase.GetAssetPath(obj);
//
//             if (!selectionPath.StartsWith(resourcesPath))
//             {
//                 Debug.Log("Dir Must In Resources!");
//                 continue;
//             }
//
//             string loadPath = selectionPath.Remove(0, resourcesPath.Length);
//             // 加载此文件下的所有资源
//             Sprite[] sprites = Resources.LoadAll<Sprite>(loadPath);
//             if (sprites.Length > 0)
//             {
//                 foreach (Sprite sprite in sprites)
//                 {
//                     Texture2D tex = new Texture2D((int) sprite.rect.width, (int) sprite.rect.height,
//                         sprite.texture.format, false);
//                     tex.SetPixels(sprite.texture.GetPixels((int) sprite.rect.xMin, (int) sprite.rect.yMin,
//                         (int) sprite.rect.width, (int) sprite.rect.height));
//                     tex.Apply();
//
//                     string filePath = string.Format($"{outPath}/{sprite.name}.png");
//                     System.IO.File.WriteAllBytes(filePath, tex.EncodeToPNG());
//                     Debug.Log("SaveSprite to " + filePath);
//                 }
//             }
//         }
//
//         Debug.Log("SaveSprite Finished");
//     }
// }