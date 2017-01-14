using UnityEngine;
using System.Collections.Generic;

public class AdjacentCubes : MonoBehaviour {
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;

    private readonly Dictionary<string, GameObject> adjacentCubes = new Dictionary<string, GameObject>();

    // Use this for initialization
    void Start() {
        if (up != null) {
            adjacentCubes.Add("up", up);
        }

        if (down != null) {
            adjacentCubes.Add("down", down);
        }

        if (left != null) {
            adjacentCubes.Add("left", left);
        }

        if (right != null) {
            adjacentCubes.Add("right", right);
        }
    }

    public Dictionary<string, GameObject> GetAdjacents() {
        return adjacentCubes;
    }
}