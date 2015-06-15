﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using GTA;
using GTA.Native;

namespace Inferno
{
    class ArmorAndHealthSupplier : InfernoScript
    {
        bool _isActive = false;

        /// <summary>
        /// 0.1秒間隔
        /// </summary>
        protected override int TickInterval
        {
            get { return 100; }
        }

        protected override void Setup()
        {

            CreateInputKeywordAsObservable("armor")
                .Subscribe(_ =>
                {
                    _isActive = !_isActive;
                    DrawText("SupplyArmorAndHealth:" + _isActive, 3.0f);
                });

            OnAllOnCommandObservable.Subscribe(_ => _isActive = true);

            //ミッションが始まった時
            OnTickAsObservable
                .Where(_ => _isActive)
                .Select(_ => Game.MissionFlag)
                .DistinctUntilChanged()
                .Where(x => x)
                .Subscribe(_ => SupplyArmorAndHealth());

            //プレイヤが復活した時
            OnTickAsObservable
                .Where(_ => _isActive && playerPed.IsSafeExist())
                .Select(_ => playerPed.IsAlive)
                .DistinctUntilChanged()
                .Skip(1) //ONにした直後の判定結果は無視
                .Where(x => x)
                .Subscribe(_ => SupplyArmorAndHealth());
        }
        
        /// <summary>
        /// 体力とアーマー回復
        /// </summary>
        private void SupplyArmorAndHealth()
        {
            var player = playerPed;
            var maxHealth = player.MaxHealth;
            var maxArmor = Game.Player.GetPlayerMaxArmor();
            player.Health = maxHealth;
            player.Armor = maxArmor;
        }
    }
}
