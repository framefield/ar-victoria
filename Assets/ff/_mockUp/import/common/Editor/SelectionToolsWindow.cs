using System.Collections.Generic;
using System.Text.RegularExpressions;
//using ff.vrflight.gameplay.gameStructure;
using UnityEditor;
using UnityEngine;

namespace ff.vrflight.helpers
{
    public class SelectionToolsWindow : EditorWindow
    {
        [MenuItem("VR-Flight/Selection Tools")]
        private static void Init()
        {
            GetWindow<SelectionToolsWindow>("Selection", true);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += SelectionChangedHandler;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= SelectionChangedHandler;
        }

        private void SelectionChangedHandler()
        {
            _needsUpdate = true;
        }

        void Update()
        {
            if (!_needsUpdate)
                return;

            UpdateSelection();
            _needsUpdate = false;
        }

        void OnGUI()
        {
            var newSearchString = EditorGUILayout.TextField("Search", _searchString);
            EditorGUILayout.BeginHorizontal();
            var newReplaceString = EditorGUILayout.TextField("Replace", _replaceString);
            if (GUILayout.Button("Rename", EditorStyles.miniButtonRight))
            {
                foreach (var o in Selection.objects)
                {
                    var newName = Regex.Replace(o.name, _searchString, _replaceString);
                    Debug.Log($"renaming '{o.name}' -> '{newName}'");

                    o.name = newName;
                    Undo.RegisterCompleteObjectUndo(o, "Rename");
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Limit selection to...", EditorStyles.boldLabel);
            var newSelectLODGroups = EditorGUILayout.Toggle("LOD-Groups", _selectLODGroups);
            var newSelectPrefabGroups = EditorGUILayout.Toggle("Prefab Groups", _selectPrefabGroups);

            // --- Parenting ------------------------
            GUILayout.Label("Parent Object", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set", EditorStyles.miniButtonLeft))
            {
                _parentObject = Selection.activeGameObject;
            }

            _parentObject = EditorGUILayout.ObjectField(_parentObject, typeof(Object), true);
            if (GUILayout.Button("<- Reparent " + Selection.objects.Length, EditorStyles.miniButtonLeft))
            {
                EditorSelectionHelpers.MoveSelectedToParent();
            }

            EditorGUILayout.EndHorizontal();

            // --- Problems -----------------
            {

                GUILayout.Label("Find Problems...", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Empties", EditorStyles.miniButtonLeft))
                {
                    EditorSelectionHelpers.SelectEmpties();
                }

                if (GUILayout.Button("Missing meshes", EditorStyles.miniButtonLeft))
                {
                    EditorSelectionHelpers.SelectMissingMesh();
                }

                if (GUILayout.Button("Nested Meshes", EditorStyles.miniButtonLeft))
                {
                    EditorSelectionHelpers.SelectMeshesInMeshes();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Mesh Copy Overlapping", EditorStyles.miniButtonLeft))
                {
                    EditorSelectionHelpers.SelectOverlappingMeshCopies();
                }

                EditorGUILayout.EndHorizontal();
            }


            // ---- replace selection  ---------------------------------------
            {
                GUILayout.Label($"Replace {Selection.objects.Length} selected objects with... ", EditorStyles.boldLabel);
                _replacementObject = EditorGUILayout.ObjectField(_replacementObject, typeof(Object), true);
                if (GUILayout.Button("Replace", EditorStyles.miniButtonLeft))
                {
                    EditorSelectionHelpers.ReplaceSelection(_replacementObject);
                }
            }

            _needsUpdate = _searchString != newSearchString
                           || _selectLODGroups != newSelectLODGroups
                           || _selectPrefabGroups != newSelectPrefabGroups
                           || _searchString != newSearchString;

            _searchString = newSearchString;
            _replaceString = newReplaceString;
            _selectLODGroups = newSelectLODGroups;
            _selectPrefabGroups = newSelectPrefabGroups;
        }

        void UpdateSelection()
        {
            if (_searchString != "")
            {
                var newSelection = new List<GameObject>();
                foreach (var go in EditorSelectionHelpers.GetObjectsInActiveScene())
                {
                    if (Regex.Match(go.name, _searchString).Success)
                    {
                        newSelection.Add(go);
                    }
                }

                Selection.objects = newSelection.ToArray();
            }

            if (!Selection.activeGameObject)
                return;

            if (_selectLODGroups)
                EditorSelectionHelpers.SelectParentsWithLOD();

            if (_selectPrefabGroups)
                EditorSelectionHelpers.SelectParentPrefabs();
        }

        string _searchString = "";
        string _replaceString = "";
        bool _selectLODGroups = false;
        bool _selectPrefabGroups = false;
        bool _needsUpdate = false;

        internal static Object _parentObject;
        internal static Object _replacementObject;
    }
}