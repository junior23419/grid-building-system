using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollider : MonoBehaviour
{
    [SerializeField] Material[] materials;

   

    private void OnTriggerStay(Collider other)
    {
        if(other.tag =="Building")
        {

            transform.parent.GetComponent<Building>().SetDeployable(false);
            gameObject.GetComponent<Renderer>().material = materials[1];
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Building")
        {

            transform.parent.GetComponent<Building>().SetDeployable(true);
            gameObject.GetComponent<Renderer>().material = materials[0];
        }
    }
}
