using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void LoadLevelManager()
    {
        SceneManager.LoadScene(1);
    }
}
