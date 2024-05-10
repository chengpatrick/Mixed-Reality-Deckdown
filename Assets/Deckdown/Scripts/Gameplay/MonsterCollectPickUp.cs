using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Oculus.Interaction;

public class MonsterCollectPickUp : MonoBehaviour
{
    [Tooltip("The IActiveState to debug.")]
    [SerializeField, Interface(typeof(IActiveState))]
    private UnityEngine.Object _activeState;
    private IActiveState ActiveState { get; set; }

    [SerializeField] private Transform WristAnchor;
    [SerializeField] private Vector3 offset;

    public bool grabbed;
    public bool offHand;

    private void Awake()
    {
        ActiveState = _activeState as IActiveState;
        this.AssertField(ActiveState, nameof(ActiveState));

        transform.parent = WristAnchor;
        grabbed = false;
        offHand = false;
    }

    private void Update()
    {
        if (!grabbed)
        {
            transform.position = WristAnchor.position + offset;
        }

        bool isActive = ActiveState.Active;

        if (isActive && !offHand)
        {
            grabbed = true;

            transform.parent = null;
            offHand = true;
        }
    }

    public void ResetMonsterPosition()
    {
        transform.position = WristAnchor.position + offset;
        transform.parent = WristAnchor;
        grabbed = false;
        offHand = false;
    }

    public void WristAnchorSetup(Transform trans)
    {
        WristAnchor = trans;
        transform.parent = WristAnchor;
    }
}
