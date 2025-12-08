using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class PuzzlePiece : MonoBehaviour
{
    [Header("Allowed Snap Targets")]
    public List<Transform> allowedTargets;

    public float snapDistance = 0.05f;
    public AudioClip snapSound;

    [HideInInspector] public bool isSolved = false;

    [Header("Glow Settings")]
    public Material glowMaterial;

    private AudioSource audioSource;
    private Rigidbody rb;
    private Collider col;
    private Renderer rend;
    private Material originalMat;

    private PuzzleController controller;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalMat = rend.material;

        controller = FindObjectOfType<PuzzleController>();
    }

    void Update()
    {
        if (isSolved) return;

        foreach (var target in allowedTargets)
        {
            if (Vector3.Distance(transform.position, target.position) < snapDistance)
            {
                controller.TrySnap(this, target);
                break;
            }
        }
    }

    public void SnapIntoPlace(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
        isSolved = true;

        if (audioSource != null && snapSound != null)
            audioSource.PlayOneShot(snapSound);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        var grabbable = GetComponent<Grabbable>();
        if (grabbable != null)
            grabbable.enabled = false;

        if (col != null)
            col.enabled = false;

        SetGlow(false);
    }

    public void SetGlow(bool glow)
    {
        if (rend == null) return;

        if (glow && glowMaterial != null)
            rend.material = glowMaterial;
        else if (originalMat != null)
            rend.material = originalMat;
    }
}