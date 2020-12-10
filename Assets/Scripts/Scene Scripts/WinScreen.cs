using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public void PlayAgain() {
        SceneManager.LoadScene("PlayerManager Playground");
    }

    public void Quit() {
        Application.Quit();
    }
}
