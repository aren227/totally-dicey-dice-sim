using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState state { get; private set; }

    Structure structure;
    Tester tester;

    Vector3 initialStructurePos;

    void Awake() {
        structure = FindObjectOfType<Structure>();
        tester = FindObjectOfType<Tester>();

        initialStructurePos = structure.transform.position;
    }

    void Update() {
        if (state == GameState.BUILD) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = GameState.TEST;

                structure.BeginTest();
                tester.BeginTest();
            }
        }
        else if (state == GameState.TEST) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = GameState.BUILD;

                structure.EndTest(initialStructurePos);
                tester.EndTest();
            }
            else if (tester.verdict != Verdict.RUNNING) {
                Debug.Log("Test result : " + tester.verdict);

                state = GameState.TEST_DONE;
            }
        }
        else if (state == GameState.TEST_DONE) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = GameState.BUILD;

                structure.EndTest(initialStructurePos);
                tester.EndTest();
            }
            if (Input.GetKeyDown(KeyCode.X) && tester.verdict == Verdict.ACCEPTED) {
                // @Todo: Go to next level.
                Debug.Log("Next level");
            }
        }
    }
}

public enum GameState {
    BUILD,
    TEST,
    TEST_DONE,
};