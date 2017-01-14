using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour {
    void Awake() {
        GameState gs = GameObject.Find("GameState").GetComponent<GameState>();
        Text livesText = GetComponent<Text>();
        livesText.text = gs.GetLives().ToString();
    }
}