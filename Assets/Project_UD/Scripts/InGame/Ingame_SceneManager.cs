using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ingame_SceneManager : MonoBehaviour
{
    public void _nextScene(int sceneIdx)
    {
        if (sceneIdx >= 0)
        {
            SceneManager.LoadSceneAsync(sceneIdx);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
