using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse.AI;
using Verse;
using RimWorld;

namespace Pharma
{
    /*
    public static class Pharma_ThingDefOf
    {
        public static ThingDef DrugSprayer; 
    }*/
    /// <summary>
    /// Drug sprayer building
    /// </summary>
    /// <seealso cref="Verse.Building" />
    [StaticConstructorOnStartup]
    public class Building_DrugSprayer : Building_IngestibleSprayer
    {
        public Building_DrugSprayer()
        {

            Log.Message("[Pharma] Created drug sprayer.");
             // testing remove tk
            //this.TryGetComp<CompDrugSprayer>().SetRange(10f);
        }

        public override bool IsAcceptableAmmoToSpray(ThingDef thing)
        {
            Log.Message("[Pharma] Checking is acceptable ammo.");
            if (thing.IsDrug)
            {
                return true;
            }
            return false;

        }
        public override void Tick()
        {
            Log.Message("[Pharma] Drug sprayer tick.");
            base.Tick();
         }

    }

    public class CompDrugSprayer : ThingComp
    {
        public override void CompTick()
        {
            Log.Message("[Pharma] Drug sprayer comp tick.");

            if (base.parent.Spawned)
            {
            }
        }
        public void SetRange(float newrange)
        {
            Props.range = newrange;
        }
        private CompProperties_DrugSprayer Props
        {
            get
            {
                return (CompProperties_DrugSprayer)base.props;
            }
        }

    }



    public class CompIngestibleSprayer : ThingComp
    {
        public override void CompTick()
        {
            Log.Message("[Pharma] Ingestible sprayer comp tick.");

            if (base.parent.Spawned)
            {
            }
        }
        public void SetRange(float newrange)
        {
            Props.range = newrange;
        }
        private CompProperties_IngestibleSprayer Props
        {
            get
            {
                return (CompProperties_IngestibleSprayer)base.props;
            }
        }

    }

    public class CompProperties_DrugSprayer : CompProperties_IngestibleSprayer
    {
        public CompProperties_DrugSprayer()
        {
            Log.Message("[Pharma] Drug sprayer comp created.");

            compClass = typeof(CompDrugSprayer);

            Log.Message("[Pharma] Drug sprayer comp class set.");

        }

    }


    public class CompProperties_IngestibleSprayer : CompProperties
    {
        public CompProperties_IngestibleSprayer()
        {

            Log.Message("[Pharma] Ingest sprayer comp created.");

            compClass = typeof(CompIngestibleSprayer);

            Log.Message("[Pharma] Ingest sprayer comp class set.");

        }
        public float range;
    }
}