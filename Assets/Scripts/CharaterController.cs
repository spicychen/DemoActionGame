using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CustomCC
{
public class CharaterController : MonoBehaviour
{
    public float movement_speed;
        public int health;
        public int attack;
        public int defense;
        public int max_health;

    public Transform enemy;

    public float attackRange = 2f;
    public LayerMask enemy_layer;


    Vector3 movement;

    private Rigidbody rb;
    private Animator anime;

        private int attack_up_rate = 1;
        private int defense_up_rate = 1;
        private float speed_up_rate = 1f;

        private int received_attack;

    const float MOVE_THRE=0.25f;

    // Start is called before the first frame update
    void Start()
    {
            health = max_health;
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        //anime.SetTrigger("ultimate");
    }

    private float h;

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");

        if (Math.Abs(h) > MOVE_THRE)
        {
            anime.SetBool("moving",true);
        }
        else
        {
            anime.SetBool("moving", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anime.SetTrigger("jump");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            anime.ResetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            anime.SetTrigger("punch");
            //Attack();
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            anime.ResetTrigger("punch");
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            anime.SetBool("defend",true);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            anime.SetBool("defend",false);
        }

        
        if (Input.GetKeyDown(KeyCode.L))
        {
            anime.SetBool("ult",true);
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            anime.SetBool("ult",false);
        }


        MoveForward(h);
    }

    private void FixedUpdate()
    {

        if (enemy.position.x - transform.position.x < 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(90f, 0f, 0f), Vector3.up);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-90f, 0f, 0f), Vector3.up);

        }
    }

    void MoveForward(float h)
    {
        movement.Set(h, 0f, 0f);
        movement = movement.normalized;
        movement = movement * movement_speed * speed_up_rate * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    void Attack(int a)
    {
        Collider[] hit_enemies = Physics.OverlapSphere(transform.position, attackRange, enemy_layer);
        foreach(Collider enemy in hit_enemies)
        {
            //Debug.Log(enemy.name);
            EnemyController e = enemy.GetComponent<EnemyController>();
            if (e != null)
            {
                e.TakeDmg(a*attack*attack_up_rate);

            }
        }
    }

    public void TakeDmg(int attack)
    {
        anime.SetTrigger("hit");

            received_attack = attack;

        }


    public void CalculateDmg(int dmg_blocked)
    {

            int dmg = received_attack - (this.defense + dmg_blocked) * defense_up_rate;
            if (dmg < 0)
            {
                dmg = 0;
            }
            health -= dmg;

            if (health <= 0)
            {
                gameObject.SetActive(false);
                FindObjectOfType<GameController>().GameOver();
            }
    }

        public void UseAttackUp()
        {
            attack_up_rate = 2;
        }
        public void UseDefenseUp()
        {
            defense_up_rate = 2;
        }
        public void UseSpeedUp()
        {
            speed_up_rate = 1.5f;
        }

        public void DrinkPotion()
        {
            int total_health = health + 50;
            if (total_health > max_health)
            {
                total_health = max_health;
            }
            health = total_health;
        }


    }


}
