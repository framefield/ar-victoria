using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ff.vrflight.helpers
{
    /** A collection of helpers that improves editing and clean up of complex scenes */
    public class EditorSelectionHelpers : MonoBehaviour
    {
        [MenuItem("ff/Utils/Select/Select Parent Prefabs")]
        internal static void SelectParentPrefabs()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (var s in Selection.gameObjects)
            {
                var parentPrefab = PrefabUtility.FindValidUploadPrefabInstanceRoot(s);
                if (parentPrefab)
                {
                    newSelection.Add(parentPrefab);
                }
                else
                {
                    newSelection.Add(s);
                }
            }

            Selection.objects = newSelection.ToArray();
        }


        [MenuItem("ff/Utils/Select/Select Parent with LODs")]
        internal static void SelectParentsWithLOD()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (var s in Selection.gameObjects)
            {
                var parentPrefab = s.transform.GetComponentsInParent<LODGroup>(includeInactive: true);
                if (parentPrefab.Length > 0)
                {
                    newSelection.Add(parentPrefab[0].gameObject);
                }
                else
                {
                    newSelection.Add(s);
                }
            }

            Selection.objects = newSelection.ToArray();
        }


        [MenuItem("ff/Utils/Select/Select Empty Groups in Scene")]
        internal static void SelectEmpties()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (GameObject go in GetObjectsInActiveScene())
            {
                if (go.transform.childCount == 0)
                {
                    var count = go.GetComponents(typeof(Component)).Length;
                    if (count == 1)
                        newSelection.Add(go.gameObject);
                }
            }

            Selection.objects = newSelection.ToArray();
        }


        internal static List<GameObject> GetObjectsInActiveScene()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(t => t.GetComponentsInChildren<Transform>(true))
                .Select(t => t.gameObject).ToList();
        }


        [MenuItem("ff/Utils/Select/Select Objects with missing Meshes")]
        internal static void SelectMissingMesh()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (MeshFilter mf in root.GetComponentsInChildren<MeshFilter>(includeInactive: true))
                {
                    if (mf.sharedMesh == null)
                        newSelection.Add(mf.gameObject);
                }
            }

            Selection.objects = newSelection.ToArray();
        }

        [MenuItem("ff/Utils/Select/Select Objects With Odd negative scale")]
        internal static void SelectObjectsWithOddNegativeScaling()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (Transform mf in root.GetComponentsInChildren<Transform>(includeInactive: true))
                {

                    if (mf.localScale.x < 0 || mf.localScale.y < 0 || mf.localScale.z < 0)
                        newSelection.Add(mf.gameObject);
                }
            }

            Selection.objects = newSelection.ToArray();
        }


        [MenuItem("ff/Utils/Flip negative scale values")]
        internal static void FlipNegativeScaleValues()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (var go in Selection.gameObjects)
            {
                var mf = go.transform;
                if (mf.localScale.x >= 0 && mf.localScale.y >= 0 && mf.localScale.z >= 0)
                    continue;

                newSelection.Add(mf.gameObject);
                mf.localScale = new Vector3(
                    Mathf.Abs(mf.localScale.x),
                    Mathf.Abs(mf.localScale.y),
                    Mathf.Abs(mf.localScale.z)
                 );
            }
            Selection.objects = newSelection.ToArray();
        }


        [MenuItem("ff/Utils/Select/Select Nested Mesh Renderers")]
        internal static void SelectMeshesInMeshes()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (MeshRenderer mr in root.GetComponentsInChildren<MeshRenderer>(includeInactive: true))
                {
                    if (mr.transform.parent == null)
                        continue;

                    var renderersInParent = mr.transform.parent.GetComponentsInParent<MeshRenderer>(true);
                    foreach (var r in renderersInParent)
                    {
                        newSelection.Add(r.gameObject);
                    }
                }
            }

            Selection.objects = newSelection.ToArray();
        }

        [MenuItem("ff/Utils/Select/Static Renderers with GPU-instancing Material")]
        internal static void SelectStaticRenderesWithGPUInstancingMaterial()
        {
            var newSelection = new HashSet<GameObject>();
            foreach (var renderer in GetComponentsInAllScenes<MeshRenderer>())
            {
                if (renderer.gameObject.isStatic && renderer.sharedMaterial && renderer.sharedMaterial.enableInstancing)
                {
                    newSelection.Add(renderer.gameObject);
                }
            }

            Selection.objects = newSelection.ToArray();
            Debug.Log($"Selected {newSelection.Count} static objects with GPU-instancing");
        }

        private static List<T> GetComponentsInAllScenes<T>()
        {
            var result = new List<T>();
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                foreach (var root in scene.GetRootGameObjects())
                {
                    result.AddRange(root.GetComponentsInChildren<T>(includeInactive: true));
                }
            }
            return result;
        }


        [MenuItem("ff/Utils/Replace with prefabs (In active scene)")]
        internal static void ReplaceWithPrefabs()
        {
            var prefabNames = GetAllPrefabs();
            var pathsByName = new Dictionary<string, string>();
            foreach (var path in prefabNames)
            {
                var result = Regex.Match(path, @".*\/(.+?)\.prefab");
                if (result.Success)
                {
                    var nameOnly = result.Groups[1].Value;
                    if (pathsByName.ContainsKey(nameOnly))
                    {
                        Debug.LogWarning($"'{nameOnly}' is not unique");
                    }
                    else
                    {
                        pathsByName[nameOnly] = path;
                    }
                }
                else
                {
                    Debug.Log("invalid prefab path:" + path);
                }
            }

            var pathsByGameObject = new Dictionary<GameObject, string>();

            foreach (var go in Selection.gameObjects)
            {
                if (!go)
                    continue;

                // var parentPrefab = PrefabUtility.FindValidUploadPrefabInstanceRoot(go);
                // if (parentPrefab == go)
                //     continue;

                if (go.scene != SceneManager.GetActiveScene())
                {
                    Debug.LogWarning("Replacing only works for the active game scene...");
                    return;
                }

                var preciseMatches = pathsByName.Keys
                    .Where(stringToCheck => Regex.Match(stringToCheck, go.name).Success).ToArray();
                if (preciseMatches.Count() == 1)
                {
                    //Debug.Log($"'{go.name}' -> {preciseMatches.Count()}");
                    pathsByGameObject[go] = pathsByName[preciseMatches[0]];
                    continue;
                }

                var nameWithoutBraces = Regex.Replace(go.name, @"\s*\(\d+\)$", "");
                var prefabsWithoutBraces =
                    pathsByName.Keys.Where(stringToCheck => stringToCheck.Equals(nameWithoutBraces)).ToArray();
                if (prefabsWithoutBraces.Length == 1)
                {
                    Debug.Log($"replacing '{nameWithoutBraces}'...");
                    pathsByGameObject[go] = pathsByName[prefabsWithoutBraces.First()];
                    continue;
                }

                var nameWithoutCloneSuffix = Regex.Replace(nameWithoutBraces, @"\s*\(Clone\)$", "");
                var prefabsWithoutCloneSuffix =
                    pathsByName.Keys.Where(stringToCheck => stringToCheck.Equals(nameWithoutCloneSuffix)).ToArray();
                if (prefabsWithoutCloneSuffix.Length == 1)
                {
                    Debug.Log($"'{nameWithoutCloneSuffix}' -> {prefabsWithoutCloneSuffix.Length}");
                    pathsByGameObject[go] = pathsByName[prefabsWithoutCloneSuffix.First()];
                    continue;
                }

                Debug.Log($"No prefab found for {go.name} {nameWithoutBraces} {nameWithoutCloneSuffix}");
            }


            var newSelection = new List<GameObject>();

            foreach (var pair in pathsByGameObject)
            {
                var go2 = pair.Key;
                var prefabPath = pair.Value; //.Replace(".prefab", "");

                var loadedPrefab = AssetDatabase.LoadAssetAtPath<Transform>(prefabPath);

                if (!loadedPrefab)
                {
                    Debug.LogWarning($"Failed loading {prefabPath}");
                    continue;
                }


                var newInstance = (Transform)PrefabUtility.InstantiatePrefab(loadedPrefab);
                //GameObject newInstance = PrefabUtility.InstantiatePrefab(loadedPrefab as GameObject) as GameObject;
                //GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(loadedPrefab);
                newInstance.transform.SetParent(go2.transform.parent);
                newInstance.transform.localPosition = go2.transform.localPosition;
                newInstance.transform.localRotation = go2.transform.localRotation;
                newInstance.transform.localScale = go2.transform.localScale;
                newInstance.transform.SetSiblingIndex(go2.transform.GetSiblingIndex());
                newSelection.Add(newInstance.gameObject);
                DestroyImmediate(go2);
            }

            Selection.objects = newSelection.ToArray();
        }

        private static List<string> GetAllPrefabs()
        {
            var temp = AssetDatabase.GetAllAssetPaths();
            var result = new List<string>();
            foreach (var s in temp)
            {
                if (s.Contains(".prefab")) result.Add(s);
            }

            return result;
        }


        [MenuItem("ff/Utils/Reset prefab name")]
        internal static void ResetPrefabName()
        {
            var objects = Selection.gameObjects;

            var roots = objects.Select(PrefabUtility.FindPrefabRoot).Where(prefabRoot => prefabRoot != null)
                .Distinct();

            foreach (var root in roots)
            {
                var prefabObject = PrefabUtility.GetCorrespondingObjectFromSource(root);
                if (prefabObject)
                {
                    root.name = prefabObject.name;
                    EditorGUIUtility.PingObject(root);
                }
            }
        }

        [MenuItem("ff/Utils/Ungroup Objects")]
        internal static void Ungroup()
        {
            var objects = Selection.gameObjects;
            Undo.IncrementCurrentGroup();

            foreach (var root in objects)
            {
                var rootTransform = root.transform;
                var targetParent = rootTransform.parent;
                var targetIndex = rootTransform.GetSiblingIndex();

                var childCount = rootTransform.childCount;

                if (childCount == 0)
                {
                    continue;
                }

                // re-parent & keep world position
                for (var i = childCount - 1; i >= 0; i--)
                {
                    var childTransform = rootTransform.GetChild(i);
                    Undo.SetTransformParent(childTransform, targetParent, "Re-parent");
                    Undo.RecordObject(childTransform, "Apply index");
                    childTransform.SetSiblingIndex(targetIndex + 1);
                }

                Undo.DestroyObjectImmediate(root);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("ff/Utils/Apply transformation to children")]
        internal static void ApplyParentTransform()
        {
            var objects = Selection.gameObjects;
            Undo.IncrementCurrentGroup();

            foreach (var root in objects)
            {
                var rootTransform = root.transform;

                var tmpParent = new GameObject();
                var tmpParentTransform = tmpParent.transform;
                // move to correct scene
                tmpParentTransform.SetParent(rootTransform, false);
                // move to correct hierarchy level
                tmpParentTransform.SetParent(rootTransform.parent, false);
                // ensure everything is defaulted
                tmpParentTransform.localScale = Vector3.one;
                tmpParentTransform.localRotation = Quaternion.identity;
                tmpParentTransform.localPosition = Vector3.zero;

                var childCount = rootTransform.childCount;
                // move & keep world position
                for (var i = childCount - 1; i >= 0; i--)
                {
                    var childTransform = rootTransform.GetChild(i);
                    Undo.RecordObject(childTransform, "Apply transform 1");
                    childTransform.SetParent(tmpParentTransform, true);
                }

                // reset root
                Undo.RecordObject(rootTransform, "Apply root transform");
                rootTransform.localPosition = Vector3.zero;
                rootTransform.localRotation = Quaternion.identity;
                rootTransform.localScale = Vector3.one;
                EditorUtility.SetDirty(rootTransform);

                // move & keep world position
                for (var i = childCount - 1; i >= 0; i--)
                {
                    var childTransform = tmpParentTransform.GetChild(i);
                    Undo.RecordObject(childTransform, "Apply transform 2");
                    childTransform.SetParent(rootTransform, true);
                    EditorUtility.SetDirty(childTransform);
                }

                DestroyImmediate(tmpParent);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("ff/Utils/Flag as parent &#p")]
        internal static void MarkParent()
        {
            var newParent = Selection.activeGameObject;
            Debug.Log($"Setting parent to {newParent}", newParent);
            EditorGUIUtility.PingObject(newParent);
            SelectionToolsWindow._parentObject = newParent;
        }


        [MenuItem("ff/Utils/Select/Select parent &p")]
        internal static void SelectParent()
        {
            var currentObject = Selection.activeGameObject;
            if (!currentObject)
                return;

            var newParent = currentObject.transform.parent;
            if (!newParent)
                return;

            Selection.activeGameObject = newParent.gameObject;
            EditorGUIUtility.PingObject(newParent);
        }


        [MenuItem("ff/Utils/Move to parent &#m")]
        internal static void MoveSelectedToParent()
        {
            var movedElementCount = 0;
            var newParent = (GameObject)SelectionToolsWindow._parentObject;
            if (!newParent)
                return;

            Undo.IncrementCurrentGroup();

            foreach (var o in Selection.objects)
            {
                var go = o as GameObject;
                if (!go)
                    continue;

                if (go.transform.parent == newParent)
                    continue;

                //Undo.RegisterCompleteObjectUndo(s.transform, "set parent");
                Undo.SetTransformParent(go.transform, newParent.transform, "move parent");
                Undo.RegisterFullObjectHierarchyUndo(go, "move to parent");
                //s.transform.SetParent(newParent.transform);
                movedElementCount++;
            }

            Debug.Log($"Moved {movedElementCount} elements to {newParent}", newParent);
            EditorGUIUtility.PingObject(newParent);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }



        internal static void ReplaceSelection(Object replacementObject)
        {
            var newSelection = new List<GameObject>();

            foreach (var go in Selection.gameObjects)
            {
                if (!go)
                    continue;

                if (go.scene != SceneManager.GetActiveScene())
                {
                    Debug.LogWarning("Replacing only works for the active game scene...");
                    return;
                }

                string myPath = AssetDatabase.GetAssetPath(replacementObject);
                var loadedPrefab = AssetDatabase.LoadAssetAtPath<Transform>(myPath);

                if (!loadedPrefab)
                {
                    Debug.LogWarning($"Failed loading {myPath}");
                    continue;
                }

                var newInstance = (Transform)PrefabUtility.InstantiatePrefab(loadedPrefab);
                newInstance.transform.SetParent(go.transform.parent);
                newInstance.transform.localPosition = go.transform.localPosition;
                newInstance.transform.localRotation = go.transform.localRotation;
                newInstance.transform.localScale = go.transform.localScale;
                newInstance.transform.SetSiblingIndex(go.transform.GetSiblingIndex());
                newSelection.Add(newInstance.gameObject);
                DestroyImmediate(go);
            }
            Selection.objects = newSelection.ToArray();
        }



        [MenuItem("ff/Utils/Group Objects %G")]
        internal static void GroupObjects()
        {
            var currentObject = Selection.activeGameObject;
            if (!currentObject)
                return;

            Undo.IncrementCurrentGroup();

            var newParent = new GameObject
            {
                name = "Group"
            };
            Undo.RegisterCreatedObjectUndo(newParent, "create group");

            newParent.transform.SetParent(currentObject.transform.parent, false);

            SelectionToolsWindow._parentObject = newParent;
            MoveSelectedToParent();
            Selection.activeGameObject = newParent;
            EditorGUIUtility.PingObject(newParent);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("ff/Utils/Select/Select Overlapping Mesh Copies")]
        public static void SelectOverlappingMeshCopies()
        {
            // find all prefabs in active scene            
            var similarMeshes = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(t => t.GetComponentsInChildren<MeshFilter>(true))
                .GroupBy(GenerateSimilarMeshRendererHash)
                .Where(group => group.Count() > 1)
                .ToList();

            var selection = new List<Object>();
            foreach (var similarMeshGroup in similarMeshes)
            {
                // these are their own prefab or stand alone
                var standalones = similarMeshGroup
                    .Where(x =>
                        PrefabUtility.GetPrefabType(x.gameObject) == PrefabType.None ||
                        PrefabUtility.FindValidUploadPrefabInstanceRoot(x.gameObject) == x.gameObject)
                    .Select(mf => mf.gameObject).ToList();

                if (standalones.Count < similarMeshes.Count)
                {
                    selection.AddRange(standalones);
                }
                else
                {
                    selection.AddRange(standalones.GetRange(0, standalones.Count - 2));
                }
            }

            Selection.objects = selection.ToArray();
        }

        private static int GenerateSimilarMeshRendererHash(MeshFilter mf)
        {
            var hash = 17;
            hash = hash * 23 + (mf.sharedMesh ? mf.sharedMesh.GetInstanceID() : 0);
            hash = hash * 23 + GenerateSimilarTransformHash(mf.transform);
            return hash;
        }

        private static int GenerateSimilarTransformHash(Transform t)
        {
            var hash = 17;

            hash = hash * 23 + Math.Round(t.position.x, 2).GetHashCode();
            hash = hash * 23 + Math.Round(t.position.y, 2).GetHashCode();
            hash = hash * 23 + Math.Round(t.position.z, 2).GetHashCode();
            hash = hash * 23 + Math.Round(t.rotation.w, 2).GetHashCode();
            hash = hash * 23 + Math.Round(t.rotation.x, 2).GetHashCode();
            hash = hash * 23 + Math.Round(t.rotation.y, 2).GetHashCode();
            hash = hash * 23 + Math.Round(t.rotation.z, 2).GetHashCode();
            hash = hash * 23 + Math.Round(t.lossyScale.x, 1).GetHashCode();
            hash = hash * 23 + Math.Round(t.lossyScale.y, 1).GetHashCode();
            hash = hash * 23 + Math.Round(t.lossyScale.z, 1).GetHashCode();

            return hash;
        }
    }
}