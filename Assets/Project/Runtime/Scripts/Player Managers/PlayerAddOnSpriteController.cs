using System.Collections.Generic;
using UnityEngine;

public class PlayerAddOnSpriteController : GameBehaviour
{
    [SerializeField] private PlayerAddOnManager _addOnManager;
    [SerializeField] private List<AddOnSprite> _addOnSprites = new();

    private void OnEnable()
    {
        SetAddOnSprites();
    }

    private void SetAddOnSprites()
    {
        for (int i = 0; i < _addOnSprites.Count; i++)
        {
            bool enabled = _addOnManager.AddOnActiveStates[i].IsAddOnActive;
            _addOnSprites[i].SpriteRenderer.enabled = enabled;
        }
    }
}

[System.Serializable]
public class AddOnSprite
{
    public string Name;
    public SpriteRenderer SpriteRenderer;
}