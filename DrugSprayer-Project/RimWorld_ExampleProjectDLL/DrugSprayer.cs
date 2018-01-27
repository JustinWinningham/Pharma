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

    public class CompTest : ThingComp
    {
        
        protected CompProperties_Test PropsTest
        {
            get
            {
                return (CompProperties_Test)base.props;
            }
        }

        public override void PostExposeData()
        {
            // do for each comp property
            //Scribe_Values.Look<int>(ref this.plantHarmAge, "plantHarmAge", 0, false);
            //Scribe_Values.Look<int>(ref this.ticksToPlantHarm, "ticksToPlantHarm", 0, false);
        }

        public override void CompTick()
        {

            Building_IngestibleSprayer.SprayerLog("[Pharma] test comp tick.");
            TestCompFunction();
        }

        private void TestCompFunction()
        {

            Building_IngestibleSprayer.SprayerLog("[Pharma] test comp function.");
        }
    }

    public class CompProperties_Test : CompProperties
    {
        public CompProperties_Test()
        {
            Building_IngestibleSprayer.SprayerLog("[Pharma] test comp properties.");
            base.compClass = typeof(CompTest);
        }
    }
    */
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
        public Building_DrugSprayer() : base()
        {
            
            SprayerLog("Created drug sprayer.");
             // testing remove tk
            //this.TryGetComp<CompDrugSprayer>().SetRange(10f);
        }

        public override bool IsAcceptableAmmoToSpray(ThingDef thing)
        {
            SprayerLog("Checking is acceptable ammo.");
            if (thing.IsDrug)
            {
                SprayerLog("acceptable.");
                return true;
            }
            SprayerLog("not acceptable.");

            return false;

        }
        public override void Tick()
        {
            SprayerLog("Drug sprayer tick.");
            base.Tick();
         }

    }
    /*

    public class CompDrugSprayer : ThingComp
    {
        public override void CompTick()
        {
            Building_IngestibleSprayer.SprayerLog("Drug sprayer comp tick.");

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
            Building_IngestibleSprayer.SprayerLog("Ingestible sprayer comp tick.");

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
            Building_IngestibleSprayer.SprayerLog("[Pharma] Drug sprayer comp created.");

            compClass = typeof(CompDrugSprayer);

            Building_IngestibleSprayer.SprayerLog("[Pharma] Drug sprayer comp class set.");

        }

    }


    public class CompProperties_IngestibleSprayer : CompProperties
    {
        public CompProperties_IngestibleSprayer()
        {

            Building_IngestibleSprayer.SprayerLog("Ingest sprayer comp created.");

            compClass = typeof(CompIngestibleSprayer);

            Building_IngestibleSprayer.SprayerLog("Ingest sprayer comp class set.");

        }
        public float range;
    }
    */
}