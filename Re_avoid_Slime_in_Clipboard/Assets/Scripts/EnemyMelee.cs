using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : MonoBehaviour
{
    public enum TypeMonster { A, B };
    public TypeMonster enemyType;
    //public enum Type { Melee, Range };
    //public Type type;
    public int damage;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public bool isAttack;
    public bool isChase;

    

    Rigidbody rigid;
    BoxCollider boxcollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxcollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();


        Invoke("ChaseStart", 8);

    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if (isChase)
        {
            nav.SetDestination(target.position);
            
        }
    }

    void FreezeVelocity()
    {
        {
            if (isChase)
            { 
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            }
        }

    }

    void Targerting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyType)
        {
            case TypeMonster.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case TypeMonster.B:
                targetRadius = 1f;
                targetRange = 6f;
                break;


        }

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, 
            targetRadius, transform.forward, 
            targetRange, LayerMask.GetMask("Player"));

        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case TypeMonster.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                break;

            case TypeMonster.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
        }
        

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }
    void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));

        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.gray;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;
            isChase = false;

            anim.SetTrigger("doDie");
            Player player = target.GetComponent<Player>();
            


            nav.enabled = false;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            Destroy(gameObject, 1);
        }
    }
}
