using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTSPlayer player;

    private void Update()
    {
        if (player == null) {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            if (player != null) {
                player.ClientUpdateResourcesEvent += onUpdateResources;
                onUpdateResources(player.GetResources());
            }
        }
    }

    private void OnDestroy() {
        player.ClientUpdateResourcesEvent -= onUpdateResources;
    }

    private void onUpdateResources(int newResources) {
        resourcesText.text = $"Resources: {newResources}";
    }
}
