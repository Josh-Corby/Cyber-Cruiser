using UnityEditor;
using UnityEngine;

namespace CyberCruiser
{
    [CustomEditor(typeof(EnemySpawner))]
    public class SpawnerEditor : Editor
    {
        SerializedProperty enemyCategories;
        private void OnEnable()
        {
            enemyCategories = serializedObject.FindProperty(nameof(enemyCategories));
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EnemySpawner spawner = (EnemySpawner)target;

            serializedObject.Update();

            if (GUILayout.Button("Reset Category Weights"))
            {
                spawner.ResetCategoryWeights();
            }

            if (GUILayout.Button("Reset Type Weights"))
            {
                spawner.ResetTypeWeights();
            }

            spawner.OnInspectorUpdate();
            serializedObject.ApplyModifiedProperties();
        }
    }
}