using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestPhase
{
    public TestType type;
    public int number;
    public NumberTest numberTest;

    public Verdict verdict;

    Structure structure;

    float beginTimestamp;

    float lastTransformUpdatedAt;
    Vector3 lastPosition;
    Vector3 lastAngles;

    public void Begin(Structure structure) {
        this.structure = structure;

        verdict = Verdict.RUNNING;

        beginTimestamp = Time.time;

        lastTransformUpdatedAt = Time.time;
        lastPosition = structure.transform.position;
        lastAngles = structure.transform.eulerAngles;

        if (type == TestType.ROLL) {
            structure.Throw();
        }
    }

    public void Update() {
        // @Hardcoded
        if (Time.time - beginTimestamp > 10) {
            verdict = Verdict.TIME_OVER;
            return;
        }

        if (type == TestType.ROLL) {
            if (Time.time - beginTimestamp > 1f) {
                verdict = Verdict.ACCEPTED;
            }
        }
        else if (type == TestType.NUMBER) {
            // @Todo: These are arbitrary threshold.
            if (
                Vector3.Distance(structure.transform.position, lastPosition) > 0.1f
                || Vector3.Distance(structure.transform.eulerAngles, lastAngles) > 0.1f
            ) {
                lastTransformUpdatedAt = Time.time;
                lastPosition = structure.transform.position;
                lastAngles = structure.transform.eulerAngles;
            }

            // @Hardcoded
            if (Time.time - lastTransformUpdatedAt > 1f) {
                int sum = structure.GetSum();
                if (
                    numberTest == NumberTest.EQUAL && number == sum
                    || numberTest == NumberTest.GEQUAL && sum >= number
                    || numberTest == NumberTest.LEQUAL && sum <= number
                ) {
                    verdict = Verdict.ACCEPTED;
                }
                else {
                    verdict = Verdict.FAILED;
                }
                return;
            }
        }
    }
}

public enum TestType {
    ROLL,
    NUMBER,
}

public enum NumberTest {
    EQUAL,
    LEQUAL,
    GEQUAL,
}