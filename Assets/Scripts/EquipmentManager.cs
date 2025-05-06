using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class SlotPair
{
    public GameObject key;
    public bool value;
}

public class EquipmentManager : MonoBehaviour
{
 
   [SerializeField]
    private List<SlotPair> slotsDict = new List<SlotPair>();
    //TRUE - SLOT IS FREE, FALSE - SLOT IS OCCUPIED
    private Dictionary<GameObject, bool> eqSlots = new Dictionary<GameObject, bool>();
    private GraphicRaycaster raycaster;
    private GameObject targetSlot = null;

    void Awake()
    {
        foreach (var pair in slotsDict)
        {
            eqSlots[pair.key] = pair.value;
        }
    }

    public void SetGraphicRaycaster(GraphicRaycaster graphicRaycaster)
    {
        raycaster = graphicRaycaster;
    }

    public bool CheckIfPointHoversOverSlot(Vector3 point, bool checkForEmpty, bool previousTargetSlot = false)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Camera.main.WorldToScreenPoint(point)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (eqSlots.ContainsKey(result.gameObject) &&
            ((checkForEmpty && eqSlots[result.gameObject]) ||
             (!checkForEmpty && !eqSlots[result.gameObject])))
            {
                if (previousTargetSlot && targetSlot != null)
                {
                    eqSlots[targetSlot] = true;
                }
                targetSlot = result.gameObject;
                return true;
            }
        }
        return false;
    }

    public Vector3 SetTargetSlotAsOccupied(Vector3 defaultPos, float zPosition = 0f)
    {
        if (targetSlot != null)
        {
            Vector3 slotPosition = Camera.main.ScreenToWorldPoint(targetSlot.transform.position);
            eqSlots[targetSlot] = false;
            targetSlot = null;
            return new Vector3(slotPosition.x, slotPosition.y, zPosition);
        }
        return defaultPos;
    }

    public void SetTargetSlotAsFree()
    {
        if (targetSlot != null)
        {
            eqSlots[targetSlot] = true;
            targetSlot = null;
        }
    }
}
