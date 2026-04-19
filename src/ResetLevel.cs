using UnityEngine;
using UnityEngine.SceneManagement;
public class ResetLevel : MonoBehaviour
{
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
