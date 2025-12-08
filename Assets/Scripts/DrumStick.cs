using UnityEngine;

/// <summary>
/// Attach this to each physical drum stick object (or the stick collider root).
/// Set Side to Left or Right in the Inspector so hit zones can distinguish sticks.
/// </summary>
public class DrumStick : MonoBehaviour
{
    public enum Side { Left, Right }
    public Side side = Side.Left;
}