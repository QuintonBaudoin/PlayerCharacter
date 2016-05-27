using UnityEngine;
using System.Collections;
using System;

public class PlayerCharacter : MonoBehaviour, IDamageable
{
    [SerializeField] private int _Health    = 1;
    [SerializeField] private int _MaxHealth = 1;
    [SerializeField] private int _Lives     = 1;
    private bool _Alive = true;

    public float m_Speed = 5.0f;

    private float CapsuleHeight;    // For Capsule resizing 
    private Vector3 CapsuleCenter;  // For Capsule resizing

    // public float m_jumpSpeed = 5.0f;
    [SerializeField] private float m_JumpPower = 5.0f;
    private bool m_OnGround;
   
    Rigidbody m_Rigid;  // Our RigidBody    // Grabbed at Start()

    [SerializeField] private GameObject m_WeaponA;

    /// <summary>
    /// IDamageable.Health
    /// 
    /// Used to manage health
    /// </summary>
    public int Health
    {
        get
        {
            return _Health;
        }

        set
        {
            if (value > MaxHealth)
                value = MaxHealth;
            _Health = value;

            if (Health <= 0)
                Lives--;

            //if (GUIManager.Instance != null)
            //    GUIManager.Instance.UpdateHealthBar(Health, MaxHealth);

        }
    }
    /// <summary>
    /// IDamageable.MaxHealth
    /// 
    /// Used to manage max health
    /// </summary>
    public int MaxHealth
    {
        get
        {
            return _MaxHealth;
        }

        set
        {
            if (value <= 0)
                value = 1;
            if (Health > value)
                Health = value;
            _MaxHealth = value;

            //if (GUIManager.Instance != null)
            //    GUIManager.Instance.UpdateHealthBar(Health, MaxHealth);

        }
    }
    /// <summary>
    /// IDamageable.Alive
    /// 
    /// Used to manage Alive
    /// </summary>s
    public bool Alive
    {
        get
        {
            return _Alive;
        }

        set
        {
            _Alive = value;

            if (!Alive)
                OnDeath();

        }
    }

   
   /// <summary>
   /// manages lives
   /// </summary>
    public int Lives
    {
        get { return _Lives; }
        set { _Lives = value; if (_Lives <= 0) NoMoreLives(); else OnDeath(); }
    }


    void Start()
    {
        ///Sets memory for capsule stats
        CapsuleHeight = GetComponent<CapsuleCollider>().height;
        CapsuleCenter = GetComponent<CapsuleCollider>().center;
        ///sets a refrence to rigidbody
        m_Rigid = gameObject.GetComponent<Rigidbody>();
        //Funnels starting stats into health and max health to insure correct stats;
        Health = _Health;
        MaxHealth = _MaxHealth;

        //Sets constraints
        m_Rigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;


    }

    void FixedUpdate()
    {
        CheckForGround();
        ResizeCapsule();
    }

    /// <summary>
    /// Used by the CharacterController
    /// </summary>
    /// <param name="direction">will ither be -1, +1, or 0 to give direction for wich way to move</param>
    /// <param name="jump">Bool to say weather jump key was pressed or not</param>
    /// <param name="attack">bool to say weather attack keys or not</param>
    public void ReceiveInput(int direction, bool jump, bool attack)
    {
        if (direction > 1)
            direction = direction / direction;

        HandleMovement(direction);
        HandleJump(jump);
        HandleAttack(attack);
    }

    /// <summary>
    /// Handles Movement
    /// </summary>
    /// <param name="direction"></param>
    void HandleMovement(float direction)
    {
        Vector3 pos = m_Rigid.position;//gives me position based on rigid body

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack"))   // If attacking, can not move
            direction = 0;  //Sets direction to 0

        float movement = direction * m_Speed * Time.deltaTime;  //Calculates the amount of movement based on direction, speed, and time

        if (Mathf.Abs(movement) > 0)
        {
            GetComponent<Animator>().SetBool("moving", true);///Sets weather moving into animator
            Vector3 forward = gameObject.transform.forward;///sets tmp variable based on our forward
            forward.x = direction;//changes forward based on direction
            gameObject.transform.forward = forward;//sets forward

            pos.x += movement;///ads movment to our position
        }

      else
        { 
            GetComponent<Animator>().SetBool("moving", false);//we not movin
        }

        m_Rigid.MovePosition(pos);///Makes anna move
    }








    /// <summary>
    ///Handles jump
    /// </summary>
    void HandleJump(bool jump)
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack")) //prevents us from jumping when attackign
            jump = false;

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("jump"))///prevents jump when jumping
            jump = false;

        if (m_OnGround && jump)///sees if we are on ground or jumping
        {

            GetComponent<Animator>().SetTrigger("jump"); //triggers jump animation
          
            m_Rigid.AddForce(0, m_JumpPower, 0,ForceMode.Impulse);///Adds force up
        }

    }
    void HandleAttack(bool attack)
    {
        
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack"))//if attacking keeps weapon damage active
        { m_WeaponA.GetComponent<DamagingObject>().active = true; return; }

        if (m_WeaponA == null || m_OnGround == false)///If no weapon or not on ground we cant attack
            return;

        if (m_WeaponA.GetComponent<DamagingObject>())//if weapon has correct script we set it to attack and start attack animatoin
        {
            if (attack == true)
                GetComponent<Animator>().SetTrigger("attack");


        }
    }    
    /// <summary>
    ///Resizes capsule to match jump animation
    /// </summary>
    void ResizeCapsule()
    {
        CapsuleCollider c = GetComponent<CapsuleCollider>();

        if(m_OnGround)
        {
            c.center = CapsuleCenter;
            c.height = CapsuleHeight;
        }

        else
        {
            c.height = 1.15f;
            c.center = new Vector3(CapsuleCenter.x, .56f, CapsuleCenter.z);
        }

    }

    /// <summary>
    /// Checks if we are on ground
    /// </summary>
    void CheckForGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + (Vector3.forward * 0.1f), Vector3.down, out hit, .12f))
        {
            m_OnGround = true;
        }
        else { m_OnGround = false; GetComponent<Animator>().ResetTrigger("attack"); }

        GetComponent<Animator>().SetBool("airborn", !m_OnGround);
    }

    
    /// <summary>
    /// Gives damge
    /// </summary>
    public void TakeDamage(GameObject other)
    {
        Health--;
    }
    /// <summary>
    /// when die(but with lives)
    /// </summary>
    public void OnDeath()
    {  
      //  GameManager.PlayerRecall();
    }
    /// <summary>
    /// when lives hit 0
    /// </summary>
    void NoMoreLives()
    {
     //   GameManager.ResetLevel(); 
    }
    
   

}
 