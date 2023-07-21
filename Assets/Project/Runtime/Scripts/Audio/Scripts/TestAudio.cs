using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class TestAudio : GameBehaviour
        {
            public AudioController audioController;


            //example audio call
            private void Start()
            {
                audioController.PlayAudio(AudioType.ST_01);
            }
        }

        
    }
}