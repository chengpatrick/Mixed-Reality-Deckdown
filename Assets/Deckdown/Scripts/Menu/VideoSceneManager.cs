using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoSceneManager : MonoBehaviour
{
    private void Start()
    {
        Invoke("NextScene", 120f);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(1);
    }
}
