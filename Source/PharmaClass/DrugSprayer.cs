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
    public class Building_DrugSprayer : Building_IngestibleSprayer
    {
        public Building_DrugSprayer()
        {
            this.TryGetComp<CompDrugSprayer>().SetRange(10f);
        }

        public override bool IsAcceptableAmmoToSpray(ThingDef thing)
        {
            if(thing.IsDrug)
            {
                return true;
            }
            return false;

        }
    }

    public class CompDrugSprayer : ThingComp
    {
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

    public class CompProperties_DrugSprayer : CompProperties_IngestibleSprayer
    {
        public CompProperties_DrugSprayer()
        {
            compClass = typeof(CompDrugSprayer);
        }
       
    }


    public class CompProperties_IngestibleSprayer : CompProperties
    {
        public float range;
    }
}