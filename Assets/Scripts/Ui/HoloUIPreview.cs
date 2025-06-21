using UnityEngine;
using UnityEngine.UI;

public class HoloUIPreview : MonoBehaviour
{
    // [SerializeField] private Material holoMaterial;
    [SerializeField] private Image previewImage;
    [SerializeField] private Image previewChildImage;

    public void ActivateHoloPreview(Sprite previewSprite, Sprite previewChildSprite = null, float rotation = 0f)
    {
        previewImage.sprite = previewSprite;
        previewImage.transform.localRotation = Quaternion.Euler(0, 0, rotation);

        if (previewChildSprite != null)
        {
            previewChildImage.gameObject.SetActive(true);
            previewChildImage.sprite = previewChildSprite;
            // previewChildImage.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        }
        else
        {
            previewChildImage.gameObject.SetActive(false);
        }
    }

    public void UpdateHoloPreview(bool status)
    {

    }

    public void DeactivateHoloPreview()
    {
        previewImage.sprite = null;

        if (previewChildImage != null)
        {
            previewChildImage.sprite = null;
        }
    }
}
