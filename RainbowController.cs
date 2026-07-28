using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace RainbowGorilla
{
    [BepInPlugin("Rainbow.Gorilla.Thingy", "Rainbow Gorilla", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private float hue = 0f;

        private class OriginalColors
        {
            public Color mainSkin;
            public Color[]? materialschanged;
        }
        private Dictionary<VRRig, OriginalColors> originalColorCache = new Dictionary<VRRig, OriginalColors>();

        private bool wasmodenabled = true;
        private bool wasapplied = true;

        private void Awake()
        {
            gameObject.AddComponent<RainbowGUI>();
        }

        private void Update()
        {
            var settings = RainbowGUI.Settings;

            VRRig[] allRigs = Object.FindObjectsByType<VRRig>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (VRRig rig in allRigs)
            {
                if (rig == null) continue;
                CacheOriginalColorIfNeeded(rig);
            }

            bool justdisabled = wasmodenabled && !settings.modenabled;
            bool just_toself = wasapplied && !settings.applytoall;

            if (justdisabled)
            {
                foreach (VRRig rig in allRigs)
                    RestoreOriginalColor(rig);
            }
            else if (just_toself)
            {
                foreach (VRRig rig in allRigs)
                {
                    if (!IsLocalRig(rig))
                        RestoreOriginalColor(rig);
                }
            }

            wasmodenabled = settings.modenabled;
            wasapplied = settings.applytoall;

            if (!settings.modenabled) return;

            hue += Time.deltaTime * settings.cyclespeed;
            if (hue > 1f)
                hue -= 1f;

            Color rainbow = Color.HSVToRGB(hue, settings.saturation, settings.value);

            foreach (VRRig rig in allRigs)
            {
                if (rig == null) continue;
                if (!settings.applytoall && !IsLocalRig(rig))
                    continue;

                ApplyRainbow(rig, rainbow);
            }
        }

        private void CacheOriginalColorIfNeeded(VRRig rig)
        {
            if (originalColorCache.ContainsKey(rig)) return;

            var cached = new OriginalColors();
            if (rig.mainSkin != null && rig.mainSkin.material != null)
                cached.mainSkin = rig.mainSkin.material.color;

            if (rig.materialsToChangeTo != null)
            {
                cached.materialschanged = new Color[rig.materialsToChangeTo.Length];
                for (int i = 0; i < rig.materialsToChangeTo.Length; i++)
                {
                    if (rig.materialsToChangeTo[i] != null)
                        cached.materialschanged[i] = rig.materialsToChangeTo[i].color;
                }
            }

            originalColorCache[rig] = cached;
        }

        private void ApplyRainbow(VRRig rig, Color rainbow)
        {
            if (rig.mainSkin != null && rig.mainSkin.material != null)
                rig.mainSkin.material.color = rainbow;

            if (rig.materialsToChangeTo != null)
            {
                for (int i = 0; i < rig.materialsToChangeTo.Length; i++)
                {
                    if (rig.materialsToChangeTo[i] != null)
                        rig.materialsToChangeTo[i].color = rainbow;
                }
            }
        }

        private void RestoreOriginalColor(VRRig rig)
        {
            if (rig == null || !originalColorCache.TryGetValue(rig, out var cached))
                return;

            if (rig.mainSkin != null && rig.mainSkin.material != null)
                rig.mainSkin.material.color = cached.mainSkin;

            if (rig.materialsToChangeTo != null && cached.materialschanged != null)
            {
                for (int i = 0; i < rig.materialsToChangeTo.Length && i < cached.materialschanged.Length; i++)
                {
                    if (rig.materialsToChangeTo[i] != null)
                        rig.materialsToChangeTo[i].color = cached.materialschanged[i];
                }
            }
        }

        private bool IsLocalRig(VRRig rig)
        {
            return rig == GorillaTagger.Instance?.offlineVRRig;
        }
    }
}