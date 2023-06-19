using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class DistanceSlider : UISlider
    {


        /*
         * on mission start
         * min value will be 0
         * max value will be first boss distance
         * 
         */

        /*
         * onenable
         */

        private void OnEnable()
        {
            //min value = previous boss distance int
            //max value = next boss distance int
            //current value = distance float
        }

        private void OnDisable()
        {
            
        }
        private void Start()
        {
            
        }
    }
}
