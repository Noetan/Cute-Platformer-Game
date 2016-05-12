// Spawns pickups along the curve
// Designed to be used in the scene editor

using UnityEngine;

public class PickUpSpawnerCurve : MonoBehaviour 
{
    // The pickup to spawn
    [SerializeField]
    PickUpDB.Type m_item = PickUpDB.Type.none;
    // How many pickups to spawn
    [SerializeField]
    [Tooltip("Note: Might not spawn evenly with closed lines")]
    int m_amount = 5;
    // If false, pickups have neutral rotation, if true, they'll be matched to the curve's normal
    [SerializeField]
    bool m_rotateItemToCurve = false;

    // 
    [SerializeField]
    bool m_isCircle = false;

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
        // How much to increment the percentage for each pickup
        float step;
        if (!m_isCircle)
        {
            // m_amount - 1 because we want to both start at 0.0 and end at 1.0
            step = 1f / (m_amount - 1); ;
        }
        else
        {
            // m_amount because the start and end point are the same
            step = 1f / m_amount;
        }
        // How far along the curve we are
        float progress = 0.0f;

        for (int i = 0; i < m_amount; i++)
        {
            Vector3 pos = m_bezierCurve.GetPointAt(progress);
            

            Debug.Log("progress " + i + ": " + progress);
            //Debug.Log("pos " + i + ": " + pos);

            // Spawn
            GameObject GO = PickUpDB.Instance.Spawn(m_item);
            GO.transform.position = pos;
            GO.SetActive(true);

            progress += step;
        }
    }
}
