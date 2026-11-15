using UnityEngine;

public class parryject : MonoBehaviour
{
    [SerializeField] main play;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Respawn"))
        {
            if(col.GetComponent<boss>() != null)
            {
                col.GetComponent<boss>().Parried();
            }
            play.Parry();
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}
