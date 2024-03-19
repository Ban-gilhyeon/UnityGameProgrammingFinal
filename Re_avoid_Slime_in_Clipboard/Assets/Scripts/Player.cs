using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public int maxHealth;
    public int curHealth;
    public int score;
    SkinnedMeshRenderer[] meshs;

    Material mat;
    BoxCollider boxcollider;

    public GameObject[] weapons;
    public bool[] hasWeapons;
    public float jumpPower;
    public float speed;
    float hAxis;
    float vAxis;
    //bool wDown;
    bool jDown;
    bool fDown;
    bool dDown;

    bool isJump;
    bool isDodge;
    bool isFireReady = true;
    bool isDamage;
    bool isBorder;

    int weaponIndex = 0;
    Weapon equipWeapon;



    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<SkinnedMeshRenderer>();

        equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
        equipWeapon.gameObject.SetActive(true);

        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", 11250);
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Dodge();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        dDown = Input.GetButton("Dodge");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;
        if (!isFireReady)
            moveVec = Vector3.zero;

        //transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        transform.position += moveVec * speed * Time.deltaTime;

        anim.SetBool("IsRun", moveVec != Vector3.zero);
        //anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        //if (equipWeapon == null)
        //    return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown)
        {
            equipWeapon.Use();
            anim.SetTrigger("doSwing");
            fireDelay = 3;

        }
    }

    void Dodge()
    {
        if (dDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    void OnCollisionEnter(Collision collision) //유니티에서 태그설정해야함
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }
    void StopTowall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);

    }
    private void FixedUpdate()
    {
        StopTowall();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                //EnemyMelee enemy = other.GetComponent<EnemyMelee>();
                //curHealth -= enemy.damage;
                Bullet enemyBullet = other.GetComponent<Bullet>();
                curHealth -= enemyBullet.damage;
                StartCoroutine(OnDamage());
                
            }
            //else if (other.tag == "Bullet")
            //{
            //    Bullet bullet = other.GetComponent<Bullet>();
            //    curHealth -= bullet.damage;
            //    Vector3 reactVec = transform.position - other.transform.position;
            //    Destroy(other.gameObject);
            //    StartCoroutine(OnDamage(reactVec));
            //}
        }

        IEnumerator OnDamage()
        {
            isDamage = true;

            foreach (SkinnedMeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.red;
            }

            yield return new WaitForSeconds(0.5f);

            isDamage = false;

            if (curHealth > 0)
            {
                foreach (SkinnedMeshRenderer mesh in meshs)
                {
                    mesh.material.color = Color.white;
                }
            }
            else
            {
                foreach (SkinnedMeshRenderer mesh in meshs)
                {
                    mesh.material.color = Color.gray;
                }
                gameObject.layer = 14;
                anim.SetTrigger("doDie");

            }
        }
    }
}
