using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyAI : MonoBehaviour {
    // enemy variables
    private const float speed = 1.0f;
    private Vector2 targetPos;
    private Transform tr;
    private readonly Vector2 placeOnTop = new Vector2(0.008f, 0.40f);
    private new Renderer renderer;
    private CircleCollider2D cc2d;
    private Animator anim;
    private bool spawned;

    // current and adjacent cube variables
    public GameObject startCube;
    private GameObject currentCube;
    private AdjacentCubes currentCubeAdj;
    private Dictionary<string, GameObject> adjacentCubes;

    // direction variables
    private const float distanceUpDownX = 0.2346f;
    private const float distanceUpDownY = 0.29432f;
    private const float distanceLeftRightX = 0.2254f;
    private const float distanceLeftRightY = -0.29432f;

    // pattern variables
    public string[] pattern = new string[] { "right", "right", "down" };
    //private string[] pattern = new string[3] { "up", "up", "right" };
    //private string[] pattern = new string[3] { "up", "right", "up" };
    private int counter;
    public int counterLimit = 3;
    private bool floorless;

    // coroutine variables
    private bool coroutineSpawn;
    private bool coroutineMovement;
    private bool respawnFinished = true;

    private GameState gameState;

    // Use this for initialization
    void Start() {
        StartCoroutine("SpawnEnemy");

        gameState = GameObject.Find("GameState").GetComponent<GameState>();
        renderer = GetComponent<Renderer>();
        anim = GetComponent<Animator>();
        cc2d = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update() {
        if (gameState.GetGameOver() == false && gameState.GetPause() == false) {
            if (spawned && renderer.isVisible) {
                if (coroutineMovement == false) {
                    StartCoroutine("DoMovement");
                }

                transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            }
            else if (spawned && respawnFinished) {
                coroutineMovement = false;
                StopCoroutine("DoMovement");
                StartCoroutine("DoRespawn");
            }
            else if (spawned == false && coroutineSpawn == false) {
                StartCoroutine("SpawnEnemy");
            }
        }
        else {
            coroutineSpawn = false;
            coroutineMovement = false;
            StopAllCoroutines();
        }
    }

    IEnumerator SpawnEnemy() {
        coroutineSpawn = true;

        yield return new WaitForSeconds(3f);

        currentCube = startCube;
        transform.position = (Vector2)currentCube.transform.position + placeOnTop;

        targetPos = transform.position;
        tr = transform;

        spawned = true;
        coroutineSpawn = false;
    }

    IEnumerator DoRespawn() {
        respawnFinished = false;

        yield return new WaitForSeconds(3f);

        currentCube = startCube;
        transform.position = (Vector2)currentCube.transform.position + placeOnTop;
        targetPos = transform.position;
        counter = 0;

        SpriteRenderer enemySprite = GetComponent<SpriteRenderer>();
        floorless = false;
        enemySprite.sortingOrder = 1;
        cc2d.enabled = true;

        respawnFinished = true;
    }

    IEnumerator DoMovement() {
        coroutineMovement = true;

        if ((Vector2)tr.position == targetPos && counter < counterLimit) {
            currentCubeAdj = currentCube.GetComponent<AdjacentCubes>();
            adjacentCubes = currentCubeAdj.GetAdjacents();
            anim.SetInteger("state", 0);

            if (adjacentCubes.ContainsKey(pattern[counter])) {
                yield return new WaitForSeconds(1f);

                currentCube = adjacentCubes[pattern[counter]];
                targetPos = (Vector2)currentCube.transform.position + placeOnTop;

                switch (pattern[counter]) {
                    case "right":
                        anim.SetInteger("state", 2);
                        break;
                    case "left":
                        anim.SetInteger("state", 3);
                        break;
                    case "up":
                        anim.SetInteger("state", 4);
                        break;
                    case "down":
                        anim.SetInteger("state", 1);
                        break;
                }

                counter++;
            }
            else {
                SpriteRenderer enemySprite = GetComponent<SpriteRenderer>();

                switch (pattern[counter]) {
                    case "right":
                        if (floorless == false) {
                            yield return new WaitForSeconds(1f);

                            targetPos += new Vector2(distanceLeftRightX, distanceLeftRightY);
                            anim.SetInteger("state", 2);
                            floorless = true;
                            cc2d.enabled = false;
                        }
                        else {
                            targetPos += new Vector2(0, -0.2f);
                        }
                        break;
                    case "left":
                        if (floorless == false) {
                            yield return new WaitForSeconds(1f);

                            targetPos += new Vector2(-distanceLeftRightX, -distanceLeftRightY);
                            anim.SetInteger("state", 3);
                            floorless = true;
                            enemySprite.sortingOrder = -1;
                            cc2d.enabled = false;
                        }
                        else {
                            targetPos += new Vector2(0, -0.2f);
                        }
                        break;
                    case "up":
                        if (floorless == false) {
                            yield return new WaitForSeconds(1f);

                            targetPos += new Vector2(distanceUpDownX, distanceUpDownY);
                            anim.SetInteger("state", 4);
                            floorless = true;
                            enemySprite.sortingOrder = -1;
                            cc2d.enabled = false;
                        }
                        else {
                            targetPos += new Vector2(0, -0.2f);
                        }
                        break;
                    case "down":
                        if (floorless == false) {
                            yield return new WaitForSeconds(1f);

                            targetPos += new Vector2(-distanceUpDownX, -distanceUpDownY);
                            anim.SetInteger("state", 1);
                            floorless = true;
                            cc2d.enabled = false;
                        }
                        else {
                            targetPos += new Vector2(0, -0.2f);
                        }
                        break;
                }
            }
        }
        else if ((Vector2)tr.position == targetPos) {
            counter = 0;
        }

        coroutineMovement = false;
    }
}