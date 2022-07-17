using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    Structure structure;

    ParticleSystem particleSystem;

    bool on = false;

    void Awake() {
        structure = FindObjectOfType<Structure>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void TurnOn() {
        on = true;
        particleSystem.Play();
    }

    public void TurnOff() {
        on = false;
        particleSystem.Stop();
    }

    void Update() {
        if (Input.GetKey(KeyCode.Q) && !on) TurnOn();
        else if (!Input.GetKey(KeyCode.Q) && on) TurnOff();
    }

    void FixedUpdate()
    {
        if (on) {
            const float force = 50;
            structure.rigid.AddForceAtPosition(transform.rotation * Vector3.up * force, transform.position);
        }
    }
}
