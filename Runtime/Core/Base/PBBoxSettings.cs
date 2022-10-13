// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace PBBox
// {
//     internal partial class PBBoxSettings : ScriptableObject
//     {
//         private static PBBoxSettings _instance;
//         public static PBBoxSettings Instance
//         {
//             get
//             {
//                 if (_instance == null)
//                 {
//                     LoadSetting();
//                 }
//                 return _instance;
//             }
//         }

// #if UNITY_EDITOR
//         private const string SettingAssetPath = "Assets/Resources/PBBox/Settings.asset";

//         [UnityEditor.MenuItem("PBBox/Settings", false, 100)]
//         private static void ShowSetting()
//         {
//             UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath(SettingAssetPath, typeof(PBBoxSettings));
//         }

//         [UnityEditor.InitializeOnLoadMethod]
//         private static void CheckSettingAsset()
//         {
//             if (!UnityEditor.AssetDatabase.LoadAssetAtPath(SettingAssetPath, typeof(PBBoxSettings)))
//             {
//                 DebugUtils.Internal.Info<PBBoxSettings>("创建Settings");
//                 UnityEditor.AssetDatabase.CreateAsset(Instance, SettingAssetPath);
//             }
//         } 
// #endif

//         private static PBBoxSettings LoadSetting()
//         {
//             _instance = Resources.Load<PBBoxSettings>("PBBox/Settings");
//             if (_instance == null)
//             {
//                 _instance = ScriptableObject.CreateInstance(typeof(PBBoxSettings)) as PBBoxSettings;
//             }
//             return _instance;
//         }
//     }
// }