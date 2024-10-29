using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoolBehaviour : StateMachineBehaviour
{
    public string parameterName = "canMove";
    public bool updateOnState = false;
    public bool updateOnStateMachine = true;
    public bool valueOnEnter = false;
    public bool valueOnExit = true;

    // Helper method to check if the Animator has a parameter with the specified name and type
    private bool HasParameter(Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName && param.type == type)
            {
                return true;
            }
        }
        return false;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (parameterName != null && HasParameter(animator, parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, valueOnEnter);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (parameterName != null && HasParameter(animator, parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, valueOnExit);
        }
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine && parameterName != null && HasParameter(animator, parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, valueOnEnter);
        }
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine && parameterName != null && HasParameter(animator, parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, valueOnExit);
        }
    }
}
