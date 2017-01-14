using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public void StartGame() {
        SceneManager.LoadScene("level", LoadSceneMode.Single);
    }

    public void About() {
        SceneManager.LoadScene("about", LoadSceneMode.Additive);
    }

    public void NextLevel() {
        StartCoroutine("WaitForSceneLoad");
    }

    IEnumerator WaitForSceneLoad() {
        GameObject gs = GameObject.Find("GameState");
        AsyncOperation load;

        for (int i = 0; i < SceneManager.sceneCount; i++) {
            switch (SceneManager.GetSceneAt(i).name) {
                case "level":
                    Destroy(gs);
                    load = SceneManager.LoadSceneAsync("level2", LoadSceneMode.Single);

                    while (load.isDone == false) {
                        yield return null;
                    }

                    break;
                case "level2":
                    Destroy(gs);
                    load = SceneManager.LoadSceneAsync("level3", LoadSceneMode.Single);

                    while (load.isDone == false) {
                        yield return null;
                    }

                    break;
            }
        }
    }

    public void RestartLevel() {
        string scene = "";

        for (int i = 0; i < SceneManager.sceneCount; i++) {
            switch (SceneManager.GetSceneAt(i).name) {
                case "level":
                    scene = "level";
                    break;
                case "level2":
                    scene = "level2";
                    break;
                case "level3":
                    scene = "level3";
                    break;
            }
        }

        GameObject gs = GameObject.Find("GameState");
        Destroy(gs);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void BackToMenu() {
        GameObject gs = GameObject.Find("GameState");
        Destroy(gs);
        SceneManager.LoadScene("menu", LoadSceneMode.Single);
    }

    public void BackToMenuAbout() {
        SceneManager.UnloadSceneAsync("about");
    }

    public void PauseGame() {
        GameState gs = GameObject.Find("GameState").GetComponent<GameState>();

        if (gs.GetPause() == false) {
            gs.SetPause(true);
            SceneManager.LoadScene("pause", LoadSceneMode.Additive);
        }
        else {
            SceneManager.UnloadSceneAsync("pause");
            gs.SetPause(false);
        }
    }
}