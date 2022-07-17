using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public List<TestPhase> testPhases = new List<TestPhase>();

    Structure structure;

    public int phase { get; private set; }

    public Verdict verdict { get; private set; }

    float lastMoveUpdatedAt;

    public void BeginTest() {
        structure = FindObjectOfType<Structure>();

        structure.BeginTest();

        verdict = Verdict.RUNNING;
        phase = 0;

        foreach (TestPhase testPhase in testPhases) {
            testPhase.verdict = Verdict.WAITING;
        }

        Debug.Log("Begin test phases");
    }

    public void EndTest() {
        Debug.Log("End test phases");
    }

    void Update() {
        if (verdict != Verdict.RUNNING) return;

        if (phase >= testPhases.Count) {
            verdict = Verdict.ACCEPTED;
            return;
        }

        TestPhase testPhase = testPhases[phase];

        if (testPhase.verdict == Verdict.WAITING) {
            testPhase.Begin(structure);
        }
        else {
            testPhase.Update();

            if (testPhase.verdict != Verdict.RUNNING) {
                if (testPhase.verdict == Verdict.ACCEPTED) {
                    // Success
                    Debug.Log($"Phase {phase} accepted.");
                    phase++;
                }
                else {
                    // Fail
                    Debug.Log($"Phase {phase} failed. Reason: {testPhase.verdict}");
                    verdict = testPhase.verdict;
                }
            }
        }
    }
}

public enum Verdict {
    WAITING,
    RUNNING,
    FAILED,
    TIME_OVER,
    ACCEPTED,
}