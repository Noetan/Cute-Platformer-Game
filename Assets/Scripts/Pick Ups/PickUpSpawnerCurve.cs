// Spawns pickups along the curve
// Designed to be used in the scene editor

using UnityEngine;

public class PickUpSpawnerCurve : MonoBehaviour 
{
    // The pickup to spawn
    [SerializeField]
    PooledDB.PickUp m_item = PooledDB.PickUp.None;
    // How many pickups to spawn
    [SerializeField]
    [Tooltip("Note: Might not spawn evenly with closed lines")]
    int m_amount = 5;
    // Basically is the start and end point on top of each other
    // Since you want to avoid using closed curves but still want a "closed" curve
    [SerializeField]
    bool m_isCircle = false;

    // If false, pickups have neutral rotation, if true, they'll be matched to the curve's normal
    // Used if you want the drop to "hop" in a direction other than up
    // Difficult subject, wont implement unless we actually need it 
    // (like some sort of reverse gravity level?)
    // Though even then might be easier to rotate the camera
    //[SerializeField]
    //bool m_rotateItemToCurve = false;



    BezierCurve m_bezierCurve;

    void Awake()
    {
        m_bezierCurve = GetComponent<BezierCurve>();
    }

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        // How far along the curve we are
        float progress = 0.0f;
        // How much to increment the percentage for each pickup
        float step = 0.0f;

        if (!m_isCircle)
        {
            // m_amount - 1 because we want to both start at 0.0 and end at 1.0
            step = 1f / (m_amount - 1);
        }
        else
        {
            // m_amount because the start and end point are the same
            step = 1f / m_amount;
        }        

        for (int i = 0; i < m_amount; i++)
        {
            Vector3 pos = m_bezierCurve.GetPointAt(progress);

            // Spawn
            GameObject GO = PooledDB.Instance.Spawn(m_item, pos, true);

            progress += step;
        }
    }
}
