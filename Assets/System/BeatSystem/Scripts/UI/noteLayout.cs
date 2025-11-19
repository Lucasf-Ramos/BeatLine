using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class noteLayout : MonoBehaviour
{
    [Header("Espaçamento")]
    public float gapX = 10f;
    public float gapY = 10f;

    [Header("Padding (Top, Left, Bottom, Right)")]
    public Vector4 padding;

    [Header("Configurações")]
    public bool allowLineBreak = true;

    private List<RectTransform> children = new List<RectTransform>();
    private RectTransform self;

    void OnEnable() => RefreshChildren();
    void OnTransformChildrenChanged() => RefreshChildren();

    [ContextMenu("updateVisual")]
    void RefreshChildren()
    {
        self = GetComponent<RectTransform>();

        children.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var rt = transform.GetChild(i) as RectTransform;
            if (rt != null)
            {
                // Garantimos que cada filho parte do topo-esquerda
                rt.pivot = new Vector2(0f, 1f);
                children.Add(rt);
            }
        }

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (self == null) return;

        // Largura interna do pai
        float maxRowWidth = self.rect.width;

        // Ponto inicial no topo-esquerda do pai
        float startX = padding.y;
        float startY = -padding.x;

        float x = startX;
        float y = startY;

        float currentRowHeight = 0f;

        foreach (var child in children)
        {
            Vector2 size = child.sizeDelta;

            // Quebra de linha baseada na largura do RECT do pai
            if (allowLineBreak && x + size.x > maxRowWidth - padding.w)
            {
                x = startX;
                y -= currentRowHeight + gapY;
                currentRowHeight = 0f;
            }

            // Posiciona o filho no espaço local do pai
            child.anchoredPosition = new Vector2(x, y);

            x += size.x + gapX;

            if (size.y > currentRowHeight)
                currentRowHeight = size.y;
        }
    }
}
