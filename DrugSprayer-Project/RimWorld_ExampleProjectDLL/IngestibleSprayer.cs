

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
        const float radius = 5f;

        public static void SprayerLog(string log)
        {
            Pharma_Util.PharmaLog(Pharma_Util.TESTING_SPRAYER,"[IngestibleSprayer] "+log);
        }

        public Building_IngestibleSprayer() : base()
        {

        }


        public override void Tick()
        {
            try
            {

                SprayerLog("Begin Tick.");

                base.Tick();
                SprayerLog("Tick - Getting Map.");


                Map map = base.Map;

                //Pawn[] pawns = Pharma_Util.GetPawnsInRange(range);
                SprayerLog("Tick - Beginning check spray.");

                if (this.CanDispenseNow)
                {
                    SprayerLog("Can dispense now!");
                    IntVec3 c = base.Position;
                    float dist;
                    foreach (Pawn p in map.mapPawns.AllPawnsSpawned)
                    {
                        dist = c.DistanceTo(p.Position);
                        if(dist <= radius)
                        {
                            if (p.def.race.IsFlesh)
                            {

                                SprayerLog("FOUND NEARBY PAWN TO SPRAY!");
                                if (!TrySpray(p))
                                {
                                    SprayerLog("Could not spray pawn with thing.");
                                }

                                //float dist = planet.WorldGrid.ApproxDistanceInTiles(p.Tile,)
                                //  this.Tile
                            }
                        }

                    }
                }

            }
            catch (Exception e)
            {
                SprayerLog("Exception thrown while Ticking ingestibleSprayer!");
                throw e;
            }
        }
        // ==================================

        /// <summary>
        /// Do something after the object is spawned into the world
        /// </summary>
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            SprayerLog("Spawn setup");

            base.SpawnSetup(map, respawningAfterLoad);

            powerComp = base.GetComp<CompPowerTrader>();
            powerComp.PowerOn = true;

            SprayerLog("Spawn setup end.");

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
            SprayerLog("Begin TrySpray.");

            bool administer;

            administer = false;
            Thing ammo = FindAmmoInAnyHopper();
            if(ammo != null)
            {
                if (!Pharma_Util.PawnHasIngestibleEffect(p, ammo.def))
                {
                    administer = true;
                }
                if (administer)
                {
                    Spray(p);
                    return true;
                }
                SprayerLog("Could not Spray this pawn.");

            }

            return false;
        }



        public CompPowerTrader powerComp;

        private List<IntVec3> cachedAdjCellsCardinal;

        public static int CollectDuration = 50;

        public bool CanDispenseNow
        {
            get
            {
                SprayerLog("Getting CanDispenseNow.");
                return this.powerComp.PowerOn && this.HasEnoughAmmoInHoppers();
            }
        }

        private List<IntVec3> AdjCellsCardinalInBounds
        {
            get
            {
                SprayerLog("Getting AdjCellsCerdinalInBounds.");
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
                SprayerLog("Getting DispensableDef.");

                Thing ammo = FindAmmoInAnyHopper();
                if(ammo != null)
                {

                    ThingDef spraydef = FindAmmoInAnyHopper().def;
                    if (IsAcceptableAmmoToSpray(spraydef))
                    {
                        return spraydef;
                    }
                }
                return null;

            }
        }


        public virtual bool HasEnoughAmmoInHoppers()
        {
            SprayerLog("HasEnoughAmmoInHoppers - Checking Items in adjacent cells.");
            List<IntVec3> adjcells = this.AdjCellsCardinalInBounds;
            for (int i = 0; i < adjcells.Count; i++)
            {
                IntVec3 c = adjcells[i];
                Thing itemadj = null;
                Thing hopperadj = null;
                List<Thing> thingList = c.GetThingList(base.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    SprayerLog("Checking item "+j+":"+i +": "+ thingList[j].ToString());
                    Thing currentthing = thingList[j];
                    if (IsAcceptableAmmoToSpray(currentthing.def))
                    {
                        SprayerLog("Found a valid item to spray.");

                        itemadj = currentthing;
                    }
                    if (currentthing.def.defName == "DrugHopper")
                    {
                        SprayerLog("Found a drug hopper.");

                        hopperadj = currentthing;
                    }
                }
                if (itemadj != null && hopperadj != null)
                {
                    SprayerLog("Found things in hoppers.");
                    return true;
                }
            }
            SprayerLog("[Pharma] IngestibleSprayer: Does not have enough ammo.");

            return false;
        }

        public virtual Building AdjacentReachableHopper(Pawn reacher)
        {
            SprayerLog("Finding AdjacentReachableHopper.");

            for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
            {
                IntVec3 c = this.AdjCellsCardinalInBounds[i];
                Building edifice = c.GetEdifice(base.Map);
                if (edifice != null && edifice.def.defName == "DrugHopper" && reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
                {
                    SprayerLog("Found hopper.");

                    return (Building_Storage)edifice;

                }
            }
            SprayerLog("Did not find hopper.");

            return null;
        }



        public void Spray(Pawn pawn)
        {
            if(pawn != null)
            {
                SprayerLog("Spraying Ammo at pawn: " + pawn.Name);

                //this.TryGetComp<CompProperties_DrugSprayer>().range;
                //Props.ingestible.Ingested(pawn, 0f);
                Thing ammo = FindAmmoInAnyHopper();
                if (ammo != null)
                {

                    ammo.Ingested(pawn, 0f);
                }
                else
                {
                    Building_DrugSprayer.SprayerLog("ERROR Tried to spray.. but ammo Thing was null!");


                }
                //FindAmmoInAnyHopper().inge

            }
            else
            {
                Building_DrugSprayer.SprayerLog("ERROR Pawn was null... Could not spray");
            }


        }

        public virtual Thing FindAmmoInAnyHopper()
        {
            SprayerLog("Finding Ammo.");
            int adjcellsno = this.AdjCellsCardinalInBounds.Count;

            for (int i = 0; i < adjcellsno; i++)
            {
                SprayerLog("Finding Ammo in cardinal cell "+i);
                Thing thing = null;
                Thing thing2 = null;
                List<Thing> thingList = this.AdjCellsCardinalInBounds[i].GetThingList(base.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    Thing thing3 = thingList[j];
                    if (IsAcceptableAmmoToSpray(thing3.def))
                    {
                        SprayerLog("Found acceptable thing - ammo");
                        thing = thing3;
                    }
                    if (thing3.def.defName == "DrugHopper")
                    {
                        SprayerLog("Found acceptable thing - hopper");
                        thing2 = thing3;
                    }
                }
                if (thing != null && thing2 != null)
                {

                    SprayerLog("Found thing and hopper");
                    return thing;
                }
            }
            SprayerLog("Could not find thing and hopper");
            return null;
        }


    }
}
