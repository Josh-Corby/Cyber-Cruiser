
namespace CyberCruiser
{
    public class StarMovementAnimation : GameBehaviour
    {
        StarAnimation _goldStar;

        private void Awake()
        {
            _goldStar = GetComponentInParent<StarAnimation>();
        }

        public void OnStarMovementAnimationFinish()
        {
            _goldStar.PlayEffectAnimation();
        }
    }
}
