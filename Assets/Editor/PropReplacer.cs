using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropReplacer : EditorWindow
{
    [SerializeField]
	GameObject prefab;

    [MenuItem("Tools/Replace Props")]

    static void CreatePropReplacer()
    {
        EditorWindow.GetWindow<PropReplacer>();
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            for (int i = selectedObjects.Length - 1; i >= 0; --i)
            {
                GameObject selectedObject = selectedObjects[i];
                GameObject newObject;

				PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefab);

                if
				(
					prefabType == PrefabAssetType.Regular ||
					prefabType == PrefabAssetType.Variant
				)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    newObject = Instantiate(prefab);

                    newObject.name = prefab.name;
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");

                newObject.transform.parent = selectedObject.transform.parent;
                newObject.transform.localPosition = selectedObject.transform.localPosition;
                newObject.transform.localRotation = selectedObject.transform.localRotation;
                newObject.transform.localScale = selectedObject.transform.localScale;
                newObject.transform.SetSiblingIndex(selectedObject.transform.GetSiblingIndex());

                Undo.DestroyObjectImmediate(selectedObject);
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}
