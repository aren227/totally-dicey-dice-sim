using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Absolute garbage code but I HAVE NO TIME
public class Gizmo : MonoBehaviour
{
    public MeshCollider xPosColl;
    public MeshCollider yPosColl;
    public MeshCollider zPosColl;
    public MeshCollider xRotColl;
    public MeshCollider yRotColl;
    public MeshCollider zRotColl;

    bool locked = false;

    bool isPos = false;
    int xyz = 0;
    Plane plane1, plane2;
    Vector3Int initPos;
    Vector3 initPosAtAxis;
    Quaternion initRot;
    float initAngleAtAxis;
    float finalAngle;

    public Transform target { get; private set; }

    public Vector3Int targetPosition { get; private set; }
    public Quaternion targetRotation { get; private set; }

    Vector3 GetPosAtAxis() {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 p = Vector3.zero;
        float t = 0;
        plane1.Raycast(mouseRay, out t);
        p = mouseRay.GetPoint(t);
        p = plane2.ClosestPointOnPlane(p);
        return p;
    }

    float GetAngleAtAxis() {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 p = Vector3.zero;
        float t = 0;
        plane1.Raycast(mouseRay, out t);
        p = mouseRay.GetPoint(t);
        p -= initPos;

        if (xyz == 0) return -Mathf.Atan2(p.y, p.z) * Mathf.Rad2Deg;
        if (xyz == 1) return -Mathf.Atan2(p.z, p.x) * Mathf.Rad2Deg;
        else return -Mathf.Atan2(p.x, p.y) * Mathf.Rad2Deg;
    }

    void ShowOnlySelectedGizmo() {
        xPosColl.gameObject.SetActive(isPos && xyz == 0);
        yPosColl.gameObject.SetActive(isPos && xyz == 1);
        zPosColl.gameObject.SetActive(isPos && xyz == 2);

        xRotColl.gameObject.SetActive(!isPos && xyz == 0);
        yRotColl.gameObject.SetActive(!isPos && xyz == 1);
        zRotColl.gameObject.SetActive(!isPos && xyz == 2);
    }

    void ShowAllGizmos() {
        xPosColl.gameObject.SetActive(true);
        yPosColl.gameObject.SetActive(true);
        zPosColl.gameObject.SetActive(true);

        xRotColl.gameObject.SetActive(true);
        yRotColl.gameObject.SetActive(true);
        zRotColl.gameObject.SetActive(true);
    }

    void Start() {
        ShowAllGizmos();
    }

    public void SetTarget(Transform target) {
        this.target = target;

        if (target == null) {
            locked = false;
            return;
        }

        targetPosition = Vector3Int.RoundToInt(target.position);
        targetRotation = target.localRotation;

        ShowAllGizmos();
    }

    void Update() {
        if (!target) return;

        transform.position = target.position;
        transform.rotation = Quaternion.identity;

        if (!locked && Input.GetMouseButtonDown(0)) {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (xPosColl.Raycast(mouseRay, out hit, 1000)) {
                locked = true;
                isPos = true;
                xyz = 0;
                plane1 = new Plane(Vector3.up, transform.position);
                plane2 = new Plane(Vector3.forward, transform.position);
                initPos = Vector3Int.RoundToInt(transform.position);
                initPosAtAxis = GetPosAtAxis();
                ShowOnlySelectedGizmo();
            }
            else if (yPosColl.Raycast(mouseRay, out hit, 1000)) {
                locked = true;
                isPos = true;
                xyz = 1;
                plane1 = new Plane(Vector3.right, transform.position);
                plane2 = new Plane(Vector3.forward, transform.position);
                initPos = Vector3Int.RoundToInt(transform.position);
                initPosAtAxis = GetPosAtAxis();
                ShowOnlySelectedGizmo();
            }
            else if (zPosColl.Raycast(mouseRay, out hit, 1000)) {
                locked = true;
                isPos = true;
                xyz = 2;
                plane1 = new Plane(Vector3.right, transform.position);
                plane2 = new Plane(Vector3.up, transform.position);
                initPos = Vector3Int.RoundToInt(transform.position);
                initPosAtAxis = GetPosAtAxis();
                ShowOnlySelectedGizmo();
            }
            else if (xRotColl.Raycast(mouseRay, out hit, 1000)) {
                locked = true;
                isPos = false;
                xyz = 0;
                plane1 = new Plane(Vector3.right, transform.position);
                initPos = Vector3Int.RoundToInt(transform.position);
                initRot = target.localRotation;
                initAngleAtAxis = GetAngleAtAxis();
                finalAngle = 0;
                ShowOnlySelectedGizmo();
            }
            else if (yRotColl.Raycast(mouseRay, out hit, 1000)) {
                locked = true;
                isPos = false;
                xyz = 1;
                plane1 = new Plane(Vector3.up, transform.position);
                initPos = Vector3Int.RoundToInt(transform.position);
                initRot = target.localRotation;
                initAngleAtAxis = GetAngleAtAxis();
                finalAngle = 0;
                ShowOnlySelectedGizmo();
            }
            else if (zRotColl.Raycast(mouseRay, out hit, 1000)) {
                locked = true;
                isPos = false;
                xyz = 2;
                plane1 = new Plane(Vector3.forward, transform.position);
                initPos = Vector3Int.RoundToInt(transform.position);
                initRot = target.localRotation;
                initAngleAtAxis = GetAngleAtAxis();
                finalAngle = 0;
                ShowOnlySelectedGizmo();
            }
        }
        else if (locked) {
            if (isPos) {
                Vector3 delta = GetPosAtAxis() - initPosAtAxis;
                Vector3Int roundedDelta = Vector3Int.RoundToInt(delta);
                targetPosition = initPos + roundedDelta;
            }
            else {
                float delta = GetAngleAtAxis() - initAngleAtAxis;
                initAngleAtAxis = GetAngleAtAxis();
                finalAngle += delta;
                float roundedAngle = Mathf.Round(finalAngle / 90f) * 90f;

                if (xyz == 0) {
                    targetRotation = Quaternion.Euler(roundedAngle, 0, 0) * initRot;
                }
                if (xyz == 1) {
                    targetRotation = Quaternion.Euler(0, roundedAngle, 0) * initRot;
                }
                if (xyz == 2) {
                    targetRotation = Quaternion.Euler(0, 0, roundedAngle) * initRot;
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                locked = false;
                ShowAllGizmos();
            }
        }
    }
}
