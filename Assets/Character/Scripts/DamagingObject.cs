

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DamagingObject : MonoBehaviour
{
    bool _active = false;

  //  Dictionary<GameObject, Coroutine> storedColliders = new Dictionary<GameObject, Coroutine>();
    Dictionary<GameObject, IEnumerator> StoredColliders = new Dictionary<GameObject, IEnumerator>();
    public bool active
    {
        get
        {
            return _active;
        }
        set
        {
            
            _active  = value;
            
        }
    }
    
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider coll)
    {
        
        
        if (GetComponentInParent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack"))
            active = true;
        else active = false;
        if (coll.isTrigger || coll.GetComponent<IDamageable>() == null)
        return;

        if (!StoredColliders.ContainsKey(coll.gameObject))
        StoredColliders.Add(coll.gameObject, PollDamage(coll));

        
        StartCoroutine(StoredColliders[coll.gameObject]);
        

        
        //coll.gameObject.GetComponent<Rigidbody>().AddForce(GetComponentInParent<Transform>().forward.x * NockBack,0,0);

    }

    void OnTriggerExit(Collider coll)
    {
        
        if (StoredColliders.ContainsKey(coll.gameObject))
        {
          
            StopCoroutine(StoredColliders[coll.gameObject]);

            StoredColliders.Remove(coll.gameObject);
        }

        
    }




    IEnumerator PollDamage(Collider coll)
    {
        while (true)
        {
            if (active)
            {
                coll.GetComponent<IDamageable>().TakeDamage(gameObject);
                StoredColliders.Remove(coll.gameObject); 
                break;
            }
            yield return null;
        }
       
    }
}
