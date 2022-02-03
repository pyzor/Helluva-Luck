using System.IO;
using UnityEditor;
using UnityEngine;

public class SDFGenerator : EditorWindow {

    private Texture2D sourceTexture;
    private float radius;
    private string outputTextureName;
    private Texture2D outputTexture;
    private bool showPreview = false;

    [MenuItem("Tools/SDF Generator")]
    public static void ShowWindow() {
        GetWindow<SDFGenerator>("Signed Distance Field Generator");
    }


    private void OnGUI() {

        //sourceTexture = (Texture2D)EditorGUI.ObjectField(new Rect(0, 0, 320, 180), "Source Bitmap Texture", sourceTexture, typeof(Texture2D), false);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Bitmap Texture", sourceTexture, typeof(Texture2D), false);
        radius = EditorGUILayout.FloatField("Radius", radius);
        if (radius < 1)
            radius = 1;
        GUILayout.Label("Output Texture Name: " + (sourceTexture ? sourceTexture.name + ".sdf" : ""));
        //outputTextureName = EditorGUILayout.TextField("Output Texture Name", sourceTexture ? sourceTexture.name + ".sdf" : "sdfTexture");

        if (GUI.Button(new Rect((int)(position.width / 2 - 64), 110, 128, 24), "Generate Preview")) {
            if (outputTexture)
                DestroyImmediate(outputTexture);
            if (!sourceTexture) {
                showPreview = false;
            } else {
                outputTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);


                //outputTexture.
                outputTexture.name = sourceTexture.name + ".sdf";
                outputTexture.SetPixels(sourceTexture.GetPixels());

                // SDFGenerator
                SignedDistanceField();

                outputTexture.Apply();
                showPreview = true;
            }
        }
        if (showPreview) {
            //GUILayout.Label("Preview", EditorStyles.boldLabel);
            EditorGUI.DrawPreviewTexture(new Rect((int)(position.width / 2 - 128), 140, 256, 256), outputTexture);


            if (GUI.Button(new Rect((int)(position.width / 2 - 64), 402, 128, 24), "Save Texture")) {

                string path = EditorUtility.SaveFilePanelInProject("Save png", outputTexture.name + ".png", "png", "Please enter a file name to save the texture to");

                if (path.Length != 0) {
                    byte[] pngData = outputTexture.EncodeToPNG();
                    if (pngData != null) {
                        File.WriteAllBytes(path, pngData);

                        AssetDatabase.Refresh();
                    }
                }

            }

        }


    }

    private float SquareDist(float x1, float y1, float x2, float y2) {
        float dx = x1 - x2;
        float dy = y1 - y2;
        return dx * dx + dy * dy;
    }

    private float Dist(float x1, float y1, float x2, float y2) {
        float dx = x1 - x2;
        float dy = y1 - y2;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    private void SignedDistanceField() {
        Color[] c = outputTexture.GetPixels();

        int[][] bitmap = new int[outputTexture.width][];
        for (int i = 0; i < bitmap.Length; i++) {
            bitmap[i] = new int[outputTexture.height];
        }


        for (int i = 0; i < c.Length; i++) {
            int x = i % outputTexture.width;
            int y = i / outputTexture.width;

            bitmap[x][y] = c[i].grayscale > 0 ? 1 : 0;
        }


        for (int x = 0; x < outputTexture.width; x++) {
            for (int y = 0; y < outputTexture.height; y++) {
                int xyValue = bitmap[x][y];

                int startX = (int)Mathf.Max(0, x - radius);
                int endX = (int)Mathf.Min(outputTexture.width, x + radius);

                int startY = (int)Mathf.Max(0, y - radius);
                int endY = (int)Mathf.Min(outputTexture.height, y + radius);

                float smallestDist = radius * radius;

                for (int _x = startX; _x < endX; _x++) {
                    for (int _y = startY; _y < endY; _y++) {
                        if (xyValue != bitmap[_x][_y]) {
                            float newDist = SquareDist(x, y, _x, _y);
                            if (newDist < smallestDist) {
                                smallestDist = newDist;
                            }
                        }
                    }
                }

                float signedDist = (xyValue == 1 ? 1 : -1) * Mathf.Min(Mathf.Sqrt(smallestDist), radius);
                float grayScale = 0.5f + 0.5f * (signedDist / radius);

                int i = y * outputTexture.width + x;
                c[i].r = grayScale;
                c[i].g = grayScale;
                c[i].b = grayScale;
                c[i].a = 1.0f;
            }
        }

        outputTexture.SetPixels(c);
    }





}
