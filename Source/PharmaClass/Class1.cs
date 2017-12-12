using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace Pharma
{
    public class Building_PharmaVat : Building
    {
        private enum Phase
        {
            offline,
            active
        }

        private CompGlower glowerComp;
        private CompPowerTrader powerComp;

        private bool destroyedFlag = false;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            SetPowerGlower();
        }
    }
}
