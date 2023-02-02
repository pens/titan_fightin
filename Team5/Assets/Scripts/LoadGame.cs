using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadGame : MonoBehaviour {

	public void OnClick()
    {
        SceneManager.LoadScene(1);
    }
}
