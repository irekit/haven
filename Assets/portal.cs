using UnityEngine;
using UnityEngine.SceneManagement;
public class portal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D co)
    {
        if(co.CompareTag("Player"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
