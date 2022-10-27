using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    public PlayerController m_PlayerController;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask OtherPlayerLayer;

    int baseAttackDamage = 20;
    int m_HashParameterDamaged = Animator.StringToHash("Damaged");
    int m_HashParameterIsInCombo = Animator.StringToHash("IsInCombo");
    int m_HashParameterReceivingDamage = Animator.StringToHash("ReceivingDamage");

    void EndCombo()
    {
        m_PlayerController.isInCombo = false;
        m_PlayerController.canMove = true;
        m_Animator.SetBool(m_HashParameterIsInCombo, m_PlayerController.isInCombo);
    }

    void SetHitBox()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, OtherPlayerLayer);

        foreach(Collider enemy in hitEnemies)
        {
            Animator enemyAnimator = enemy.GetComponentInChildren<Animator>();
            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit_1") || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit_2") || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit_3"))
            {
                enemyAnimator.SetBool(m_HashParameterReceivingDamage, true);
            }
            else
            {
                enemyAnimator.SetBool(m_HashParameterReceivingDamage, false);
            }
            enemy.GetComponentInChildren<Animator>().SetTrigger(m_HashParameterDamaged);
            enemy.GetComponent<PlayerResources>().SetHealth(-baseAttackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
