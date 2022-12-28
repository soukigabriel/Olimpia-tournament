using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerResources))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    public PlayerController m_PlayerController;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask OtherPlayerLayer;

    [SerializeField] int baseAttackDamage = 20;
    int m_HashParameterDamaged = Animator.StringToHash("Damaged");
    int m_HashParameterIsInCombo = Animator.StringToHash("IsInCombo");
    int m_HashParameterReceivingDamage = Animator.StringToHash("ReceivingDamage");

    public void EndCombo()
    {
        m_PlayerController.isInCombo = false;
        m_PlayerController.canMove = true;
        m_Animator.SetBool(m_HashParameterIsInCombo, m_PlayerController.isInCombo);
    }

    void SetHitBox()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, OtherPlayerLayer);
        m_PlayerController.PlayRandomAttackAudio();
        foreach(Collider enemy in hitEnemies)
        {
            Animator enemyAnimator = enemy.GetComponentInChildren<Animator>();
            PlayerController enemyPlayerController = enemy.GetComponent<PlayerController>();
            PlayerResources enemyPlayerResources = enemy.GetComponent<PlayerResources>();

            //Añadir a esta condición que el enemigo no se este defendiendo

            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit_1") || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit_2") || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit_3"))
            {
                enemyAnimator.SetBool(m_HashParameterReceivingDamage, true);
            }
            else
            {
                enemyAnimator.SetBool(m_HashParameterReceivingDamage, false);
            }
            if (enemy.GetComponent<PlayerResources>().CurrentHealth > 0)
            {
                enemyAnimator.SetTrigger(m_HashParameterDamaged);
                if(!enemyPlayerController.onGuard)
                {
                    enemyPlayerController.PlayRandomHurtAudio();
                    enemyPlayerResources.SetHealth(-baseAttackDamage);
                }
            }
        }
    }



    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
