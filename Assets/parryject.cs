using UnityEngine;

public class parryject : MonoBehaviour
{
    [SerializeField] main play;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Respawn"))
        {
            play.Parry();
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}
