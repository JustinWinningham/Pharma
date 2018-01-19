using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using UnityEngine;
using Verse;
using RimWorld;

namespace Pharma
{
    public class Pharma_Utility
    {

        /// <summary>
        /// Check if pawn has the effect of ingestible active.
        /// TODO Fix this code, possible if ingestible gives more than one effect then it wont return false even if main drug effect not active, only additional effects.
        /// </summary>
        /// <param name="pawn">The pawn.</param>
        /// <param name="ingestible">The ingestible.</param>
        /// <returns></returns>
        public static bool PawnHasIngestibleEffect(Pawn pawn, Thing ingestible)
         {
            // check the hediffs for drug effect
            int i = 0;
            foreach (var ingestoutcome in (ingestible.def.ingestible.outcomeDoers))
            {
                i += 1;
                Log.Message("Ingest outcome found: " + ingestoutcome.ToString());
                if (pawn.health.hediffSet.HasHediff(ingestoutcome.ChangeType<IngestionOutcomeDoer_GiveHediff>().hediffDef))
                {
                    return true;
                }
             
            }
            return false;
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