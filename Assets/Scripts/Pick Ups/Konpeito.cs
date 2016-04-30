// Collectible class
// 

using UnityEngine;
using System.Collections;

public class Konpeito : BasePickUp
{
    #region Inspector
    [SerializeField]
    Material[] ColorSet;
        
    [SerializeField]
    int value = 1;
    #endregion

    // Used to change mesh's material colour
    MeshRenderer m_meshRend;
    // Used to change halo colour
    Light m_light;

    public override void Awake()
    {
        m_meshRend = GetComponent<MeshRenderer>();
        m_light = GetComponent<Light>();

        base.Awake();
    }

    public override void Start()
    {
        //RandomizeColor();

        base.Start();
    }

    protected override void PickUp()
    {
        PlayerController.Instance.AddKonpeito(value);

        base.PickUp();
    }

    void RandomizeColor()
    {
        int newColor = Random.Range(0, ColorSet.Length - 1);

        if (ColorSet[newColor] == null)
        {
            Debug.LogWarning("Konpeito ColorSet not complete");
            return;
        }

        m_meshRend.material = ColorSet[newColor];
        m_light.color = ColorSet[newColor].color;   // Use this with emissive shader
        //m_light.color = ColorSet[newColor].GetColor("_ReflectionTint");   // Use this is using reflective shader
    }
}