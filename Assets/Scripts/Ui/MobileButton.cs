using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class MobileButton : Button
{
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnButtonPressed?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        OnButtonReleased?.Invoke();
    }

    public void OnInteractionPossibilityChanged(bool canInteract)
    {
        if (canInteract)
        {
            GetComponent<Image>().color = Color.white; //new Color(0.4f, 0.8f, 0.4f);
            enabled = true;
        }
        else
        {
            GetComponent<Image>().color = new Color(0.34f, 0.34f, 0.34f, 1f);//Color.white;
            enabled = false;
        }
    }
}