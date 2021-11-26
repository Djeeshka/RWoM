﻿using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace TorannMagic
{
    [StaticConstructorOnStartup]
    public class Hediff_ApothecaryHerbs : HediffWithCompsExtra
    {
        private bool initialized = false;
        private bool removeNow = false;

        private int eventFrequency = 60;

        private int herbPwr = 0;  //increased maximum hediff value
        private int herbVer = 0;  //increased amount of herbs found during harvesting
        private int herbEff = 0;  //reduces expiration rate

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref herbPwr, "herbPwr", 0);
            Scribe_Values.Look<int>(ref herbVer, "herbVer", 0);
            Scribe_Values.Look<int>(ref herbEff, "herbEff", 0);
        }

        public override string GizmoLabel => "TM_ApothecaryHerbsGizmoLabel".Translate();
        public override float MaxSeverity => def.maxSeverity + (10 * herbPwr);
        public override bool ShouldRemove => this.removeNow;

        private void Initialize()
        {
            bool spawned = pawn.Spawned;
            CompAbilityUserMight comp = pawn.GetComp<CompAbilityUserMight>();
            if (spawned && comp != null)
            {
                herbPwr = comp.MightData.MightPowerSkill_Herbalist.FirstOrDefault((MightPowerSkill x) => x.label == "TM_Herbalist_pwr").level;
                herbVer = comp.MightData.MightPowerSkill_Herbalist.FirstOrDefault((MightPowerSkill x) => x.label == "TM_Herbalist_ver").level;
                herbEff = comp.MightData.MightPowerSkill_Herbalist.FirstOrDefault((MightPowerSkill x) => x.label == "TM_Herbalist_eff").level;
            }
            else
            {
                this.removeNow = true;
            }
        }

        public override void PostTick()
        {
            base.PostTick();
            if(!initialized)
            {
                this.initialized = true;
                Initialize();
            }
            if(Find.TickManager.TicksGame % 150 == 0)
            {
                this.Severity -= .025f * (1f - (.1f * herbEff));
                if(Find.TickManager.TicksGame % 3000 == 0)
                {
                    Initialize();
                }
            }
        }        
    }
}