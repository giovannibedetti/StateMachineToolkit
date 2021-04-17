using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.gb.statemachine_toolkit
{
    public class Screenshot : MonoBehaviour
    {
        public int w = 1920;
        public int h = 1080;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StartCoroutine(WaitForScreenshot());
            }
        }

        IEnumerator WaitForScreenshot()
        {
            yield return new WaitForEndOfFrame();
            var d = System.DateTime.Now;
            //ScreenCapture.CaptureScreenshot(d.Year+"_"+d.Month+"_"+d.Day+"_"+d.Hour+"_"+d.Minute+"_"+d.Second+"_"+d.Millisecond, 4);

            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            var bytes = tex.EncodeToPNG();
            Destroy(tex);

            File.WriteAllBytes(d.Year + "_" + d.Month + "_" + d.Day + "_" + d.Hour + "_" + d.Minute + "_" + d.Second + "_" + d.Millisecond + ".png", bytes);
        }
    }
}
