using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bestagon
{

    public static class RectExtensions
    {
        /// <summary>
        /// Subdivides a rect into smaller rects along its width, with room for padding. Fill in desired widths equal to number of subdivisions, null for flex/fill
        /// </summary>
        /// <param name="originalRect"></param>
        /// <param name="padding"></param>
        /// <param name="rects"></param>
        /// <param name="desiredWidths">Specifies a desired with, null for flexible</param>
        /// <returns></returns>
        public static bool WidthSubdivide(this Rect originalRect, float padding, out Rect[] rects, params float?[] desiredWidths)
        {
            //Count the amount of flex spots and the total width used by each flex spot
            float flexWidth = originalRect.width - ((desiredWidths.Length - 1) * padding);
            {
                int flexibleCount = 0;
                for (int i = 0; i < desiredWidths.Length; i++)
                {
                    if (desiredWidths[i] == null)
                    {
                        flexibleCount++;
                        continue;
                    }
                    flexWidth -= (float)desiredWidths[i];
                }
                flexWidth /= (float)flexibleCount;

                //Check if no room left to draw flex elements with the desired space
                if (flexWidth <= float.Epsilon)
                {
                    rects = new Rect[0];
                    return false;
                }
            }
            float consumedWidth = 0.0f;
            //Establish positions
            rects = new Rect[desiredWidths.Length];
            for (int i = 0; i < rects.Length; i++)
            {
                float usedWidth = desiredWidths[i] ?? flexWidth;
                rects[i] = new Rect(originalRect.x + consumedWidth, originalRect.y, usedWidth, originalRect.height);
                consumedWidth += usedWidth + padding;
            }

            return true;
        }
    }
}
