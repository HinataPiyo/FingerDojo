using UnityEngine;
namespace EnemyAction
{

    /// <summary>
    /// 敵の攻撃ステートを管理するクラス
    /// </summary>
    public class Attack : MonoBehaviour, ICommand
    {
        EnemyStateController e_StateCtrl;

        void Awake()
        {
            e_StateCtrl = GetComponent<EnemyStateController>();
        }

        /// <summary>
        /// 与えるダメージ量を確定(アニメーションイベントでモーションに合わせて攻撃を適用)
        /// </summary>
        public void DecideDamageAmount()
        {
            float range = Random.value;
            CombatCalculator.instance.EnemyToPlayerAttack(range + Random.Range(0.3f, 0.7f));        // 攻撃力の強さが決まる
        }

        public void Execute()
        {
            e_StateCtrl.E_Animator.SetTrigger("Attack");
        }
    }
}