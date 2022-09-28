using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestructibleCrate : MonoBehaviour
{

    [SerializeField] private Transform crateDestroyedPrefab;
    public static event EventHandler OnAnyDestroyed;

    private GridPosition gridPosition;

    private void Start()
    {
       gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void Damage()
    {

        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplotionToChildren(crateDestroyedTransform, 150f, transform.position, 10f);
        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExplotionToChildren(Transform root, float explotionForce, Vector3 explotionPosition, float explotionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explotionForce, explotionPosition, explotionRange);
            }

            ApplyExplotionToChildren(child, explotionForce, explotionPosition, explotionRange);
        }
    }
}
