//using System.Net.Sockets;
using UnityEngine;

public class Rocket : Interactive
{
    protected override void ActOnInteraction()
    {
        if (playerCollision)
        {
            GameManager.Instance.ActOnGameWon();
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.Finish);
        }
    }

    protected override void ActOnCollision()
    {
    }
}
