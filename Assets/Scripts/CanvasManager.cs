using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public GameObject buildTipBg;
    public GameObject testTipBg;
    public GameObject testPhasesBg;

    public GameObject testPhasesBgUpperPivot;
    public GameObject testPhasesBgLowerPivot;
    public GameObject buildInventoryBg;
    public GameObject testResultBg;
    public GameObject testStateBg;

    public Text numberText;

    List<GameObject> phaseIcons = new List<GameObject>();

    GameManager gameManager;
    Structure structure;
    Tester tester;
    TestResultUi testResultUi;

    Dictionary<string, GameObject> itemIcons = new Dictionary<string, GameObject>();

    void Awake() {
        gameManager = FindObjectOfType<GameManager>();
        tester = FindObjectOfType<Tester>();
        testResultUi = FindObjectOfType<TestResultUi>();
        structure = FindObjectOfType<Structure>();
    }

    void Start() {
        InitTestPhases();
    }

    public void SetState(GameState state) {
        buildTipBg.SetActive(state == GameState.BUILD);
        testTipBg.SetActive(state == GameState.TEST || state == GameState.TEST_DONE);
        // testPhasesBgUpperPivot.SetActive(state == GameState.BUILD);
        // testPhasesBgLowerPivot.SetActive(state == GameState.TEST || state == GameState.TEST_DONE);
        buildInventoryBg.SetActive(state == GameState.BUILD);
        testResultBg.SetActive(state == GameState.TEST_DONE);
        testStateBg.SetActive(state == GameState.TEST || state == GameState.TEST_DONE);

        testPhasesBg.transform.parent = state == GameState.BUILD ? testPhasesBgUpperPivot.transform : testPhasesBgLowerPivot.transform;
        testPhasesBg.transform.localPosition = Vector3.zero;
    }

    void InitTestPhases() {
        foreach (GameObject icon in phaseIcons) {
            Destroy(icon);
        }

        phaseIcons.Clear();

        foreach (TestPhase testPhase in tester.testPhases) {
            if (testPhase.type == TestType.NUMBER) {
                GameObject cloned = Instantiate(PrefabRegistry.Instance.diceUi, testPhasesBg.transform);

                DiceUi diceUi = cloned.GetComponent<DiceUi>();

                diceUi.SetNumber(testPhase.number);

                phaseIcons.Add(cloned);

                // @Todo: Number test type
            }
            else if (testPhase.type == TestType.ROLL) {
                GameObject cloned = Instantiate(PrefabRegistry.Instance.arrowUi, testPhasesBg.transform);

                phaseIcons.Add(cloned);
            }
        }
    }

    public void UpdateInventory(Dictionary<string, int> inv) {
        foreach (string item in inv.Keys) {
            if (inv[item] > 0 && !itemIcons.ContainsKey(item)) {
                GameObject cloned = Instantiate(PrefabRegistry.Instance.itemUi, buildInventoryBg.transform);

                ItemView itemView = cloned.GetComponent<ItemView>();
                itemView.SetItem(item, inv[item]);

                itemIcons.Add(item, cloned);

                GameObject partObj = Instantiate(PrefabRegistry.Instance.parts[item].gameObject);
                itemView.itemImage.QueuePicture(partObj);
            }
        }

        foreach (string item in itemIcons.Keys) {
            if (!inv.ContainsKey(item) || inv[item] <= 0) {
                itemIcons[item].SetActive(false);
            }
            else {
                itemIcons[item].SetActive(true);

                itemIcons[item].GetComponent<ItemView>().SetItem(item, inv[item]);
            }
        }
    }

    void Update() {
        if (tester.testPhases.Count == phaseIcons.Count) {
            for (int i = 0; i < tester.testPhases.Count; i++) {
                // @Hardcoded
                Color color;
                if (tester.testPhases[i].verdict == Verdict.WAITING) {
                    color = Color.clear;
                }
                else if (tester.testPhases[i].verdict == Verdict.RUNNING) {
                    color = Color.yellow;
                }
                else if (tester.testPhases[i].verdict == Verdict.ACCEPTED) {
                    color = Color.green;
                }
                else {
                    color = Color.red;
                }

                foreach (Outline outline in phaseIcons[i].GetComponentsInChildren<Outline>()) {
                    outline.effectColor = color * 5f;
                }
            }
        }

        if (gameManager.state == GameState.TEST || gameManager.state == GameState.TEST_DONE) {
            numberText.text = structure.GetSum().ToString();
        }

        if (gameManager.state == GameState.TEST_DONE) {
            if (tester.verdict == Verdict.ACCEPTED) {
                testResultUi.SetText("Solved!", "");
            }
            else {
                testResultUi.SetText("Failed!", "");
            }
        }
    }
}
