using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    Dictionary<string, int> inventory = new Dictionary<string, int>();

    Structure structure;

    string selectedName;

    string previewSelectedName;
    bool previewAvail;
    GameObject previewObject;

    bool isFreeCam = false;

    float camSpeed = 0;
    float camSpeedVel = 0;
    const float maxCamSpeed = 15;
    const float mouseSensitivity = 3;

    void Awake() {
        structure = FindObjectOfType<Structure>();

        selectedName = null;
    }

    void Start() {
        inventory.Add("dice", 10);
        inventory.Add("metal_frame", 10);
        inventory.Add("sphere_frame", 10);
        inventory.Add("jelly", 10);
        inventory.Add("box", 10);
    }

    public void AddItem(string name, int amount) {
        if (!inventory.ContainsKey(name)) {
            inventory[name] = 0;
        }
        inventory[name] = inventory[name] + amount;
    }

    void SelectItem(string name) {
        if (!inventory.ContainsKey(name) || inventory[name] <= 0) return;

        selectedName = name;
    }

    void SetPreviewObject(string name, bool avail) {
        // No need to update.
        if (previewSelectedName == name && previewAvail == avail) return;

        RemovePreviewObject();

        previewSelectedName = name;
        previewAvail = avail;

        GameObject cloned = Instantiate(PrefabRegistry.Instance.parts[name].gameObject);

        foreach (Collider collider in cloned.GetComponentsInChildren<Collider>()) {
            Destroy(collider);
        }

        foreach (MeshRenderer meshRenderer in cloned.GetComponentsInChildren<MeshRenderer>()) {
            meshRenderer.sharedMaterial = avail ? PrefabRegistry.Instance.availMat : PrefabRegistry.Instance.notAvailMat;
        }

        previewObject = cloned;
    }

    void RemovePreviewObject() {
        if (previewObject) {
            Destroy(previewObject);

            previewSelectedName = null;
        }
    }

    void Update() {
        if (isFreeCam) {
            Transform camTransform = Camera.main.transform;

            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
            if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
            if (Input.GetKey(KeyCode.D)) dir += Vector3.right;

            float targetSpeed = 0;
            if (dir.sqrMagnitude > 0) {
                targetSpeed = maxCamSpeed;
            }

            camSpeed = Mathf.SmoothDamp(camSpeed, targetSpeed, ref camSpeedVel, 0.2f);

            dir = camTransform.rotation * dir.normalized;

            Vector3 delta = dir * camSpeed * Time.deltaTime;

            camTransform.position = camTransform.position + delta;

            Vector3 angles = camTransform.eulerAngles;
            angles.y += Input.GetAxis("Mouse X") * mouseSensitivity;
            angles.x = angles.x -Input.GetAxis("Mouse Y") * mouseSensitivity;

            camTransform.eulerAngles = angles;

            if (Input.GetKeyDown(KeyCode.V)) {
                isFreeCam = false;

                Cursor.lockState = CursorLockMode.None;
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SelectItem("dice");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SelectItem("metal_frame");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SelectItem("sphere_frame");
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                SelectItem("jelly");
            }
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                SelectItem("box");
            }

            if (selectedName != null) {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(mouseRay, out hit, 1000) && hit.collider.GetComponentInParent<Part>()) {
                    Part part = hit.collider.GetComponentInParent<Part>();

                    Vector3Int pos = part.pos;
                    Vector3Int dir = Vector3Int.RoundToInt(hit.normal);

                    bool avail = structure.CanAttach(pos+dir, PrefabRegistry.Instance.parts[selectedName]);

                    SetPreviewObject(selectedName, avail);

                    previewObject.transform.position = structure.GetWorldPos(pos + dir);

                    if (avail) {
                        if (Input.GetMouseButtonDown(0)) {
                            GameObject cloned = Instantiate(PrefabRegistry.Instance.parts[selectedName].gameObject);
                            structure.AddPart(pos+dir, cloned.GetComponent<Part>());
                        }
                    }
                }
                else {
                    RemovePreviewObject();
                }
            }
            else {
                RemovePreviewObject();
            }

            if (Input.GetKeyDown(KeyCode.V)) {
                isFreeCam = true;

                Cursor.lockState = CursorLockMode.Locked;

                RemovePreviewObject();
            }
        }
    }
}
