using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{

  [SerializeField] private GameObject unitBasePrefab = null;
  [SerializeField] private GameObject gameOverHandlerPrefab = null;

  [SerializeField]
  private Color[] teamColors = new Color[]{
    Color.blue, Color.grey, Color.magenta, Color.cyan, Color.yellow
  };
  [SerializeField] private List<Color> colorsOccured = new List<Color>();

  private Color generateRandomColor()
  {
    if (colorsOccured.Count == teamColors.Length)
    {
      return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
    return teamColors[Random.Range(0, teamColors.Length - 1)];
  }
  private Color assignTeamColor()
  {
    Color newColor = generateRandomColor();
    while (colorsOccured.Contains(newColor))
    {
      newColor = generateRandomColor();
    }
    return newColor;
  }

  public override void OnServerAddPlayer(NetworkConnection conn)
  {
    base.OnServerAddPlayer(conn);

    RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
    player.SetTeamColor(assignTeamColor());
    colorsOccured.Add(player.GetTeamColor());

    GameObject unitSpawnerInstance = Instantiate(unitBasePrefab, conn.identity.transform.position, conn.identity.transform.rotation);
    NetworkServer.Spawn(unitSpawnerInstance, conn);
  }

  public override void OnServerSceneChanged(string newSceneName)
  {
    if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
    {
      GameObject instance = Instantiate(gameOverHandlerPrefab);
      NetworkServer.Spawn(instance);
    }
  }
}
