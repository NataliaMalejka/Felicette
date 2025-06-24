using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimatorEvent : MonoBehaviour
{
    public UnityEvent OnTpAnim;
    
    public void TriggerOnTpAnim()
    {
        OnTpAnim?.Invoke();
    }
}