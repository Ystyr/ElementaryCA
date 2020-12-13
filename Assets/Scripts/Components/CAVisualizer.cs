using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Renders CA Data to a texture and outputing it to the UI layout.
/// </summary>
public class CAVisualizer : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField]
    RawImage output;

    [SerializeField]
    bool refresh;
    #pragma warning restore 0649


    private void OnValidate () {
        if (refresh || output && output.texture != null) {
            var tex = new Texture2D(
                ElementaryCA.longBitsNum, ElementaryCA.maxHistLen
                );
            tex.filterMode = FilterMode.Point;
            tex.anisoLevel = 0;
            output.texture = tex;
            refresh = false;
        }
    }


    public void RefreshTex (bool[] values)
    {
        var tex = output.texture as Texture2D;
        var pixs = values.Select(
            v => new Color(
                Convert.ToSingle(v), 0, 0, 1
                )
            );
        tex.SetPixels(pixs.ToArray());
        tex.Apply();
    }
}
