using UnityEngine;
using System.Collections.Generic;
using CnControls;
using System.Collections;

public class PlayerController : MonoBehaviour {
    // player variables
    private const float speed = 1.25f;
    private Vector2 targetPos;
    private Transform tr;
    private const float placeOnTop = 0.45f;
    private bool moved;
    private CircleCollider2D cc2d;
    private Animator anim;
    private AudioSource[] audioSources;
    private bool coroutineActive;

    // current and adjacent cube variables
    public GameObject currentCube;
    private CubeState cubeState;
    private AdjacentCubes currentCubeAdj;
    private Dictionary<string, GameObject> adjacentCubes;

    // warp variables
    private bool onWarp;
    private string warpPos;
    private bool warpState;

    // direction variables
    /*private float distanceUpDownX = 0.2346f;
    private float distanceUpDownY = 0.29432f;
    private float distanceLeftRightX = 0.2254f;
    private float distanceLeftRightY = -0.29432f;*/

    private GameState gameState;

    // Use this for initialization
    void Start() {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        string playerCubeName = gameState.GetPlayerCubeName();
        GameObject playerCube = GameObject.Find(playerCubeName);

        if (gameState.GetPlayerCubeName() == null) {
            transform.position = new Vector2(currentCube.transform.position.x, currentCube.transform.position.y + placeOnTop);
        }
        else {
            currentCube = playerCube;
            transform.position = new Vector2(playerCube.transform.position.x, playerCube.transform.position.y + placeOnTop);
        }

        targetPos = transform.position;
        tr = transform;
        cc2d = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (gameState.GetGameOver() == false && gameState.GetPause() == false && onWarp == false) {
            if (coroutineActive == false) {
                StartCoroutine("DoMovement");
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
        }
        else if (gameState.GetGameOver() == false && onWarp) {
            if (warpState == false) {
                cc2d.enabled = false;
                targetPos = new Vector2(adjacentCubes[warpPos].transform.position.x, adjacentCubes[warpPos].transform.position.y + placeOnTop);

                if ((Vector2)tr.position == targetPos) {
                    warpState = true;
                }
            }
            else if (warpState) {
                currentCube = GameObject.Find("cube");
                targetPos = new Vector2(currentCube.transform.position.x, currentCube.transform.position.y + placeOnTop);

                if (warpPos == "left") {
                    adjacentCubes[warpPos].transform.position = Vector2.MoveTowards(adjacentCubes[warpPos].transform.position,
                        new Vector2(transform.position.x + 0.2f, transform.position.y - 0.3f),
                        Time.deltaTime * speed);
                }
                else {
                    adjacentCubes[warpPos].transform.position = Vector2.MoveTowards(adjacentCubes[warpPos].transform.position,
                        new Vector2(transform.position.x - 0.2f, transform.position.y - 0.3f),
                        Time.deltaTime * speed);
                }

                if ((Vector2)tr.position == targetPos) {
                    cc2d.enabled = true;
                    gameState.RemoveWarp(adjacentCubes[warpPos].name);
                    Destroy(adjacentCubes[warpPos]);
                    warpState = false;
                    onWarp = false;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        StartCoroutine("WaitForLives");
    }

    public GameObject GetCurrentCube() {
        return currentCube;
    }

    IEnumerator DoMovement() {
        coroutineActive = true;

        cubeState = currentCube.GetComponent<CubeState>();
        currentCubeAdj = currentCube.GetComponent<AdjacentCubes>();
        adjacentCubes = currentCubeAdj.GetAdjacents();

        if ((Vector2)tr.position == targetPos && moved) {
            cubeState.ChangeColor(false);
            anim.SetInteger("state", 0);
            yield return new WaitForSeconds(0.3f);
        }

        if (CnInputManager.GetAxis("Horizontal") > 0 && CnInputManager.GetAxis("Vertical") < 0 && (Vector2)tr.position == targetPos && adjacentCubes.ContainsKey("right")) {
            if (moved == false) {
                moved = true;
            }

            currentCube = adjacentCubes["right"];
            targetPos = new Vector2(currentCube.transform.position.x, currentCube.transform.position.y + placeOnTop);
            anim.SetInteger("state", 2);
            audioSources[0].Play();
        }
        else if (CnInputManager.GetAxis("Horizontal") < 0 && CnInputManager.GetAxis("Vertical") > 0 && (Vector2)tr.position == targetPos && adjacentCubes.ContainsKey("left")) {
            if (moved == false) {
                moved = true;
            }

            if (adjacentCubes["left"] != null) {
                if (!adjacentCubes["left"].CompareTag("Warp")) {
                    currentCube = adjacentCubes["left"];
                    targetPos = new Vector2(currentCube.transform.position.x, currentCube.transform.position.y + placeOnTop);
                }
                else {
                    warpPos = "left";
                    onWarp = true;
                }
            }

            anim.SetInteger("state", 3);
            audioSources[0].Play();
        }
        else if (CnInputManager.GetAxis("Horizontal") > 0 && CnInputManager.GetAxis("Vertical") > 0 && (Vector2)tr.position == targetPos && adjacentCubes.ContainsKey("up")) {
            if (moved == false) {
                moved = true;
            }

            if (adjacentCubes["up"] != null) {
                if (!adjacentCubes["up"].CompareTag("Warp")) {
                    currentCube = adjacentCubes["up"];
                    targetPos = new Vector2(currentCube.transform.position.x, currentCube.transform.position.y + placeOnTop);
                }
                else {
                    warpPos = "up";
                    onWarp = true;
                }
            }

            anim.SetInteger("state", 4);
            audioSources[0].Play();
        }
        else if (CnInputManager.GetAxis("Horizontal") < 0 && CnInputManager.GetAxis("Vertical") < 0 && (Vector2)tr.position == targetPos && adjacentCubes.ContainsKey("down")) {
            if (moved == false) {
                moved = true;
            }

            currentCube = adjacentCubes["down"];
            targetPos = new Vector2(currentCube.transform.position.x, currentCube.transform.position.y + placeOnTop);
            anim.SetInteger("state", 1);
            audioSources[0].Play();
        }

        coroutineActive = false;
    }

    IEnumerator WaitForLives() {
        audioSources[1].Play();
        gameState.SetPause(true);
        yield return new WaitForSeconds(3f);

        gameState.UpdateLives();
    }
}
