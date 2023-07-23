using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Search;
using PBBox;

namespace PBBox.CEditor
{
    /// <summary>
    /// Unity 新版搜索窗口拓展
    /// </summary>
    public static class PBSearchExtensions
    {
        /// <summary>
        /// 找到所有依赖该脚本的Prefab，格式csref:[deep:(optional)][unactive:(optional)][ClassName/FullName] 例如csref:deep:Animator
        /// </summary>
        /// <returns></returns>
        private static readonly Regex REGEX_OPTIONS = new Regex(@"(\:\s*)");
        [SearchItemProvider]
        internal static SearchProvider SearchSrciptReferences()
        {

            return new SearchProvider("pb_search_script_ref", "Prefabs with searching components")
            {
                active = false,
                filterId = "csref:",
                isExplicitProvider = true,
                fetchItems = (context, items, provider) =>
                {
                    string searchText = context.searchQuery;
                    if (string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))
                        return null;
                    searchText = REGEX_OPTIONS.Replace(searchText, ":");
                    List<string> options = new List<string>(searchText.Split(":", StringSplitOptions.RemoveEmptyEntries));
                    searchText = options[options.Count - 1];
                    options.RemoveAt(options.Count - 1);

                    bool isDeepSearch = options.Contains("deep");
                    bool isIncludeUnactive = options.Contains("unactive");
                    //TODO 拓展选项可以搜索子类 subclass:

                    string[] guids = AssetDatabase.FindAssets("t:Prefab");
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        if (prefab == null)
                            continue;
                        Component[] components;
                        if (isDeepSearch)
                        {
                            components = prefab.GetComponentsInChildren<Component>(isIncludeUnactive);
                        }
                        else
                        {
                            components = prefab.GetComponents<Component>();
                        }
                        bool isContains = false;
                        foreach (var c in components)
                        {
                            if (c.name.Contains(searchText, System.StringComparison.OrdinalIgnoreCase)
                                || c.GetType().FullName.Contains(searchText, System.StringComparison.OrdinalIgnoreCase))
                            {
                                isContains = true;
                                break;
                            }
                        }
                        if (isContains)
                        {
                            items.Add(provider.CreateItem(context, path, null, null, null, null));
                        }
                    }
                    return null;
                },
                fetchThumbnail = (item, context) => AssetDatabase.GetCachedIcon(item.id) as Texture2D,
                fetchPreview = (item, context, size, options) => AssetDatabase.GetCachedIcon(item.id) as Texture2D,
                fetchLabel = (item, context) => item.id,
                fetchDescription = (item, context) => AssetDatabase.LoadMainAssetAtPath(item.id)?.name,
                toObject = (item, type) => AssetDatabase.LoadMainAssetAtPath(item.id),
                showDetailsOptions = ShowDetailsOptions.Inspector | ShowDetailsOptions.Actions | ShowDetailsOptions.Default | ShowDetailsOptions.Description | ShowDetailsOptions.ListView | ShowDetailsOptions.DefaultGroup,
                trackSelection = (item, context) =>
                {
                    var obj = AssetDatabase.LoadMainAssetAtPath(item.id);
                    if (obj != null)
                    {
                        EditorGUIUtility.PingObject(obj.GetInstanceID());
                        Selection.activeObject = obj;
                    }
                },
                startDrag = (item, context) =>
                {
                    var obj = AssetDatabase.LoadMainAssetAtPath(item.id);
                    if (obj != null)
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.objectReferences = new UnityEngine.Object[] { obj };
                        DragAndDrop.StartDrag(item.label);
                    }
                }
            };
        }

    }
}