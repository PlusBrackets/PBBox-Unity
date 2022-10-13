using System.Linq;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PBBox.CEditor
{

    public class RaycastAndMaskableModifier : ScriptableWizard
    {
        [MenuItem("PBBox/UI/Modify UI Raycast and Maskable")]
        private static void ShowWizard()
        {
            ScriptableWizard.DisplayWizard<RaycastAndMaskableModifier>("UI Raycast and Maskable Modifier", "Do");
        }

        [MenuItem("PBBox/UI/Select Interactable Object")]
        private static void SelectInteractable()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
            {
                DebugUtils.Log("需要选中GameObject");
                return;
            }
            else
            {
                HashSet<GameObject> objs = new HashSet<GameObject>();
                foreach (var g in Selection.gameObjects)
                {
                    TraverseGameObject(g, _g =>
                    {
                        if (CheckTargetInteractable(_g))
                        {
                            objs.Add(_g);
                        }
                    });
                }
                Selection.objects = objs.ToArray();
                DebugUtils.Log($"已选中 {objs.Count} 个可拥有IEventSystemHandler的Object");
            }
        }

        public GameObject target;
        [Tooltip("修改RaycastTarget")]
        public bool modifyRaycast = true;
        public bool setRaycastTrue;
        [Tooltip("修改Maskable")]
        public bool modifyMaskable;
        public bool setMaskableTrue;
        [Tooltip("当设置maskable为fasle时，忽略parent有Mask组件的GameObject")]
        public bool ignoreMaskChild = true;
        [Tooltip("忽略预制体")]
        public bool ignorePrefab = true;
        public string ignoreNameTag = "[ignore]";
        List<GameObject> m_InteractableObjects = new List<GameObject>();

        private void OnWizardUpdate()
        {
            isValid = target != null;
        }

        private void OnWizardCreate()
        {
            this.LogInfo("Modify Start");
            m_InteractableObjects.Clear();
            string[] _ignoreNameTags = ignoreNameTag.Split(",");
            ModifyTarget(target, false, _ignoreNameTags, null);
            Selection.objects = m_InteractableObjects.ToArray();
            m_InteractableObjects.Clear();
            this.LogInfo("Modify End, Selected interactable object, please check");
        }

        private void ModifyTarget(GameObject target, bool hasMaskParent, string[] tags,string path)
        {
            TraverseGameObject(target,Func);
            void Func(GameObject target)
            {
                Graphic[] gs = target.GetComponents<Graphic>();
                if (gs != null && gs.Length != 0 && !target.name.Contains(tags))
                {

                    bool canSetRaycast = modifyRaycast && !(ignorePrefab && PrefabUtility.IsPartOfAnyPrefab(target));
                    bool canSetMaskable = modifyMaskable && !hasMaskParent && canSetRaycast;

                    foreach (var g in gs)
                    {
                        Undo.RecordObject(g, "ModifyTarget");
                        string log = null;
                        if (canSetRaycast)
                        {
                            g.raycastTarget = setRaycastTrue;
                            log += (string.IsNullOrEmpty(log) ? $"{path + g} " : "") + $"set raycast {setRaycastTrue}; ";
                        }
                        if (canSetMaskable && g is MaskableGraphic _g)
                        {
                            _g.maskable = setMaskableTrue;
                            if (_g.isMaskingGraphic) hasMaskParent = true;
                            log += (string.IsNullOrEmpty(log) ? $"{path + g} " : "") + $"set maskable {setMaskableTrue}; ";
                        }
                        if (!string.IsNullOrEmpty(log))
                        {
                            this.LogInfo(log);
                        }
                    }
                }
                if (CheckTargetInteractable(target))
                {
                    m_InteractableObjects.Add(target);
                }
            }
        }

        static void TraverseGameObject(GameObject target, Action<GameObject> action)
        {
            action?.Invoke(target);
            for (int i = 0; i < target.transform.childCount; i++)
            {
                TraverseGameObject(target.transform.GetChild(i).gameObject, action);
            }
        }

        static bool CheckTargetInteractable(GameObject target)
        {
            return target.GetComponent<IEventSystemHandler>() != null;
        }

    }
}
