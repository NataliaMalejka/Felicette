using UnityEngine.VFX;
using UnityEngine;

public class Rocket : Interactive
{
    [SerializeField] private VisualEffect trailEffect;
    [SerializeField] private Animator flyAnimator;

    protected override void Start()
    {
        base.Start();
        transform.rotation = Quaternion.identity;
    }

    protected override void ActOnInteraction()
    {
        if (playerCollision)
        {
            trailEffect?.Play();
            player.HideSprite();
            flyAnimator?.SetTrigger("Fly");
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.RocketLaunch);
            GameManager.Instance.ActOnGameWon();
        }
    }

    protected override void ActOnCollision()
    {
    }
}
