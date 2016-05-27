using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerCharacter))]
public class CharacterController : MonoBehaviour
{
    [SerializeField]
    KeyCode Left = KeyCode.A;
    [SerializeField]
    KeyCode Right = KeyCode.D;
    [SerializeField]
    KeyCode Up = KeyCode.W;
    [SerializeField]
    KeyCode Down = KeyCode.S;

    [SerializeField]
    KeyCode Attack = KeyCode.K;
    [SerializeField]
    KeyCode Jump = KeyCode.Space;

    [SerializeField]
    bool isOn = true;

    PlayerCharacter m_Character;

    void Start()
    {
        m_Character = gameObject.GetComponent<PlayerCharacter>();
    }
    void Update()
    {

        
        bool jump = false;
        bool attack = false;
        int move = 0;

        if (isOn)
        {

            if (Input.GetKey(Right))
                move = 1;
            else if (Input.GetKey(Left))
                move = -1;
            if (Input.GetKeyDown(Jump))
                jump = true;
            

            if (Input.GetKeyDown(Attack))
                attack = true;
        }

        m_Character.ReceiveInput(move, jump, attack);
    }





}