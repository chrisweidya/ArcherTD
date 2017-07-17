using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCollisionDetection : MonoBehaviour {

    [SerializeField]
    private float damage;
    private PlayerProperties playerProps;
    private CreatureHandler handler;
    private bool collided;
    public GameManager.Factions faction;
    GameObject parentGameObject;
    private CreatureHandler creatureHandler;
    void Start () {
        //playerProps = GetComponent<PlayerProperties>();
        collided = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject.layer == 8) {
            parentGameObject = other.gameObject.GetComponent<BodyPartScript>().ParentGameObject;
            handler = parentGameObject.GetComponent<CreatureHandler>();
            if (handler != null && faction != parentGameObject.GetComponent<PlayerProperties>().GetFaction() && !collided) {
                //Debug.Log("Collsion Event");
                EventManager.FireDoDamage(damage, handler.netId);
                creatureHandler = parentGameObject.GetComponent<CreatureHandler>();
                collided = true;
                StartCoroutine(DestroyArrowOnIsDead());
            }
        }
    }

    private IEnumerator DestroyArrowOnIsDead() {

        while (true) {

            if (creatureHandler.GetIsDead()) {
                yield return new WaitForSeconds(1.5f);
                Destroy(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(1.5f);
        }


    }
}
