using UnityEngine;

public class LightningManager : MonoBehaviour
{
    [SerializeField] private LightningController[] triggers;
    [SerializeField] private Animator[] lightningsTrigger1;
    [SerializeField] private Animator[] lightningsTrigger2;
    [SerializeField] private Animator[] lightningsTrigger3;

    private void HandleTrigger0() => PlayLightning(0);
    private void HandleTrigger1() => PlayLightning(1);
    private void HandleTrigger2() => PlayLightning(2);

    void OnEnable()
    {
        triggers[0].OnLightningStrike += HandleTrigger0;
        triggers[1].OnLightningStrike += HandleTrigger1;
        triggers[2].OnLightningStrike += HandleTrigger2;
    }
    void OnDisable()
    {
        triggers[0].OnLightningStrike -= HandleTrigger0;
        triggers[1].OnLightningStrike -= HandleTrigger1;
        triggers[2].OnLightningStrike -= HandleTrigger2;
    }

    public void PlayLightning(int trigger)
    {
        Animator[] animators = trigger switch
        {
            0 => lightningsTrigger1,
            1 => lightningsTrigger2,
            2 => lightningsTrigger3,
            _ => new Animator[0]
        };

        int animation = trigger switch
        {
            0 => 3,
            1 => 1,
            2 => 3,
            _ => 1
        };

        foreach (Animator animator in animators)
        {
            animator.SetTrigger("lightning" + animation);
        }
        triggers[trigger].PlaySFX(animation);
    }
}
