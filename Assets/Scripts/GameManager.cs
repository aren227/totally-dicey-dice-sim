using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameState state { get; private set; }

    Structure structure;
    Tester tester;
    LevelInfo levelInfo;

    Vector3 initialStructurePos;

    CanvasManager canvasManager;

    void Awake() {
        structure = FindObjectOfType<Structure>();
        tester = FindObjectOfType<Tester>();
        canvasManager = FindObjectOfType<CanvasManager>();
        levelInfo = FindObjectOfType<LevelInfo>();

        initialStructurePos = structure.transform.position;

        Camera.main.transform.position = initialStructurePos + new Vector3(7, 7, -7);
    }

    void Start() {
        canvasManager.SetState(GameState.BUILD);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Camera.main.transform.position = initialStructurePos + new Vector3(7, 7, -7);
        }

        if (state == GameState.BUILD) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = GameState.TEST;

                // @Temp: Fix strange issue.
                FindObjectOfType<Gizmo>()?.gameObject.SetActive(false);

                structure.BeginTest();
                tester.BeginTest();

                canvasManager.SetState(GameState.TEST);
            }
        }
        else if (state == GameState.TEST) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = GameState.BUILD;

                structure.EndTest(initialStructurePos);
                tester.EndTest();

                canvasManager.SetState(GameState.BUILD);
            }
            else if (tester.verdict != Verdict.RUNNING) {
                Debug.Log("Test result : " + tester.verdict);

                state = GameState.TEST_DONE;

                canvasManager.SetState(GameState.TEST_DONE);
            }
        }
        else if (state == GameState.TEST_DONE) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                state = GameState.BUILD;

                structure.EndTest(initialStructurePos);
                tester.EndTest();

                canvasManager.SetState(GameState.BUILD);
            }
            if (Input.GetKeyDown(KeyCode.X) && tester.verdict == Verdict.ACCEPTED) {
                // @Todo: Go to next level.
                Debug.Log("Next level");

                SceneManager.LoadScene(levelInfo.nextLevel, LoadSceneMode.Single);
            }
        }
    }
}

public enum GameState {
    BUILD,
    TEST,
    TEST_DONE,
};