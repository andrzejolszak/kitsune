﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Kitsune
{
    public class VariableView : IBlockView
    {
        public event ViewChangedEvent Changed;
        IVarBlock _model;
        Bitmap _cached;

        Nine abc;
        Graphics textMetrics;
        Font textFont;

        public VariableView(IVarBlock model, Nine abc, Graphics textMetrics, Font textFont)
        {
            this._model = model;
            this.Changed += delegate(object source) { };

            // todo: So much in common with TextView; consider abstracting into common implementation
            this.abc = abc;
            this.textMetrics = textMetrics;
            this.textFont = textFont;
        }

        public Bitmap Assemble()
        {
            if (_cached == null)
                Reassemble();
            return _cached;
        }

        public void Reassemble()
        {
            Size sz = textMetrics.MeasureString(_model.Name, textFont).ToSize();

            int w = Math.Max(sz.Width, abc.MinWidth) + 2;
            int h = Math.Max(sz.Height, abc.MinHeight) + 2;
            int middleWidth = w - (abc.NW.Width + abc.NE.Width);
            int middleHeight = h - (abc.NW.Height + abc.SW.Height);
            if (_cached != null)
                _cached.Dispose();
            _cached = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(_cached))
            {
                g.Clear(Color.Transparent);

                g.HighQuality();

                abc.RenderToFit(g, middleWidth, middleHeight);
                g.DrawString(_model.Name, textFont, Brushes.White, 2, 2);
            }
        }

        public IBlock Model { get { return _model; }
        }

        public IBlockView Parent { get; set; }
        public System.Drawing.Point RelativePos { get; set; }

        public IEnumerable<DropRegion> DropRegions(Point origin)
        {
            return new DropRegion[] { };
        }
        public IEnumerable<DropRegion> ChildDropRegions(Point origin)
        {
            return new DropRegion[] { };
        }

        public bool HasPoint(Point p, Point origin)
        {
            return Bounds(origin).Contains(p);
        }

        public IBlockView ChildHasPoint(Point p, Point origin)
        {
            return this;
        }

        Rectangle Bounds(Point p)
        {
            return new Rectangle(p, _cached.Size);
        }
    }
}
