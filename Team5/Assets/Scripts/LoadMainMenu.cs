using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadMainMenu : MonoBehaviour {
    public void Return()
    {
        SceneManager.LoadScene(0);
    }
}
