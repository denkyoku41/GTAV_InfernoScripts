﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using Inferno.ChaosMode;
using Inferno.Utilities;


namespace Inferno.InfernoScripts.Parupunte.Scripts
{
    class KillCitizens : ParupunteScript
    {
        public KillCitizens(ParupunteCore core) : base(core)
        {
        }

        public override string Name => "周辺市民全員即死";

        private uint coroutineId = 0;

        public override void OnStart()
        {
            coroutineId = StartCoroutine(KillCitizensCoroutine());
        }

        public override void OnFinished()
        {
            StopCoroutine(coroutineId);
            ParupunteEnd();
        }

        IEnumerable<object> KillCitizensCoroutine()
        {
            var radius = 100.0f;
            var player = core.PlayerPed;
            var peds = core.CachedPeds.Where(x => x.IsSafeExist()
                                               && !x.IsSameEntity(core.PlayerPed)
                                               && !x.IsRequiredForMission()
                                               && x.IsInRangeOf(player.Position, radius)).ToList();

            foreach (var ped in peds)
            {
                ped.Kill();
            }

            var removePedList = new HashSet<Ped>();
            var pedsCount = peds.Count();

            while(pedsCount > 0)//一気に数十個も同時に爆発を起こせないので時間差で行う
            {
                foreach (var ped in peds.Select((entity, index) => new { entity, index }))
                {
                    if (ped.index < pedsCount && ped.index < 10)
                    {
                        removePedList.Add(ped.entity);
                        GTA.World.AddExplosion(ped.entity.Position, GTA.ExplosionType.Rocket, 8.0f, 2.5f);
                    }
                    else
                    {
                        break;
                    }
                }

                peds.RemoveAll(removePedList.Contains);
                removePedList.Clear();
                pedsCount = peds.Count();

                yield return null;
            }

            ParupunteEnd();
        }
    }
}
