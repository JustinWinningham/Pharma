using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;


namespace Pharma
{
    public abstract class Building_IngestibleSprayer : Building
    {
        public abstract bool IsAcceptableAmmoToSpray(ThingDef thing);


        public virtual bool TrySpray(Pawn p)
        {
            bool administer;

            administer = false;
            if (!Pharma_Utility.PawnHasIngestibleEffect(p, FindAmmoInAnyHopper()))
            {
                administer = true;
            }
            if (administer)
            {
                Spray(p);
                return true;
            }
            return false;
        }


        public override void Tick()
        {
            base.Tick();

            Map map = base.Map;

            //Pawn[] pawns = Pharma_Utility.GetPawnsInRange(range);
            if (this.CanDispenseNow)
            {
                foreach (Pawn p in map.mapPawns.AllPawnsSpawned)
                {
                    if (!TrySpray(p))
                    {
                        Log.Message("Could not spray pawn with thing.");
                    }

                }
            }

        }

        public CompPowerTrader powerComp;

        private List<IntVec3> cachedAdjCellsCardinal;

        public static int CollectDuration = 50;

        public bool CanDispenseNow
        {
            get
            {
                return this.powerComp.PowerOn && this.HasEnoughAmmoInHoppers();
            }
        }

        private List<IntVec3> AdjCellsCardinalInBounds
        {
            get
            {
                if (this.cachedAdjCellsCardinal == null)
                {
                    this.cachedAdjCellsCardinal = (from c in GenAdj.CellsAdjacentCardinal(this)
                                                   where c.InBounds(base.Map)
                                                   select c).ToList();
                }
                return this.cachedAdjCellsCardinal;
            }
        }


        public virtual ThingDef DispensableDef
        {
            get
            {
                ThingDef spraydef = FindAmmoInAnyHopper().def;
                if (IsAcceptableAmmoToSpray(spraydef))
                {
                    return spraydef;
                }
                return null;
            }
        }


        public virtual bool HasEnoughAmmoInHoppers()
        {
            float num = 0f;
            for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
            {
                IntVec3 c = this.AdjCellsCardinalInBounds[i];
                Thing thing = null;
                Thing thing2 = null;
                List<Thing> thingList = c.GetThingList(base.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing thing3 = thingList[j];
                    if (IsAcceptableAmmoToSpray(thing3.def))
                    {
                        thing = thing3;
                    }
                    if (thing3.def == ThingDefOf.Hopper)
                    {
                        thing2 = thing3;
                    }
                }
                if (thing != null && thing2 != null)
                {
                    num += (float)thing.stackCount * thing.def.ingestible.nutrition;
                }
                if (num >= base.def.building.nutritionCostPerDispense)
                {
                    return true;
                }
            }
            return false;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
        }

        public virtual Building AdjacentReachableHopper(Pawn reacher)
        {
            for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
            {
                IntVec3 c = this.AdjCellsCardinalInBounds[i];
                Building edifice = c.GetEdifice(base.Map);
                if (edifice != null && edifice.def == ThingDefOf.Hopper && reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    return (Building_Storage)edifice;
                }
            }
            return null;
        }



        public void Spray(Pawn pawn)
        {
            //this.TryGetComp<CompProperties_DrugSprayer>().range;
            //Props.ingestible.Ingested(pawn, 0f);
            FindAmmoInAnyHopper().Ingested(pawn, 0f);


        }

        public virtual Thing FindAmmoInAnyHopper()
        {
            for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
            {
                Thing thing = null;
                Thing thing2 = null;
                List<Thing> thingList = this.AdjCellsCardinalInBounds[i].GetThingList(base.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing thing3 = thingList[j];
                    if (IsAcceptableAmmoToSpray(thing3.def))
                    {
                        thing = thing3;
                    }
                    if (thing3.def == ThingDefOf.Hopper)
                    {
                        thing2 = thing3;
                    }
                }
                if (thing != null && thing2 != null)
                {
                    return thing;
                }
            }
            return null;
        }


    }
}
