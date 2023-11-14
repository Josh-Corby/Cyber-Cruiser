using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerSpriteController : MonoBehaviour
    {
        [SerializeField] private GameObject _sprites;
        private void OnEnable()
        {
            EnableSprites();
        }

        private void EnableSprites()
        {
            _sprites.SetActive(true);
        }

        public void DisableSprites()
        {
            _sprites.SetActive(false);
        }
    }
}
