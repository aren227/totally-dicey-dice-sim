using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTCamera : MonoBehaviour
{
    Camera cam;

    void Awake() {
        cam = GetComponent<Camera>();
    }

    public void TakePicture(GameObject instance, RenderTextureImage image) {
        cam.enabled = true;

        cam.targetTexture = image.renderTexture;

        instance.transform.position = transform.position + new Vector3(0, 0, 2);
        instance.transform.eulerAngles = new Vector3(0, 0, 0);

        cam.Render();

        cam.targetTexture = null;

        cam.enabled = false;
    }
}
