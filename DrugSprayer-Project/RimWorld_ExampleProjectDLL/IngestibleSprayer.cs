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
    [StaticConstructorOnStartup]
    public abstract class Building_IngestibleSprayer : Building
    {
        public Building_IngestibleSprayer() : base()
        {

        }
        // ==================================

        /// <summary>
        /// Do something after the object is spawned into the world
        /// </summary>
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            Log.Message("[Pharma] IngestibleSprayer: Spawn setup");

            base.SpawnSetup(map, respawningAfterLoad);

            powerComp = base.GetComp<CompPowerTrader>();
            powerComp.PowerOn = true;

            Log.Message("[Pharma] IngestibleSprayer: Spawn setup end.");

        }


        /// <summary>
        /// To save and load actual values (savegame-data)
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
        }



        #region Destroy
        // ==================================

        /// <summary>
        /// Clean up when this is destroyed
        /// </summary>
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
        }

        #endregion

        /// <summary>
        /// This string will be shown when the object is selected (focus)
        /// </summary>
        /// <returns></returns>
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string baseString = base.GetInspectString();
            if (!baseString.NullOrEmpty())
            {
                stringBuilder.Append(base.GetInspectString());
                stringBuilder.AppendLine();
            }

            // return the complete string
            return stringBuilder.ToString().TrimEndNewlines();
        }


        public abstract bool IsAcceptableAmmoToSpray(ThingDef thing);


        public virtual bool TrySpray(Pawn p)
        {
            Log.Message("[Pharma] IngestibleSprayer: Begin TrySpray.");

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
            Log.Message("[Pharma] IngestibleSprayer: Could not Spray this pawn.");

            return false;
        }


        public override void Tick()
        {
            Log.Message("[Pharma] IngestibleSprayer: Begin Tick.");

            base.Tick();
            Log.Message("[Pharma] IngestibleSprayer: Tick - Getting Map.");


            Map map = base.Map;

            //Pawn[] pawns = Pharma_Utility.GetPawnsInRange(range);
            Log.Message("[Pharma] IngestibleSprayer: Tick - Beginning check spray.");

            if (this.CanDispenseNow)
            {
                foreach (Pawn p in map.mapPawns.AllPawnsSpawned)
                {
                    Log.Message("[Pharma] IngestibleSprayer: Attempting Spray.");
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
                Log.Message("[Pharma] IngestibleSprayer: Getting CanDispenseNow.");
                return this.powerComp.PowerOn && this.HasEnoughAmmoInHoppers();
            }
        }

        private List<IntVec3> AdjCellsCardinalInBounds
        {
            get
            {
                Log.Message("[Pharma] IngestibleSprayer: Getting AdjCellsCerdinalInBounds.");
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
                Log.Message("[Pharma] Getting DispensableDef.");

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
            Log.Message("[Pharma] IngestibleSprayer: Checking Has Enough Hopper Ammo.");
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
                    Log.Message("[Pharma] IngestibleSprayer: Has enough ammp.");

                    return true;
                }
            }
            Log.Message("[Pharma] IngestibleSprayer: Does not have enough ammo.");

            return false;
        }

        public virtual Building AdjacentReachableHopper(Pawn reacher)
        {
            Log.Message("[Pharma] IngestibleSprayer: Finding AdjacentReachableHopper.");

            for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
            {
                IntVec3 c = this.AdjCellsCardinalInBounds[i];
                Building edifice = c.GetEdifice(base.Map);
                if (edifice != null && edifice.def == ThingDefOf.Hopper && reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    Log.Message("[Pharma] IngestibleSprayer: Found hopper.");

                    return (Building_Storage)edifice;

                }
            }
            Log.Message("[Pharma] IngestibleSprayer: Did not find hopper.");

            return null;
        }



        public void Spray(Pawn pawn)
        {
            Log.Message("[Pharma] IngestibleSprayer: Spraying Ammo at pawn.");

            //this.TryGetComp<CompProperties_DrugSprayer>().range;
            //Props.ingestible.Ingested(pawn, 0f);
            FindAmmoInAnyHopper().Ingested(pawn, 0f);


        }

        public virtual Thing FindAmmoInAnyHopper()
        {
            Log.Message("[Pharma] IngestibleSprayer: Finding Ammo.");

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
