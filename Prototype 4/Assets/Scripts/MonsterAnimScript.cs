using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MonsterAnimScript : MonoBehaviour
{

    private Animator monsterAnim;
    private AnimatorStateInfo stateInfo;

    // Start is called before the first frame update
    void Start()
    {
        monsterAnim = GetComponent<Animator>();
        stateInfo = monsterAnim.GetCurrentAnimatorStateInfo(0);
    }

    public enum AnimState 
    {
        Idle,
        Walk,
        Run
    }
    public AnimState currentAnimation;
    

    public void PlayAnimation(AnimState input)
    {
        //Debug.Log("Playing Animation: " + input);
        if(monsterAnim) {
            currentAnimation = input;
            switch (input)
            {
                case AnimState.Idle:
                    //Debug.Log("Idle animation is active: " + stateInfo.IsTag("Idle"));
                    if (!stateInfo.IsTag("Idle"))
                    {
                        monsterAnim.SetTrigger("TrIdle");
                    }
                    break;
                case AnimState.Walk:
                    //Debug.Log("Walk animation is active: " + stateInfo.IsTag("Walk"));
                    if (!stateInfo.IsTag("Walk"))
                    {
                        monsterAnim.SetTrigger("TrWalk");
                    }
                    break;
                case AnimState.Run:
                    //Debug.Log("Run animation is active: " + stateInfo.IsTag("Run"));
                    if (!stateInfo.IsTag("Run"))
                    {
                        monsterAnim.SetTrigger("TrRun");
                        //Debug.Log("Run animation is active POST TRIGGER: " + stateInfo.IsTag("Run"));
                    }
                    break;
            }
        }
    }

    void Update()
    {
        stateInfo = monsterAnim.GetCurrentAnimatorStateInfo(0);
    } 


}
