using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {
    public static GameState gs;

    private string playerCubeName;
    private Text scoreText;
    private int score;
    private Text livesText;
    private int lives = 3;

    private GameObject[] cubes;
    private SpriteRenderer cubeSprite;
    private Sprite[] spriteAssets;
    private bool[] checkGoal;
    private List<string> paintedCubes;
    private List<string> warpStates;

    private bool gameOver;
    private bool pause;

    void Awake() {
        if (gs == null) {
            DontDestroyOnLoad(gameObject);
            gs = this;
        }
        else {
            if (gs != this) {
                Destroy(gameObject);
            }
        }

        if (warpStates == null) {
            warpStates = new List<string>();

            foreach (GameObject warp in GameObject.FindGameObjectsWithTag("Warp")) {
                warpStates.Add(warp.name);
            }
        }
    }

    void Start() {
        if (paintedCubes == null) {
            paintedCubes = new List<string>();
        }
    }

    public int GetScore() {
        return score;
    }

    public int GetLives() {
        return lives;
    }

    public void UpdateScore() {
        scoreText = GameObject.Find("Canvas/score_count").GetComponent<Text>();

        score = int.Parse(scoreText.text) + 50;
        scoreText.text = score.ToString();
        CheckCubeStates();
    }

    public void UpdateLives() {
        livesText = GameObject.Find("Canvas/lives_count").GetComponent<Text>();

        lives--;
        livesText.text = lives.ToString();

        if (lives >= 0) {
            PlayerController pc = GameObject.Find("player").GetComponent<PlayerController>();
            playerCubeName = pc.GetCurrentCube().name;

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

            pause = false;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
        else {
            SceneManager.LoadScene("gameover", LoadSceneMode.Additive);
            StartCoroutine("SetGameOver", true);
        }
    }

    void CheckCubeStates() {
        cubes = GameObject.FindGameObjectsWithTag("Cubes");
        checkGoal = new bool[cubes.Length];

        for (int i = 0; i < cubes.Length; i++) {
            cubeSprite = cubes[i].GetComponent<SpriteRenderer>();
            spriteAssets = Resources.LoadAll<Sprite>("cube");

            if (cubeSprite.sprite == spriteAssets[1]) {
                checkGoal[i] = true;
                paintedCubes.Add(cubes[i].name);
            }
            else {
                checkGoal[i] = false;
            }
        }

        if (checkGoal.All(element => element.Equals(true))) {
            bool sceneExists = false;

            for (int i = 0; i < SceneManager.sceneCount; i++) {
                switch (SceneManager.GetSceneAt(i).name) {
                    case "level":
                        sceneExists = true;
                        break;
                    case "level2":
                        sceneExists = true;
                        break;
                }
            }

            if (sceneExists) {
                SceneManager.LoadScene("nextlevel", LoadSceneMode.Additive);
                StartCoroutine("SetGameOver", true);
            }
            else {
                SceneManager.LoadScene("gameover", LoadSceneMode.Additive);
                StartCoroutine("SetGameOver", true);
            }
        }
    }

    public List<string> GetPaintedCubes() {
        return paintedCubes;
    }

    public string GetPlayerCubeName() {
        return playerCubeName;
    }

    public bool GetWarpState(string warpName) {
        if (warpStates.Exists(element => element.Equals(warpName))) {
            return true;
        }
        else {
            return false;
        }
    }

    public void RemoveWarp(string warpName) {
        warpStates.Remove(warpStates.Find(element => element.Equals(warpName)));
    }

    public bool GetGameOver() {
        return gameOver;
    }

    public IEnumerator SetGameOver(bool state) {
        if (state) {
            gameOver = true;

            yield return new WaitForSeconds(.1f);

            GameObject resultScoreUI = GameObject.Find("Canvas/gameover_panel/result_score");
            Text resultScore = resultScoreUI.GetComponent<Text>();
            resultScore.text = "Your score is: " + scoreText.text + ".";
        }
        else {
            gameOver = false;
        }
    }

    public bool GetPause() {
        return pause;
    }

    public void SetPause(bool state) {
        pause = state;
    }
}