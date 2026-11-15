using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class boss : MonoBehaviour
{

    private main controller;
    private GameObject player;
    private Vector3 initpos;
    bool myattack = false;
    int health = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<main>();
        initpos = transform.position;
        GetComponent<SpriteRenderer>().color = Color.white;
        health = 3;
    }
    void Update()
    {
        if(vulnerable_timer > 0)
        {
            vulnerable_timer -= Time.deltaTime;
            if(vulnerable_timer <= 0)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        if(parried_timer > 0)
        {
            parried_timer -= Time.deltaTime;
        }
        if (controller.respawning)
        {
            Debug.Log("Holy respawn");
            gameObject.tag = "Untagged";
            transform.position = initpos;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            GetComponent<SpriteRenderer>().color = Color.white;
            StopAllCoroutines();
            myattack = false;
        }
    }
    private float parried_timer = 0;
    private float vulnerable_timer = 0;
    public void Parried()
    {
        parried_timer = 0.2f;
        vulnerable_timer = 3.5f;
        GetComponent<SpriteRenderer>().color = Color.blue;
        myattack = false;
        gameObject.tag = "Untagged";
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }
    IEnumerator Attacked()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.5f);
        myattack = true;
        gameObject.tag = "Respawn";
        Debug.Log("bost");
        GetComponent<Rigidbody2D>().AddForce(400 * (player.transform.position - transform.position), ForceMode2D.Impulse);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            if (controller.boosted)
            {
                if(vulnerable_timer <= 0)
                {
                    GetComponent<Rigidbody2D>().AddForce(-300 * (player.transform.position - transform.position), ForceMode2D.Impulse);
                    controller.bounces = 10;
                    StartCoroutine(Attacked());
                }
                else
                {
                    health--;
                    if(health <= 0)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                }
            }
            else if(parried_timer <= 0)
            {
                controller.Respawn();
            }
        }
    }
}
