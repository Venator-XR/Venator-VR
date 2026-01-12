using UnityEngine;

public class LightningManager : MonoBehaviour
{
    [SerializeField] private LightningController[] triggers;
    [SerializeField] private Animator[] lightningsTrigger1;
    [SerializeField] private Animator[] lightningsTrigger2;

    private void HandleTrigger0() => PlayLightning(0);
    private void HandleTrigger1() => PlayLightning(1);

    void OnEnable()
    {
        triggers[0].OnLightningStrike += HandleTrigger0;
        triggers[1].OnLightningStrike += HandleTrigger1;
    }
    void OnDisable()
    {
        triggers[0].OnLightningStrike -= HandleTrigger0;
        triggers[1].OnLightningStrike -= HandleTrigger1;
    }

    public void PlayLightning(int trigger)
    {
        Animator[] animators = trigger switch
        {
            0 => lightningsTrigger1,
            1 => lightningsTrigger2,
            _ => new Animator[0]
        };

        int animation = trigger switch
        {
            0 => 3,
            1 => 1,
            _ => 1
        };

        foreach (Animator animator in animators)
        {
            animator.SetTrigger("lightning" + animation);
        }
        triggers[trigger].PlaySFX(animation);
    }
}
