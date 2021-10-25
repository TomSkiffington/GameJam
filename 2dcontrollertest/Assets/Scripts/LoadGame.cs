using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        SceneManager.LoadSceneAsync(2);
        SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(4, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(5, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(6, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(7, LoadSceneMode.Additive);

        //SceneManager.UnloadSceneAsync(1);
    }

}
