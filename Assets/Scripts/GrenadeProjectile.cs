using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GrenadeProjectile : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }

    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Action onGrenadeBehaviorComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
        else
        {

            Vector3 moveDir = (targetPosition - positionXZ).normalized;

            float moveSpeed = 15f;
            positionXZ += moveDir * moveSpeed * Runner.DeltaTime;

            float distance = Vector3.Distance(positionXZ, targetPosition);
            float distanceNormalized = 1 - distance / totalDistance;

            float maxHeight = totalDistance / 3f;
            float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
            transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

            float reachedTargetDistance = 0.2f;
            if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
            {
                float damageRadius = 2f;
                Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

                foreach (Collider collider in colliderArray)
                {
                    if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                    {
                        Debug.Log("unit found");
                        targetUnit.Damage(30);
                    }

                    if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                    {
                        Debug.Log("destructible crate found");
                        destructibleCrate.Damage();
                    }
                }

                OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
                trailRenderer.transform.parent = null;

                Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
                Destroy(gameObject);

                onGrenadeBehaviorComplete();
            }
        }
    }

    public void Setup( GridPosition targetGridPosition, Action onGrenadeBehaviorComplete)
    {
        this.onGrenadeBehaviorComplete = onGrenadeBehaviorComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
