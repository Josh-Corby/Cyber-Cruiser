using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CyberCruiser
{

    public class inputtest : GameBehaviour
    {
        [SerializeField] private PlayerShipController controller;
        [SerializeField] TMP_Text text;


        // Update is called once per frame
        void Update()
        {
            Vector2 inputvector = controller.GetInput;
            if(Mathf.Abs(inputvector.x) > 0 || Mathf.Abs(inputvector.y) > 0)
            {
            text.text = controller.GetInput.ToString();

            }
        }
    }
}
