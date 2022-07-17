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

    void Awake() {
        structure = FindObjectOfType<Structure>();

        selectedName = null;
    }

    void Start() {
        inventory.Add("dice", 10);
        inventory.Add("metal_frame", 10);
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
            // @Todo
        }
        else {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SelectItem("dice");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SelectItem("metal_frame");
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
        }
    }
}
