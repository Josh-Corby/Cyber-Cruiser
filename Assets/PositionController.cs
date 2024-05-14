using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class PositionController : MonoBehaviour
    {
        [SerializeField] float screenwidth;
        [SerializeField] float screenheight;
        [SerializeField] Vector2 screenposition;

        void Start()
        {
            screenwidth = Screen.width;
            screenheight = Screen.height;

            Vector2 newPosition;
            newPosition.x = Mathf.Lerp(0f, screenwidth, screenposition.x);
            newPosition.y = Mathf.Lerp(0f, screenheight, screenposition.y);

            newPosition = Camera.main.ScreenToWorldPoint(newPosition);
            transform.position = newPosition;
        }

    }
}
