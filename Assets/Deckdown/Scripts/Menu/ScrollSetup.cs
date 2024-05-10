using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ScrollSetup : NetworkBehaviour
{
    [SerializeField] GameObject EarthCard, FireCard, WaterCard;
    [SerializeField] GameObject Poof;
    [SerializeField] SkinnedMeshRenderer ScrollSMR;

    private Animator animator;

    [Networked]
    public int hostTouch { get; set; } = 1;

    [Networked]
    public int clientTouch { get; set; } = 1;

    private bool hasStarted = false;


    private void Start()
    {
        PanelController.Instance.UpdatePanelState(GameState.ScrollAppeared);
        animator = transform.parent.GetComponent<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!hasStarted)
        {
            StartUnscrollRpc();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void HostWatchTouchRpc(bool hasAuth)
    {
        DebugPanel.Instance.UpdateMessage("Host Watch Touch");
        hostTouch--;

        if (clientTouch <= 0 && hostTouch <= 0)
        {
            animator.SetTrigger("MoveToSide");
            StartCoroutine(ShowCards());
        }

        DebugPanel.Instance.UpdateMessage("Non condition touch, client is " + clientTouch + " and host is " + hostTouch);
    }

    public void DebugScroll()
    {
        // DebugPanel.Instance.UpdateMessage("Debug: Non condition touch, client is " + clientTouch + " and host is " + hostTouch);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void PlayerWatchTouchRpc(bool hasAuth)
    {
        DebugPanel.Instance.UpdateMessage("Player Watch Touch Rpc");

        clientTouch--;

        DebugPanel.Instance.UpdateMessage("Non condition touch, client is " + clientTouch + " and host is " + hostTouch);

        if (clientTouch <= 0 && hostTouch <= 0)
        {
            animator.SetTrigger("MoveToSide");
            StartCoroutine(ShowCards());
        }
    }

    public void StartUnscrollRpc()
    {
        // DebugPanel.Instance.UpdateMessage("Non condition touch, client is " + clientTouch + " and host is " + hostTouch);
        if (clientTouch <= 0 && hostTouch <=0)
        {
            DebugPanel.Instance.UpdateMessage("Both Zeros");

            Material[] mat = new Material[2];
            Array.Copy(ScrollSMR.materials, mat, 2);
            ScrollSMR.materials = mat;

            animator.SetTrigger("MoveToSide");
            StartCoroutine(ShowCards());
            hasStarted = true;
        }
    }

    private IEnumerator ShowCards()
    {
        yield return new WaitForSeconds(18f);
        PanelController.Instance.UpdatePanelState(GameState.CardAppeared);
        EarthCard.SetActive(true);
        FireCard.SetActive(true);
        WaterCard.SetActive(true);
        Poof.SetActive(true);
        // gameObject.SetActive(false);
    }

}
