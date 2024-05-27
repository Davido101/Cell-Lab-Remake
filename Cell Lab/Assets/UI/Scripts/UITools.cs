using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class UITools : object
{
    public static Vector2 GetRenderedValues(this TMP_Text textMeshPro, string text, float maxWidth = Mathf.Infinity, float maxHeight = Mathf.Infinity, bool onlyVisibleCharacters = true)
    {
        if (string.IsNullOrEmpty(text)) return Vector2.zero;

        var originalRenderMode = textMeshPro.renderMode;
        var originalText = textMeshPro.text;
        var originalDeltaSize = textMeshPro.rectTransform.sizeDelta;

        textMeshPro.renderMode = TextRenderFlags.DontRender;
        textMeshPro.text = text;
        textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
        textMeshPro.ForceMeshUpdate(true);

        if (text.Length == 0) return Vector2.zero;

        var renderedSize = textMeshPro.GetRenderedValues(onlyVisibleCharacters);
        if (IsInvalidFloat(renderedSize.x) || IsInvalidFloat(renderedSize.y))
        {
            var preferredSize = textMeshPro.GetPreferredValues(text, maxWidth, maxHeight);
            preferredSize = new Vector2(Mathf.Max(preferredSize.x, 0), Mathf.Max(preferredSize.y, 0));
            if (IsInvalidFloat(renderedSize.x)) renderedSize.x = preferredSize.x;
            if (IsInvalidFloat(renderedSize.y)) renderedSize.y = preferredSize.y;
        }

        bool IsInvalidFloat(float f) { return float.IsNaN(f) || f == Mathf.Infinity || f < 0; }

        textMeshPro.renderMode = originalRenderMode;
        textMeshPro.text = originalText;
        textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalDeltaSize.x);
        textMeshPro.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalDeltaSize.y);
        textMeshPro.ForceMeshUpdate(true);

        return renderedSize;
    }
}
