using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    private Vector3 startPos;
    public float scrollSpeed = 2f; 
    public float parallaxEffect = 0.5f; 

    private void Start()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float movement = scrollSpeed * parallaxEffect * Time.deltaTime;
        transform.position -= new Vector3(movement, 0f, 0f);

        if (transform.position.x < startPos.x - length)
        {
            transform.position += new Vector3(length * 3f, 0f, 0f);
        }
    }
}
