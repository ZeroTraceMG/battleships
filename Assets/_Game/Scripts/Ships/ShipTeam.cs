using UnityEngine;

namespace IronTide.Ships
{
    /// <summary>
    /// Simple team identifier component. Attached to every ship prefab.
    /// Set by GameManager at spawn time.
    /// </summary>
    public class ShipTeam : MonoBehaviour
    {
        public int TeamId { get; private set; } = -1;

        public void SetTeam(int teamId) => TeamId = teamId;
    }
}
