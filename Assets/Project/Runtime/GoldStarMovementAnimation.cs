
namespace CyberCruiser
{
    public class GoldStarMovementAnimation : GameBehaviour
    {
        StarAnimation _goldStar;

        private void Awake()
        {
            _goldStar = GetComponentInParent<StarAnimation>();
        }

        private void OnStarMovementAnimationFinish()
        {
            _goldStar.PlayEffectAnimation();
        }
    }
}
