using UnityEngine;

public class BlinkTriggerMenu : MonoBehaviour
{    
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("TriggerBlink", 3f, 3f);
    }
    
    void TriggerBlink()
    {
        animator.SetTrigger("Blink");
    }
}
