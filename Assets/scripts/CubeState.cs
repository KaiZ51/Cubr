using System.Collections.Generic;
using UnityEngine;

public class CubeState : MonoBehaviour {
    private GameState gs;
    private SpriteRenderer cubeSprite;
    private Sprite[] sprites;

    // Use this for initialization
    void Start() {
        gs = GameObject.Find("GameState").GetComponent<GameState>();

        List<string> paintedCubes = gs.GetPaintedCubes();
        if (paintedCubes != null && paintedCubes.Exists(element => element == gameObject.name)) {
            ChangeColor(true);
        }
    }

    public void ChangeColor(bool resetLevel) {
        sprites = Resources.LoadAll<Sprite>("cube");
        cubeSprite = GetComponent<SpriteRenderer>();

        if (cubeSprite.sprite != sprites[1] && resetLevel == false) {
            cubeSprite.sprite = sprites[1];
            gs.UpdateScore();
        }
        else {
            cubeSprite.sprite = sprites[1];
        }
    }
}