using UnityEngine;


public class BeamAttack : MonoBehaviour
{

    public LineRenderer lineRenderer;
    private float beamSize;
    [SerializeField] private float beamSpeed;

    public float beamDuration;
    [SerializeField] private float beamTimer;



    private void OnEnable()
    {
        ResetBeam();
    }

    private void OnDisable()
    {
        ResetBeam();
    }


    private void Update()
    {
        beamTimer -= Time.deltaTime;

        if (beamTimer > 0)
        {
            beamSize += beamSpeed * Time.deltaTime;
            lineRenderer.SetPosition(1, transform.localPosition + transform.right * beamSize);
        }
        if(beamTimer <= 0)
        {
            lineRenderer.enabled = false;
        }

    }

    public void ResetBeam()
    {
        lineRenderer.SetPosition(0, transform.localPosition);
        lineRenderer.SetPosition(1, transform.localPosition);
        beamSize = 0;
        beamTimer = beamDuration;
    }
}
