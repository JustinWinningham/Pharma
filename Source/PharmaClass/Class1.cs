using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace Pharma
{
    public class SpreadingPlant : Plant
    {
        private int spreadTicks;

        public override void TickLong()
        {
            if (Destroyed)
                return;

            if (GenPlant.GrowthSeasonNow(Position, Map))
            {
                //Grow
                float prevGrowth = growthInt;
                bool wasMature = LifeStage == PlantLifeStage.Mature;
                growthInt += GrowthPerTick * GenTicks.TickLongInterval;

                if (growthInt > 1f)
                    growthInt = 1f;

                //Regenerate layers
                if ((!wasMature && LifeStage == PlantLifeStage.Mature)
                    || (int)(prevGrowth * 10f) != (int)(growthInt * 10f))
                {
                    if (CurrentlyCultivated())
                        Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
                }
                //Reproduce
                if (CanReproduceNow
                    && Rand.MTBEventOccurs(def.plant.reproduceMtbDays, GenDate.TicksPerDay, GenTicks.TickLongInterval))
                {
                    GenPlantReproduction.TryReproduceFrom(Position, def, SeedTargFindMode.Reproduce, Map);
                }
            }

            bool hasLight = HasEnoughLightToGrow;

            //Record light starvation
            if (!hasLight)
                unlitTicks += GenTicks.TickLongInterval;
            else
                unlitTicks = 0;

            //Age
            ageInt += GenTicks.TickLongInterval;

            //Dying
            if (Dying)
            {
                var map = Map;
                bool isCrop = IsCrop; // before applying damage!
                bool harvestableNow = HarvestableNow;
                bool dyingBecauseExposedToLight = DyingBecauseExposedToLight;

                int dyingDamAmount = Mathf.CeilToInt(CurrentDyingDamagePerTick * GenTicks.TickLongInterval);
                //TakeDamage(new DamageInfo(DamageDefOf.Rotting, dyingDamAmount));

                if (Destroyed)
                {
                    if (isCrop && def.plant.Harvestable && MessagesRepeatAvoider.MessageShowAllowed("MessagePlantDiedOfRot-" + def.defName, 240f))
                    {
                        string messageKey;
                        if (harvestableNow)
                            messageKey = "MessagePlantDiedOfRot_LeftUnharvested";
                        else if (dyingBecauseExposedToLight)
                            messageKey = "MessagePlantDiedOfRot_ExposedToLight";
                        else
                            messageKey = "MessagePlantDiedOfRot";

                        Messages.Message(messageKey.Translate(Label).CapitalizeFirst(),
                            new TargetInfo(Position, map),
                            MessageTypeDefOf.NegativeEvent);
                    }

                    return;
                }
            }

            if(Find.TickManager.TicksGame % spreadTicks == 0)
            {
                GenPlantReproduction.TryReproduceFrom(Position, def, SeedTargFindMode.Reproduce, Map);
            }

        }
    }
}
