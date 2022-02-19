using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
#endif


public class AgentAnimationUpdater : MonoBehaviour
{
    NavMeshAgent agent;
	Animator animator;

	// animation Parameter Names
	[SerializeField] private string forwardSpeed = "Speed";
	[SerializeField] private string angularSpeed = "Turn";

	public bool movementHandeledByAnimator = false;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDAngularSpeed;

	private void Awake()
	{
		AssignAnimationIDs();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash(forwardSpeed);
		_animIDAngularSpeed = Animator.StringToHash(angularSpeed);
	}



	/// <summary>
	/// Updates the animation parameters for the blend trees
	/// </summary>
	private void UpdateAnimatorParameters()
	{
		animator.SetFloat(_animIDSpeed, agent.velocity.magnitude);
	}

    private void Update()
    {
        UpdateAnimatorParameters();
    }
}
