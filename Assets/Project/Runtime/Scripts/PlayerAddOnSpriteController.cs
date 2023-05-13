using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddOnSpriteController : GameBehaviour
{
    [SerializeField] private PlayerAddOnManager _addOnManager;
    [SerializeField] private List<AddOnSprite> _addOnSprites = new();

    private void OnEnable()
    {
        GameManager.OnMissionStart += SetAddOnSprites;
    }

    private void OnDisable()
    {
        GameManager.OnMissionStart -= SetAddOnSprites;
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

[Serializable]
public class AddOnSprite
{
    public string Name { get; }
    public SpriteRenderer SpriteRenderer { get; set; }
}