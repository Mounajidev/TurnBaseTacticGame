using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransform(originalRootBone, ragdollRootBone);

        Vector3 randomDir = new Vector3(Random.Range(-1f, +1f), 0, Random.Range(-1f, +1f));
        ApplyExplotionToRagdoll(ragdollRootBone, 300f, transform.position + randomDir, 10f);
    }

    private void MatchAllChildTransform(Transform root, Transform clone)
    {
        foreach ( Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if ( cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransform(child, cloneChild);

            }
        }
    }

    private void ApplyExplotionToRagdoll(Transform root, float explotionForce, Vector3 explotionPosition, float explotionRange)
    {
        foreach ( Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explotionForce, explotionPosition, explotionRange);
            }

            ApplyExplotionToRagdoll(child, explotionForce, explotionPosition, explotionRange);
        }
    }
}
