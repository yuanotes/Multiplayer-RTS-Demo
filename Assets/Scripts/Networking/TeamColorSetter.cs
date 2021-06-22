using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] renderers = new Renderer[0];
    private RTSPlayer player;

    [SyncVar(hook = nameof(onClientSetTeamColor))]
    private Color teamColor;

    #region Server
    public override void OnStartServer() {
        player = connectionToClient.identity.GetComponent<RTSPlayer>();
        teamColor = player.GetTeamColor();
    }
    #endregion

    #region Client
    private void onClientSetTeamColor(Color oldValue, Color newValue) {
        foreach (var renderer in renderers) {
            renderer.material.SetColor("_Color", newValue);
        }
    }
    #endregion
}
