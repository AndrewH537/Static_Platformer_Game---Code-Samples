using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityUnlock : MonoBehaviour
{
    public bool unlockDoubleJump, unlockDash, unlockBecomeBall, unlockDropBomb;

    public GameObject pickupEffect;

    public string unlockedMessage;
    public TMP_Text unlockText;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player"){
            PlayerAbilityTracker player = other.GetComponentInParent<PlayerAbilityTracker>();

            if(unlockDoubleJump)
            {
                player.canDoubleJump = true;
            }
            if(unlockDash)
            {
                player.canDash = true;
            }
            if(unlockBecomeBall)
            {
                player.canBecomeBall = true;
            }
            if(unlockDropBomb)
            {
                player.canDropBomb = true;
            }

            Instantiate(pickupEffect, transform.position, transform.rotation);

            unlockText.transform.parent.SetParent(null);
            unlockText.transform.parent.position = transform.position;

            unlockText.text = unlockedMessage;
            unlockText.gameObject.SetActive(true);

            Destroy(unlockText.transform.parent.gameObject, 2.5f);

            Destroy(gameObject);
        }
    }
}
