using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using RimWorld;

namespace Pharma
{
    public class ThingDef_NumbicaineBullet : ThingDef
    {
        public float AddHediffChance = 0.05f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = HediffDefOf.Plague;
    }
}