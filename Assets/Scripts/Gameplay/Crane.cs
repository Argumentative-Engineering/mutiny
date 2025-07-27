using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 1.0f;     // Time it takes to rotate 180 degrees
    [SerializeField] private float intervalBetweenRotations = 5.0f; // Time between each rotation

    private bool isRotating = false;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private bool flipped = false;

    private void Start()
    {
        InvokeRepeating(nameof(StartRotation), intervalBetweenRotations, intervalBetweenRotations + rotationDuration);
    }

    private void StartRotation()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateOverTime());
        }
    }

    private System.Collections.IEnumerator RotateOverTime()
    {
        isRotating = true;

        float elapsed = 0f;
        startRotation = transform.rotation;
        // Flip around Y-axis by 180 degrees
        endRotation = Quaternion.Euler(0, flipped ? 0 : 180, 0);
        flipped = !flipped;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }
}
