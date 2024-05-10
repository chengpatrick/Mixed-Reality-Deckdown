using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float captureTimer = 5f;
    [SerializeField] private int monsterType; 

    private bool underCapture = false;
    private Coroutine checkCapture;
    private Rigidbody myRigidbody;
    private Transform captureRef; //where the souls will be flying to
    private Transform startTransform;

    // Start is called before the first frame update
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();

        _ = StartCoroutine(SoulScatter());
        Destroy(gameObject, 30f);
    }

    private void Update()
    {
    }

    private IEnumerator SoulScatter()
    {
        var randomDirection = Random.onUnitSphere;
        var randomForce = Random.Range(minForce, maxForce);

        gameObject.GetComponent<Rigidbody>().AddForce(randomDirection * randomForce, ForceMode.Impulse);
        yield return null;
    }

    private IEnumerator MoveToCollector(float interval, float duration, Transform t)
    {
        float elapsedTime = 0f;
        Transform currPosition = startTransform; 
        while (t != null && elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(currPosition.position, t.position, elapsedTime / duration);
            yield return new WaitForSeconds(interval);
        }

    }

    public void OnCapture(int typeID, float duration, Transform t)
    {
        //DebugPanel.Instance.UpdateMessage("Comparing projectile type[" + typeID + "] to soul type[" + monsterType + "]");
        if (typeID != monsterType && !underCapture)
        {
            captureRef = t;
            //DebugPanel.Instance.UpdateMessage("CAPTURING soul, delay for: " + captureTimer);
            myRigidbody.isKinematic = true;
            underCapture = true;
            checkCapture = StartCoroutine(CheckForCapture(captureTimer));
            StartCoroutine(MoveToCollector(0.05f, duration, t));
        }
    }

    public void InterruptCapture()
    {
        underCapture = false;
        myRigidbody.isKinematic = false;
        StopCoroutine(checkCapture);
    }

    private IEnumerator CheckForCapture(float delay)
    {
        //after delay, decide if capture is done
        yield return new WaitForSeconds(delay);
        if (underCapture)
        {
            PlayerManager.Instance.SoulCollection();
            //DebugPanel.Instance.UpdateMessage("soul captured ");
            Destroy(gameObject);
        }


    }
}
