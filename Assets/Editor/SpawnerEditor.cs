using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class SpawnerEditor : Editor
{
    SerializedProperty EnemyCategories;

    private void OnEnable()
    {
        EnemyCategories = serializedObject.FindProperty(nameof(EnemyCategories));
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemySpawner spawner = (EnemySpawner)target;

        serializedObject.Update();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Check Category Weights"))
        {
            serializedObject.ApplyModifiedProperties();
            spawner.ValidateWeights();
        }

        if(GUILayout.Button("Reset Category Weights"))
        {
            spawner.ResetCategoryWeights();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Check Type Weights"))
        {
            spawner.CheckTypeWeights();
        }
        if(GUILayout.Button("Reset Type Weights"))
        {
            spawner.ResetTypeWeights();
        }
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
        //spawner.OnInspectorUpdate();
    }
}
