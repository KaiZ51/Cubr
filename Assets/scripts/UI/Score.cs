using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    void Awake() {
        GameState gs = GameObject.Find("GameState").GetComponent<GameState>();
        Text scoreText = GetComponent<Text>();
        scoreText.text = gs.GetScore().ToString();
    }
}