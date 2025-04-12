//using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rocket : Interactive
{
    // private void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.E))
    //     {
    //         Debug.Log(playerCollision);
    //         gameManager.ShowWinPanel();
    //         soundManager.PlaySound(SoundsManager.Sounds.Finish);
    //     }
    // }
    protected override void ActOnInteraction(InputAction.CallbackContext context)
    {
        if (playerCollision)
        {
            Debug.Log(playerCollision);
            GameManager.Instance.ActOnGameWon();
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.Finish);
        }
    }

    protected override void ActOnCollision()
    {
    }
}
