using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsRecorder : MonoBehaviour
{
    public static PlayerStatsRecorder Instance { get; private set; }

    [SerializeField] GameObject Scoreboard;
    [SerializeField] TextMeshProUGUI attackUsedText, confirmHitText, confirmBlockText, specialUsedText;
    private int attackUsed, confirmHit, confirmBlock, specialUsed; 

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScoreboard(float delay)
    {
        Scoreboard.SetActive(true);
        attackUsedText.text = attackUsed.ToString();
        confirmHitText.text = confirmHit.ToString();
        confirmBlockText.text = confirmBlock.ToString();
        specialUsedText.text = specialUsed.ToString();

        //disable scoreboard after [delay] seconds
        Invoke("disableScoreboard", delay);
    }

    public void UsedAttack()
    {
        attackUsed++; 
    }

    public void HitConfirm()
    {
        confirmHit++;
    }

    public void BlockConfirm() 
    {
        confirmBlock++;
    }

    public void UsedSpecial()
    {
        specialUsed++;
    }

    private void disableScoreboard()
    {
        Scoreboard.SetActive(false);
    }
}
