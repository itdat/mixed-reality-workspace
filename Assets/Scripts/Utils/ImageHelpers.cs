using UnityEngine;

namespace Utils {
    public class ImageHelpers {
        //https://answers.unity.com/questions/1008802/merge-multiple-png-images-one-on-top-of-the-other.html
        public static Texture2D AlphaBlend(Texture2D aBottom, Texture2D aTop) {
            if (aBottom.width != aTop.width || aBottom.height != aTop.height)
                throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
            var bData = aBottom.GetPixels();
            var tData = aTop.GetPixels();
            var count = bData.Length;
            var rData = new Color[count];
            for (var i = 0; i < count; i++) {
                var B = bData[i];
                var T = tData[i];
                var srcF = T.a;
                var destF = 1f - T.a;
                var alpha = srcF + destF * B.a;
                var R = (T * srcF + B * B.a * destF) / alpha;
                R.a = alpha;
                rData[i] = R;
            }

            var res = new Texture2D(aTop.width, aTop.height);
            res.SetPixels(rData);
            res.Apply();
            return res;
        }

        //https://answers.unity.com/questions/1276548/crop-texture2d-from-center.html
        public static Texture2D ResampleAndCrop(Texture2D source, int targetWidth, int targetHeight) {
            var sourceWidth = source.width;
            var sourceHeight = source.height;
            var sourceAspect = (float) sourceWidth / sourceHeight;
            var targetAspect = (float) targetWidth / targetHeight;
            var xOffset = 0;
            var yOffset = 0;
            float factor = 1;
            if (sourceAspect > targetAspect) { // crop width
                factor = (float) targetHeight / sourceHeight;
                xOffset = (int) ((sourceWidth - sourceHeight * targetAspect) * 0.5f);
            }
            else { // crop height
                factor = (float) targetWidth / sourceWidth;
                yOffset = (int) ((sourceHeight - sourceWidth / targetAspect) * 0.5f);
            }

            var data = source.GetPixels32();
            var data2 = new Color32[targetWidth * targetHeight];
            for (var y = 0; y < targetHeight; y++) {
                for (var x = 0; x < targetWidth; x++) {
                    var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1),
                        Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                    // bilinear filtering
                    var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                    data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
                }
            }

            var tex = new Texture2D(targetWidth, targetHeight);
            tex.SetPixels32(data2);
            tex.Apply(true);
            return tex;
        }
    }
}
