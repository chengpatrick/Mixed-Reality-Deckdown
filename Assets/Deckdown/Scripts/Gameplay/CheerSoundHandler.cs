using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CheerSoundHandler : NetworkBehaviour
{
    [SerializeField] NetworkRunner NetworkRunner;
    [SerializeField] LobbySharedData LSD;

    [SerializeField] List<AudioSource> CheerList;

    private void PlayCheerAudio(int type)
    {
        PauseCheerAudio();

        CheerList[type].Play();
    }

    private void PauseCheerAudio()
    {
        foreach (var cheer in CheerList)
        {
            cheer.Pause();
        }
    }

    public void CheckCheerSide()
    {
        HealthHandler myHH = LSD.GetMyRef(HasStateAuthority).gameObject.GetComponent<HealthHandler>();
        HealthHandler oppHH = LSD.GetEnemyRef(HasStateAuthority).gameObject.GetComponent<HealthHandler>();

        if (myHH.CurrHealth > oppHH.CurrHealth)
        {
            PlayCheerAudio(myHH.GetMonsterType());
            DebugPanel.Instance.UpdateMessage("CSH: myHH is greater");
        }
        else if (myHH.CurrHealth < oppHH.CurrHealth)
        {
            PlayCheerAudio(oppHH.GetMonsterType());
            DebugPanel.Instance.UpdateMessage("CSH: oppHH is greater");
        }
        else
        {
            PauseCheerAudio();
        }
    }
}
