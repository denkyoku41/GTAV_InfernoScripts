﻿namespace Inferno.ChaosMode
{
    internal interface IWeaponProvider
    {
        /// <summary>
        /// 遠距離攻撃系の武器を取得する
        /// </summary>
        Weapon GetRandomWeaponExcludeClosedWeapon();

        /// <summary>
        /// ドライブバイ用の武器を取得する
        /// </summary>
        Weapon GetRandomDriveByWeapon();
    }
}
