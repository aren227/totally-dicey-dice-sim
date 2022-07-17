using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    const int minX = -2, minY = -2, minZ = -2;
    const int xSize = 5, ySize = 5, zSize = 5;

    Dictionary<Vector3Int, Part> parts = new Dictionary<Vector3Int, Part>();

    Rigidbody rigid;

    void Awake() {
        rigid = GetComponent<Rigidbody>();

        rigid.maxAngularVelocity = 30;

        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            Vector3Int pos = Vector3Int.RoundToInt(child.localPosition);

            AddPart(pos, child.GetComponent<Part>());
        }
    }

    public void AddPart(Vector3Int pos, Part part) {
        part.transform.parent = transform;
        part.transform.localPosition = pos;

        // @Todo: Check bounds
        parts.Add(new Vector3Int(pos.x, pos.y, pos.z), part);
    }

    public void RemovePart(GameObject part) {
        parts.Remove(Vector3Int.RoundToInt(part.transform.localPosition));
    }

    public Vector3 GetWorldPos(Vector3Int localPos) {
        return transform.position + localPos;
    }

    public bool CanAttach(Vector3Int pos, Part part) {
        if (parts.ContainsKey(pos)) return false;

        foreach (Side side in System.Enum.GetValues(typeof(Side))) {
            Vector3Int dir = side.GetVector();
            if (!part.CanAttach(side)) continue;
            if (!parts.ContainsKey(pos + dir)) continue;
            if (parts[pos + dir].CanAttach(side.GetOpposite())) return true;
        }

        return false;
    }

    void Start() {

    }

    public void BeginTest() {
        rigid.isKinematic = false;
    }

    public void EndTest(Vector3 moveTo) {
        rigid.isKinematic = true;

        transform.position = moveTo;
        transform.rotation = Quaternion.identity;
    }

    public int GetSum() {
        int sum = 0;
        foreach (Part part in parts.Values) {
            if (part.GetComponent<Dice>()) {
                sum += part.GetComponent<Dice>().GetNumber();
            }
        }
        return sum;
    }

    public void Throw() {
        transform.localEulerAngles = new Vector3(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f));

        rigid.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
        rigid.AddTorque(Random.onUnitSphere * 10, ForceMode.VelocityChange);
    }
}
