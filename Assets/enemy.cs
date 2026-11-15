using UnityEngine;
public class enemy : MonoBehaviour
{
    private main controller;
    private Vector3 initpos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<main>();
        initpos = transform.position;
    }
    void Update()
    {
        if (controller.respawning)
        {
            transform.position = initpos;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            if (controller.boosted)
            {
                Debug.Log("hitdown");
                controller.bounces = 10;
                transform.Translate(0, -15, 0);
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 100, ForceMode2D.Impulse);
            }
            else
            {
                controller.Respawn();
            }
        }
    }
}
