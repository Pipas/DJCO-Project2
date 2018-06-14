﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : Movement 
{
    public HealthSystem playerHealth;
    public bool running = false;
    public bool autoMoving = false;
    void Start ()
    {
        savedPosition = transform.position;
        state = State.still;
    }
    void Update ()
    {	
        if(!playerHealth.gameOver)
        {
            if (autoMoving)
            {
                if (transform.hasChanged)
                {
                    transform.hasChanged = false;
                    running = true;
                    movePlayerAnimation();
                }
                else
                {
                    running = false;
                    autoMoving = false;
                    stopPlayerAnimation();
                }
            }

            if (!autoMoving)
            {
                HandleInput(); // Handles the user input, duh
            }

            HandleMovement(); // Updates movement everyframe

            HandlePartyMovement(); // Updates the rest of the party movement, this way it's sequential after the player
        }
    }

    public void movePlayerAnimation()
    {
        if (running)
        {
            transform.Find("Player").GetComponent<Animator>().SetInteger("State", 1);
            running = true;
        }
    }

    public void stopPlayerAnimation()
    {
        if (!running)
        {
            transform.Find("Player").GetComponent<Animator>().SetInteger("State", 0);
            running = false;
        }
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Up"))
        {
            QueueInput(Vector3.forward);

            if (!running)
            {
                running = true;
                movePlayerAnimation();
            }
        }

        if (Input.GetButtonDown("Down"))
        {
            QueueInput(Vector3.back);

            if (!running)
            {
                running = true;
                movePlayerAnimation();
            }
        }

        if (Input.GetButtonDown("Left"))
        {
            QueueInput(Vector3.left);

            if (!running)
            {
                running = true;
                movePlayerAnimation();
            }
        }

        if (Input.GetButtonDown("Right"))
        {
            QueueInput(Vector3.right);

            if (!running)
            {
                running = true;
                movePlayerAnimation();
            }
        }

        if (!Input.GetButton("Up") && !Input.GetButton("Down") && !Input.GetButton("Left") && !Input.GetButton("Right"))
        {
            running = false;
            stopPlayerAnimation();
        }


        if(Input.GetButtonDown("Interact"))
            checkSurroundings();

        if(Input.GetButtonDown("Ditch"))
            DitchPartyMember();
    }

    private void checkSurroundings() // Checks all surrounding blocks and adds party members to party
    {
        Vector3[] directions = {Vector3.forward, Vector3.left, Vector3.back, Vector3.right};
        RaycastHit hit;
        GameObject obstacle = null;
        //Vector3 raycastPosition = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        foreach(Vector3 direction in directions)
        {
            if (Physics.Raycast(transform.position, direction, out hit, 2))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
                obstacle = hit.transform.gameObject;

                if(obstacle.tag == "Party")
                {
                    if(!party.Contains(obstacle))
                    {
                        obstacle.GetComponent<BoxCollider>().enabled = false;
                        party.Add(obstacle);
                        if(party.Count == 1)
                        {
                            nextInParty = obstacle;
                        }
                        else party[party.IndexOf(obstacle) - 1].GetComponent<PartyMovement>().nextInParty = obstacle;
                    }
                }
            }
        }
    }

    private void HandlePartyMovement()
    {
        foreach(GameObject member in party.ToArray())
            member.GetComponent<PartyMovement>().HandleMovement();
    }

    private void DitchPartyMember()
    {
        for (int i = party.Count - 1; i >= 0; i--)
        {
            if(party[i].GetComponent<PartyMovement>().toBeDitched == false)
            {
                party[i].GetComponent<PartyMovement>().toBeDitched = true;
                if(!party[i].GetComponent<PartyMovement>().isMoving)
                    party[i].GetComponent<PartyMovement>().CheckIfDitched();
                    
                break;
            }	
        }
    }

    protected override void CheckHoldInput()
    {
        if(Input.GetButton("Up"))
            QueueInput(Vector3.forward);
        if(Input.GetButton("Down"))
            QueueInput(Vector3.back);
        if(Input.GetButton("Left"))
            QueueInput(Vector3.left);
        if(Input.GetButton("Right"))
            QueueInput(Vector3.right);
    }

    public bool isAutoMoving()
    {
        return autoMoving;
    }

    public void setAutoMoving(bool auto)
    {
        autoMoving = auto;
    }
}