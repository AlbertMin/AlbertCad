﻿using Albert.Geometry.Primitives;
using Albert.DrawingKernel.Controls;
using Albert.DrawingKernel.Geometries.Primitives;
using Albert.DrawingKernel.Geometries.Temporary;
using Albert.DrawingKernel.PenAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Albert.DrawingKernel.Events;

namespace Albert.DrawingKernel.Assistant
{
    public class SubWallTip : BaseTip<WallAction>
    {

        private WallAction action = null;
        private SublineGeometry sublineGeometry = null;

        /// <summary>
        /// 当前的构造函数，初始化当前的绘制
        /// </summary>
        /// <param name="dc"></param>
        public SubWallTip(DrawingControl dc, TextTip tip)
            : base(dc, tip)
        {
            sublineGeometry = new SublineGeometry();
            sublineGeometry.ShowSize = false;
            Tip.TipChanged += tip_TipChanged;
            Tip.TipCompleted += tip_TipCompleted;
        }

        /// <summary>
        /// 提示完成的事件
        /// </summary>
        /// <param name="value"></param>
        void tip_TipCompleted(double value)
        {
            this.action.SetResult(value);
        }

        /// <summary>
        /// 提示改变事件
        /// </summary>
        /// <param name="value"></param>
        void tip_TipChanged(double value)
        {
            //停止当前的拖拽
            this.action.Pause();
        }

        /// <summary>
        /// 开始关心一个动作
        /// </summary>
        /// <param name="a"></param>
        public override void Attention(WallAction a)
        {
            action = a;
            action.DrawStartEvent += A_DrawStartEvent;
            action.DrawEraseEvent += A_DrawEraseEvent;
            action.DrawTermimationEvent += A_DrawTermimationEvent;
            action.DrawCompleteEvent += A_DrawCompleteEvent;
        }

        private void A_DrawCompleteEvent(ActionEventArgs gs)
        {
            this.drawingControl.RemoveTemporaryVisual(sublineGeometry);
            this.Tip.SetHide();
        }

        private void A_DrawTermimationEvent(ActionEventArgs gs)
        {
            this.drawingControl.RemoveTemporaryVisual(sublineGeometry);
            this.Tip.SetHide();
        }

        private void A_DrawEraseEvent(ActionEventArgs gs)
        {

            WallGeometry wall = this.action.Geometry as WallGeometry;
            sublineGeometry.Start = wall.Start;
            sublineGeometry.End = wall.End;
            sublineGeometry.Update();
            this.Tip.SetText(sublineGeometry.TextValue, sublineGeometry.TextPosition, sublineGeometry.TextAngle);
        }

        /// <summary>
        /// 开始绘制
        /// </summary>
        /// <param name="gs"></param>
        private void A_DrawStartEvent(ActionEventArgs gs)
        {
            if (!this.drawingControl.IsContains(sublineGeometry))
            {
                this.drawingControl.AddTemporaryVisual(sublineGeometry);
            }

        }

        /// <summary>
        /// 取消一个关心动作
        /// </summary>
        public override void unAttention()
        {
            action.DrawStartEvent -= A_DrawStartEvent;
            action.DrawEraseEvent -= A_DrawEraseEvent;
            action.DrawTermimationEvent -= A_DrawTermimationEvent;
            action.DrawCompleteEvent -= A_DrawCompleteEvent;
            Tip.TipChanged -= tip_TipChanged;
            Tip.TipCompleted -= tip_TipCompleted;
        }
    }
}
