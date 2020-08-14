using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomCC;

public class EnemyController : MonoBehaviour
{

    public Transform player;
    public Animator player_animator;

    private Rigidbody rb;
    private Animator anime;

    Vector3 movement;
    public float movement_speed;
    public float fly_speed;
    public int health;
    public int attack;
    public int defense;
    public int max_health;

    public float h;

    public float attackRange = 3f;
    public LayerMask player_layer;

    public GameController game_controller;
    const float MOVE_THRE = 2f;

    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        h = player.position.x - transform.position.x;


        if (Math.Abs( h) > MOVE_THRE)
        {
            MoveForward(h);
            anime.SetBool("walk", true);
        }
        else
        {
            anime.SetBool("walk", false);
            if (player_animator.GetCurrentAnimatorStateInfo(0).IsName("punch"))
            {
                anime.ResetTrigger("attack");
                FlyForward();
            }
            else
            {
                anime.ResetTrigger("fly");
                anime.ResetTrigger("fly_back");
                anime.SetTrigger("attack");
            }
        }
    }

    private void FixedUpdate()
    {
        
        if (h < 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(90f,0f,0f), Vector3.up);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-90f, 0f, 0f), Vector3.up);

        }
    }

    void Attack()
    {

        Collider[] hit_enemies = Physics.OverlapSphere(transform.position, attackRange, player_layer);
        foreach (Collider enemy in hit_enemies)
        {
            //Debug.Log(enemy.name);
            CustomCC.CharaterController e = enemy.GetComponent<CustomCC.CharaterController>();
            if (e != null)
            {
                e.TakeDmg(this.attack);

            }
        }
    }

    void MoveForward(float h)
    {
        movement.Set(h, 0f, 0f);
        movement = movement.normalized;
        movement = movement * movement_speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    void FlyForward()
    {
        //if(anime.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        anime.SetTrigger("fly");
        //if(anime.GetCurrentAnimatorStateInfo(0).IsName("fly"))
        anime.SetTrigger("fly_back");


        //movement.Set(h, 0f, 0f);
        //movement = movement.normalized;
        //movement = movement * fly_speed * Time.deltaTime;
    }

    public void TakeDmg(int attack)
    {
        anime.SetTrigger("hit");
        int dmg = attack - this.defense;
        if (dmg < 0)
        {
            dmg = 0;
        }
        health -= dmg;
        game_controller.AddScore(dmg);
    }
}
