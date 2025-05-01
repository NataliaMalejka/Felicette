using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    private float startPosition;
    public GameObject cam;
    public float parallaxEffect;

    private void Start()
    {
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distanse = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPosition + distanse, transform.position.y, transform.position.z);

        if (temp > startPosition + length)
        {
            startPosition += length * 2;
        }
        else if (temp < startPosition - length)
        {
            startPosition -= length * 2;
        }
    }
}
