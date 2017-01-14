using UnityEngine;

public class WarpState : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GameState gs = GameObject.Find("GameState").GetComponent<GameState>();

        if (gs.GetWarpState(gameObject.name) == false) {
            Destroy(gameObject);
        }
    }
}