using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {

    private void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            Quit();
        }
    }

    public void SceneLoader(int index) {
        SceneManager.LoadScene(index);
    }

    public void Quit() {
        Application.Quit();
    }
}
