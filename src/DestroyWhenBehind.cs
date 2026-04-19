using UnityEngine;

public class DestroyWhenBehind : MonoBehaviour
{
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < player.transform.position.z - 10f)
        {
            Destroy(gameObject);
        }
    }
}
