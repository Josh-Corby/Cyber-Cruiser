using UnityEditor;

[CustomEditor(typeof(Pickup))]
public class PickupEditor : Editor
{
    #region SerializedProperties

    SerializedProperty _pickupType;
    SerializedProperty _upgradeType;
    SerializedProperty _speed;
    SerializedProperty _healthAmount;
    SerializedProperty _plasmaAmount;
    SerializedProperty _ionAmount;
    #endregion

    private void OnEnable()
    {
        _pickupType = serializedObject.FindProperty(nameof(_pickupType));
        _upgradeType = serializedObject.FindProperty(nameof(_upgradeType));
        _speed = serializedObject.FindProperty(nameof(_speed));
        _healthAmount = serializedObject.FindProperty(nameof(_healthAmount));
        _plasmaAmount = serializedObject.FindProperty(nameof(_plasmaAmount));
        _ionAmount = serializedObject.FindProperty(nameof(_ionAmount));
    }

    public override void OnInspectorGUI()
    {
        Pickup pickup = (Pickup)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(_pickupType);
        EditorGUILayout.PropertyField(_speed);
        switch (pickup._pickupType)
        {
            case PickupType.Health:
            case PickupType.Plasma:
                EditorGUILayout.PropertyField(_healthAmount);
                EditorGUILayout.PropertyField(_plasmaAmount);
                EditorGUILayout.PropertyField(_ionAmount);
                break;
            case PickupType.Weapon:
                EditorGUILayout.PropertyField(_upgradeType);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
