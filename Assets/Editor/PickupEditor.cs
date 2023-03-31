using UnityEditor;

[CustomEditor(typeof(Pickup))]
public class PickupEditor : Editor
{
    #region SerializedProperties

    SerializedProperty _pickupType;
    SerializedProperty _upgradeType;
    SerializedProperty _speed;
    SerializedProperty _plasmaAmount;
    SerializedProperty _healthAmount;
    SerializedProperty _upgradeDuration;
    #endregion

    private void OnEnable()
    {
        _pickupType = serializedObject.FindProperty(nameof(_pickupType));
        _upgradeType = serializedObject.FindProperty(nameof(_upgradeType));
        _speed = serializedObject.FindProperty(nameof(_speed));
        _plasmaAmount = serializedObject.FindProperty(nameof(_plasmaAmount));
        _healthAmount = serializedObject.FindProperty(nameof(_healthAmount));
        _upgradeDuration = serializedObject.FindProperty(nameof(_upgradeDuration));
    }

    public override void OnInspectorGUI()
    {
        Pickup pickup = (Pickup)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(_pickupType);
        EditorGUILayout.PropertyField(_speed);
        switch (pickup._pickupType)
        {
            case PickupType.Plasma:
                EditorGUILayout.PropertyField(_plasmaAmount);
                break;
            case PickupType.Health:
                EditorGUILayout.PropertyField(_healthAmount);
                break;
            case PickupType.Weapon:
                EditorGUILayout.PropertyField(_upgradeType);
                EditorGUILayout.PropertyField(_upgradeDuration);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
