
using UnityEngine;
using UnityEngine.Events;

public class AnimatorEvent : MonoBehaviour
{
    public UnityEvent OnWallAnimEnd;
    public UnityEvent OnFloorAnimEnd;
    
    public void TriggerOnFloorAnimEnd()
    {
        OnFloorAnimEnd?.Invoke();
    }

    public void TriggerOnWallAnimEnd()
    {
        OnWallAnimEnd?.Invoke();
    }
}