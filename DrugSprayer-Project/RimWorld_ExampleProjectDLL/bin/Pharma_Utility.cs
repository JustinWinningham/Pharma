

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using UnityEngine;
using Verse;
using RimWorld;

namespace Pharma
{
    public class Pharma_Util
    {
        public static bool TESTING_SPRAYER = false;

       
        /// <summary>
        /// Check if pawn has the effect of ingestible active.
        /// TODO Fix this code, possible if ingestible gives more than one effect then it wont return false even if main drug effect not active, only additional effects.
        /// </summary>
        /// <param name="pawn">The pawn.</param>
        /// <param name="ingestible">The ingestible.</param>
        /// <returns></returns>
        public static bool PawnHasIngestibleEffect(Pawn pawn, ThingDef ingestible)
        {
            bool containsThisHediff; // bool if contains a specific hediff.
            IngestionOutcomeDoer_GiveHediff giveHediff; // each IngestionOutcomeDoer_GiveHediff for the item inspected
            HediffDef hediffToGive; // each hediff the drug gives
            try
            {

                if (ingestible != null)
                {
                    //pawn.

                    Building_DrugSprayer.SprayerLog("Checking effect of " + ingestible.ToString());
                    IngestibleProperties ingestibleProps = ingestible.ingestible;
                    if (ingestibleProps != null)
                    {
                        Building_DrugSprayer.SprayerLog("found ingestible props");
                        List<IngestionOutcomeDoer> outcomes = ingestibleProps.outcomeDoers;
                        int hediffOutcomeCount = 0;
                        int pawnMatchingHediffs = 0; // hediffs that match those given by the drug

                        foreach (var outcome in outcomes)
                        {
                            Building_DrugSprayer.SprayerLog("type: "+ outcome.GetType());
                            if(outcome.GetType() == typeof(IngestionOutcomeDoer_GiveHediff))
                            {
                                giveHediff = outcome.ChangeType<IngestionOutcomeDoer_GiveHediff>();
                                if (giveHediff != null)
                                {
                                    hediffOutcomeCount += 1;
                                    hediffToGive = giveHediff.hediffDef;
                                    Building_DrugSprayer.SprayerLog("hediffToGive: " + hediffToGive.ToString());
                                    containsThisHediff = false;
                                    foreach (Hediff h in pawn.health.hediffSet.hediffs)
                                    {
                                        if (h.def == hediffToGive)
                                        {
                                            containsThisHediff = true;
                                        }
                                    }
                                    if (containsThisHediff)
                                    {
                                        pawnMatchingHediffs += 1;
                                    }

                                }
                                else
                                {
                                    Building_DrugSprayer.SprayerLog("giveHediff is null.");

                                }
                            }
                           
                        }
                        // all effects match - the pawn has the effects of this drug.
                        if(pawnMatchingHediffs == hediffOutcomeCount)
                        {
                            Building_DrugSprayer.SprayerLog("Pawn has drug effects!");

                            return true;
                        }
                        //List<Hediff> outcomeHediffs = outcomes[0].
                    }
                    else
                    {
                        Building_DrugSprayer.SprayerLog("Checking effect - IngestibleProps is null");
                    }
                }
                else
                {
                    Building_DrugSprayer.SprayerLog("Checking effect - Ingestible is null");

                }

            }
            catch (Exception e)
            {
                Building_DrugSprayer.SprayerLog("Exception caught checking if pawn has ingestible effect: " + e.Message);
                throw (e);
            }
            Building_DrugSprayer.SprayerLog("Checking effect - No Ingest outcome found: object is null ");
            return false;
        }

        public static void PharmaLog(bool condition, string log)
        {
            if (condition)
            {
                Log.Message("[Pharma] " + log);
            }
        }


        /*
        public static void AdministerDrugEffect(Pawn pawn, Thing drug)
        {
            foreach (var outcomedoer in drug.def.ingestible.outcomeDoers)
            {
                outcomedoer.DoIngestionOutcome(pawn, drug);
            }
        }*/

    }
    
}