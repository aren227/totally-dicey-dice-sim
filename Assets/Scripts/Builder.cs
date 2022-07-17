using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    Dictionary<string, int> inventory = new Dictionary<string, int>();

    GameManager gameManager;
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

    Gizmo gizmo;

    void Awake() {
        gameManager = FindObjectOfType<GameManager>();
        structure = FindObjectOfType<Structure>();

        selectedName = null;
    }

    void Start() {
        AddItem("dice", 10);
        AddItem("metal_frame", 10);
        AddItem("sphere_frame", 10);
        AddItem("jelly", 10);
        AddItem("box", 10);
        AddItem("thruster", 20);

        gizmo = Instantiate(PrefabRegistry.Instance.gizmos).GetComponent<Gizmo>();
        gizmo.gameObject.SetActive(false);
    }

    public void AddItem(string name, int amount) {
        if (!inventory.ContainsKey(name)) {
            inventory[name] = 0;
        }
        inventory[name] = inventory[name] + amount;

        FindObjectOfType<CanvasManager>().UpdateInventory(inventory);
    }

    public void RemoveItem(string name, int amount) {
        if (!inventory.ContainsKey(name)) return;

        inventory[name] = Mathf.Max(inventory[name] - amount, 0);

        FindObjectOfType<CanvasManager>().UpdateInventory(inventory);
    }

    public void SelectItem(string name) {
        if (name == null) {
            selectedName = null;
            return;
        }

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
        isFreeCam = Input.GetMouseButton(1) || gameManager.state != GameState.BUILD;

        if (isFreeCam) {
            Cursor.lockState = CursorLockMode.Locked;

            Transform camTransform = Camera.main.transform;

            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
            if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
            if (Input.GetKey(KeyCode.D)) dir += Vector3.right;
            dir.Normalize();

            if (Input.GetKey(KeyCode.Q)) dir += Vector3.up;
            if (Input.GetKey(KeyCode.E)) dir += Vector3.down;

            float targetSpeed = 0;
            if (dir.sqrMagnitude > 0) {
                targetSpeed = maxCamSpeed;
            }

            camSpeed = Mathf.SmoothDamp(camSpeed, targetSpeed, ref camSpeedVel, 0.2f);

            dir = (camTransform.rotation * dir.normalized) * dir.magnitude;

            Vector3 delta = dir * camSpeed * Time.deltaTime;

            camTransform.position = camTransform.position + delta;

            Vector3 angles = camTransform.eulerAngles;
            angles.y += Input.GetAxis("Mouse X") * mouseSensitivity;
            angles.x = angles.x -Input.GetAxis("Mouse Y") * mouseSensitivity;

            camTransform.eulerAngles = angles;

            RemovePreviewObject();
            gizmo.SetTarget(null);
            gizmo.gameObject.SetActive(false);
        }
        else {
            Cursor.lockState = CursorLockMode.None;

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
            if (Input.GetKeyDown(KeyCode.Alpha6)) {
                SelectItem("thruster");
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                SelectItem(null);
            }

            if (Input.GetKey(KeyCode.LeftShift)) {
                // Delete
                RemovePreviewObject();

                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(mouseRay, out hit, 1000) && hit.collider.GetComponentInParent<Part>()) {
                    Part part = hit.collider.GetComponentInParent<Part>();

                    Vector3Int pos = part.pos;
                    Vector3Int dir = Vector3Int.RoundToInt(hit.normal);

                    if (Input.GetMouseButtonDown(0)) {
                        if (structure.CanRemove(part)) {
                            structure.RemovePart(part);
                        }
                    }
                }
            }
            else if (selectedName != null) {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(mouseRay, out hit, 1000) && hit.collider.GetComponentInParent<Part>()) {
                    Part part = hit.collider.GetComponentInParent<Part>();

                    Vector3Int pos = part.pos;
                    Vector3Int dir = Vector3Int.RoundToInt(hit.normal);

                    bool avail = structure.CanAttach(pos+dir, PrefabRegistry.Instance.parts[selectedName]);

                    SetPreviewObject(selectedName, avail);

                    previewObject.transform.position = structure.GetWorldPos(pos + dir);

                    if (Input.GetMouseButtonDown(0)) {
                        if (avail) {
                            GameObject cloned = Instantiate(PrefabRegistry.Instance.parts[selectedName].gameObject);
                            structure.AddPart(pos+dir, cloned.GetComponent<Part>());
                        }
                    }
                }
                else {
                    RemovePreviewObject();
                }

                gizmo.SetTarget(null);
                gizmo.gameObject.SetActive(false);
            }
            else {
                RemovePreviewObject();

                if (Input.GetMouseButtonDown(0)) {
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(mouseRay, out hit, 1000) && hit.collider.GetComponentInParent<Part>()) {
                        Part part = hit.collider.GetComponentInParent<Part>();

                        gizmo.gameObject.SetActive(true);
                        gizmo.SetTarget(part.transform);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape)) {
                    gizmo.SetTarget(null);
                    gizmo.gameObject.SetActive(false);
                }

                if (gizmo.gameObject.activeSelf && gizmo.target && gizmo.target.GetComponent<Part>()) {
                    Part part = gizmo.target.GetComponent<Part>();

                    structure.MovePart(part, structure.GetLocalPos(gizmo.targetPosition));

                    part.transform.localRotation = gizmo.targetRotation;
                }
            }
        }
    }
}
