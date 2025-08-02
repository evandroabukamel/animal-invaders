using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Utilities
{
    public static class ScreenshotUtils
    {
        [MenuItem("Tools/Screenshots/Apple Screenshots Portrait")]
        public static void ScreenshotApplePortrait()
        {
            Screenshot(1242, 2208); // 5.5"
            Screenshot(1242, 2688); // 6.5"
            Screenshot(1290, 2796); // 6.7"
            Screenshot(2048, 2732);
            Screenshot(2048, 2732, "ipadPro129");
        }

        [MenuItem("Tools/Screenshots/Apple Screenshots Landscape")]
        public static void ScreenshotAppleLandscape()
        {
            Screenshot(2208, 1242); // 5.5"
            Screenshot(2688, 1242); // 6.5"
            Screenshot(2796, 1290); // 6.7"
            Screenshot(2732, 2048);
            Screenshot(2732, 2048, "ipadPro129");
        }

        static void Screenshot(int tw, int th, string suffix = "")
        {
            RenderTexture rt = new RenderTexture(tw, th, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 4;

            var activeCamerasByDepth = Camera.allCameras.ToList()
                .Where(c => c.isActiveAndEnabled)
                .OrderBy(c => c.depth);
            foreach (var camera in activeCamerasByDepth)
            {
                var previousTargetTexture = camera.targetTexture;
                camera.targetTexture = rt;
                camera.Render();

                camera.targetTexture = previousTargetTexture;
            }

            Texture2D thumb = new Texture2D(tw, th, TextureFormat.RGB24, false);
            RenderTexture.active = rt;
            thumb.ReadPixels(new Rect(0, 0, tw, th), 0, 0, false);

            byte[] bytes = thumb.EncodeToJPG(90);
            Object.DestroyImmediate(thumb);

            File.WriteAllBytes(Application.dataPath + $"/Metadata/Screenshots/Screenshot-{tw}x{th}-{DateTime.Now.Ticks}_{suffix}.jpg", bytes);

            RenderTexture.active = null;

            rt.DiscardContents();
        }
    }
}
