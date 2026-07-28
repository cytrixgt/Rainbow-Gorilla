using UnityEngine;

namespace RainbowGorilla
{
    public class RainbowGUI : MonoBehaviour
    {
        public class RainbowSettings
        {
            public bool modenabled = true;
            public bool applytoall = true;
            public float saturation = 1f;
            public float cyclespeed = 0.4f;
            public float value = 1f;
        }
        public static RainbowSettings Settings = new RainbowSettings();

        private bool showmenu = false;
        private Rect windowrect = new Rect(20, 60, 240, 200);
        private Rect togglebutton = new Rect(20, 20, 140, 30);
        private void OnGUI()
        {
            if (GUI.Button(togglebutton, showmenu ? "Hide Menu" : "Show Menu"))
            {
                showmenu = !showmenu;
            }

            if (showmenu)
            {
                windowrect = GUI.Window(1234, windowrect, DrawWindow, "Rainbow Gorilla");
            }
        }
        private void DrawWindow(int id)
        {
            Settings.modenabled = GUI.Toggle(new Rect(10, 25, 220, 20), Settings.modenabled, "Mod Enabled");
            Settings.applytoall = GUI.Toggle(new Rect(10, 50, 220, 20), Settings.applytoall, "Apply All");
            GUI.Label(new Rect(10, 80, 220, 20), $"Speed: {Settings.cyclespeed:F2}");
            Settings.cyclespeed = GUI.HorizontalSlider(new Rect(10, 100, 220, 20), Settings.cyclespeed, 0.05f, 2f);
            GUI.Label(new Rect(10, 125, 220, 20), $"Saturation: {Settings.saturation:F2}");
            Settings.saturation = GUI.HorizontalSlider(new Rect(10, 145, 220, 20), Settings.saturation, 0f, 1f);

            if (GUI.Button(new Rect(10, 170, 220, 25), "Close"))
            {
                showmenu = false;
            }
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}