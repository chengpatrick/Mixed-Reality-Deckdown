using System.Collections;
using System.Collections.Generic;
using POpusCodec.Enums;
using UnityEngine;

public class BlockProjectile : MonoBehaviour, IBlocker
{
    [SerializeField] bool isGrowing = false;
    [SerializeField] float growthRate;
    [SerializeField] float scaleFactor = 0.05f;
    [SerializeField] Blockbrella bb; 

    [SerializeField] float speed;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (isGrowing && transform.localScale.x < 0.5f)
        {
            transform.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor) * Time.deltaTime;
        }

        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    public void SetBallSpeed(float f)
    {
        speed = f;
    }

    public int OnBlock()
    {
        PlayerStatsRecorder.Instance.BlockConfirm();

        return 0; //return type 0 which is a shield
    }

    public void FoldShield()
    {
        Destroy(gameObject);
    }

    public void Folding()
    {
        bb.Folding();
    }

}
