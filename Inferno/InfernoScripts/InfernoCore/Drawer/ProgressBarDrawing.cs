﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace Inferno
{
    /// <summary>
    /// プログレスバーの表示管理
    /// </summary>
    public class ProgressBarDrawing : InfernoScript
    {

        private UIContainer _mContainer = null;

        public static ProgressBarDrawing Instance { get; private set; }

        private List<ProgressBarData> progressBarDataList = new List<ProgressBarData>();

        private object lockObject = new object();

        protected override void Setup()
        {
            Instance = this;
            //描画エリア
            _mContainer = new UIContainer(new Point(0, 0), new Size(500, 20));

            //バー表示が設定されていたら描画
            this.OnDrawingTickAsObservable
                .Where(_ =>progressBarDataList.Any()) //Readだし排他ロックいらないかなという判断
                .Subscribe(_ =>
                {
                    _mContainer.Items.Clear();
                    var datas = new ProgressBarData[0];

                    //ここは排他ロック必要
                    lock (lockObject)
                    {
                        //完了しているものは除外する
                        progressBarDataList.RemoveAll(x => x.ProgressBarStatus.IsCompleted);
                        datas = progressBarDataList.ToArray();
                    }

                    foreach (var progressBarData in datas)
                    {
                        DrawProgressBar(progressBarData);
                    }
                });
        }

        /// <summary>
        /// ProgressBarを描画登録
        /// </summary>
        public void RegisterProgressBar(ProgressBarData data)
        {
            lock (lockObject)
            {
                progressBarDataList.Add(data);
            }
        }

        /// <summary>
        /// プログレスバーの描画
        /// </summary>
        private void DrawProgressBar(ProgressBarData data)
        {
            var pos = data.Position;
            var width = data.Width;
            var height = data.Height;
            const int margin = 10;
            var barSize = width*data.ProgressBarStatus.Rate;
            _mContainer.Items.Add(new UIRectangle(new Point(pos.X, pos.Y - margin/2), new Size(width + margin, height + margin), data.BackgorondColor));
            _mContainer.Items.Add(new UIRectangle(new Point(pos.X + margin/2, pos.Y), new Size((int)barSize, height), data.MainColor));

        }

        /// <summary>
        /// 描画に使うデータ詰めたやつ
        /// </summary>
        public class ProgressBarData
        {
            public Point Position { get; }
            public Color MainColor { get; }
            public Color BackgorondColor { get; }
            public int Width { get; }
            public int Height { get; }
            public IProgressBar ProgressBarStatus { get; }

            public ProgressBarData(IProgressBar barStatus, Point position, Color mainColor, Color backGroundColor,
                int width, int height)
            {
                Position = position;
                MainColor = mainColor;
                BackgorondColor = backGroundColor;
                ProgressBarStatus = barStatus;
                Width = width;
                Height = height;
            }
        }
    }
}
