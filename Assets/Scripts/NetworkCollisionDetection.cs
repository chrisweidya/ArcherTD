using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCollisionDetection : MonoBehaviour {

    [SerializeField]
    private float damage;
    [SerializeField]
    private float towerDamage;
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
        if (faction != PlayerHandler.LocalFaction)
            return;
        if (other.gameObject.layer == 8) {
            parentGameObject = other.gameObject.GetComponent<BodyPartScript>().ParentGameObject;
            handler = parentGameObject.GetComponent<CreatureHandler>();
            if (handler != null && faction != parentGameObject.GetComponent<PlayerProperties>().GetFaction() && !collided) {
                //Debug.Log("Collsion Event");
                collided = true;
                if (other.GetComponent<TowerHandler>() != null) {
                    EventManager.FireDoDamage(towerDamage, handler.netId);
                }
                else {
                    EventManager.FireDoDamage(damage, handler.netId);
                }
                Debug.Log("collided with " + other.gameObject.name);
                creatureHandler = parentGameObject.GetComponent<CreatureHandler>();
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
