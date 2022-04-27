using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System;
using FrooxEngine.UIX;
using CodeX;
using System.Collections.Generic;

namespace ReverseAudioClip
{
    public class ReverseAudioClip : NeosMod
    {
        public override string Name => "ReverseAudioClip";
        public override string Author => "badhaloninja";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/badhaloninja/ReverseAudioClip";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("me.badhaloninja.ReverseAudioClip");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(StaticAudioClip), "BuildInspectorUI")]
        class StaticAudioClip_BuildInspectorUI_Patch
        {
            public static void Postfix(StaticAudioClip __instance, UIBuilder ui)
            {
                var buttoin = ui.Button("Reverse");
                buttoin.Slot.ActiveSelf_Field.OverrideForUser(buttoin.LocalUser, true).Default.Value = false;
                
                buttoin.LocalPressed += (btn, evnt) =>
                {
                    Process(__instance, delegate (AudioX a)
                    {
                        Dictionary<int, List<float>> samples = new Dictionary<int, List<float>>();

                        a.ForEachRawSample((index, channel, amplitude) =>
                        {
                            if (!samples.ContainsKey(channel))
                            {
                                samples.Add(channel, new List<float>());
                            }
                            samples.GetValueSafe(channel).Add(amplitude);
                        });

                        //Msg(samples.Count, samples[0].Count);

                        samples.Values.Do((channelValues) =>
                        {
                            channelValues.Reverse();
                        });
                        //Msg("Reversed");
                        a.ProcessRawSamples((i, ch, amp) => samples[ch][i]);
                    }, btn);
                };
            }
            [HarmonyReversePatch]
            [HarmonyPatch(typeof(StaticAudioClip), "Process", new Type[] { typeof(Action<AudioX>), typeof(IButton)})]
            public static void Process(StaticAudioClip instance, Action<AudioX> processFunc, IButton button)
            {
                // its a stub so it has no initial content
                throw new NotImplementedException("It's a stub");
            }
        }
    }
}