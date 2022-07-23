using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    const int minX = -2, minY = -2, minZ = -2;
    const int xSize = 5, ySize = 5, zSize = 5;

    Vector3Int maxSize;

    LevelInfo levelInfo;

    Dictionary<Vector3Int, Part> parts = new Dictionary<Vector3Int, Part>();

    List<Part> permanentParts = new List<Part>();

    public Rigidbody rigid { get; private set; }

    void Awake() {
        rigid = GetComponent<Rigidbody>();

        levelInfo = FindObjectOfType<LevelInfo>();

        maxSize = levelInfo.structureSize;

        rigid.maxAngularVelocity = 30;

        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            Vector3Int pos = Vector3Int.RoundToInt(child.localPosition);

            AddPart(pos, child.GetComponent<Part>(), true);
        }
    }

    public void AddPart(Vector3Int pos, Part part, bool permanent = false) {
        if (
            pos.x < -maxSize.x/2 || maxSize.x/2 < pos.x
            || pos.y < -maxSize.y/2 || maxSize.y/2 < pos.y
            || pos.z < -maxSize.z/2 || maxSize.z/2 < pos.z
        ) return;

        part.transform.parent = transform;
        part.transform.localPosition = pos;

        // @Todo: Check bounds
        parts.Add(new Vector3Int(pos.x, pos.y, pos.z), part);

        if (permanent) {
            permanentParts.Add(part);
        }
    }

    public bool CanRemove(Part part) {
        return !permanentParts.Contains(part);
    }

    public void RemovePart(Part part) {
        parts.Remove(Vector3Int.RoundToInt(part.transform.localPosition));

        Destroy(part.gameObject);
        permanentParts.Remove(part);
    }

    public void MovePart(Part part, Vector3Int pos) {
        if (IsOccupied(pos)) return;
        parts.Remove(Vector3Int.RoundToInt(part.transform.localPosition));

        part.transform.localPosition = pos;
        parts[pos] = part;
    }

    public Part GetPartAt(Vector3Int pos) {
        if (parts.ContainsKey(pos)) return parts[pos];
        return null;
    }

    public Vector3 GetWorldPos(Vector3Int localPos) {
        return transform.position + localPos;
    }

    public Vector3Int GetLocalPos(Vector3 worldPos) {
        return Vector3Int.RoundToInt(worldPos - transform.position);
    }

    public bool CanAttach(Vector3Int pos, Part part) {
        if (parts.ContainsKey(pos)) return false;

        if (
            pos.x < -maxSize.x/2 || maxSize.x/2 < pos.x
            || pos.y < -maxSize.y/2 || maxSize.y/2 < pos.y
            || pos.z < -maxSize.z/2 || maxSize.z/2 < pos.z
        ) return false;

        foreach (Side side in System.Enum.GetValues(typeof(Side))) {
            Vector3Int dir = side.GetVector();
            if (!part.CanAttach(side)) continue;
            if (!parts.ContainsKey(pos + dir)) continue;
            if (parts[pos + dir].CanAttach(side.GetOpposite())) return true;
        }

        return false;
    }

    public bool IsOccupied(Vector3Int pos) {
        if (
            pos.x < -maxSize.x/2 || maxSize.x/2 < pos.x
            || pos.y < -maxSize.y/2 || maxSize.y/2 < pos.y
            || pos.z < -maxSize.z/2 || maxSize.z/2 < pos.z
        ) return true;

        return parts.ContainsKey(pos);
    }

    void Start() {

    }

    public void BeginTest() {
        rigid.isKinematic = false;

        Vector3 centerOfMass = Vector3.zero;
        float sum = 0;
        foreach (Part part in parts.Values) {
            centerOfMass += part.transform.localPosition * part.mass;
            sum += part.mass;
        }

        rigid.centerOfMass = centerOfMass / sum;
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

        rigid.AddForce(Vector3.up * 20, ForceMode.Impulse);
        rigid.AddTorque(Random.onUnitSphere * 20, ForceMode.Impulse);
    }
}
