﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ShapeEditor.Annotations;
using ShapeEditor.Fabrics;
using ShapeEditor.Renderers;
using ShapeEditor.Shapes;

namespace ShapeEditor.Utils
{
    public class GraphicsController : INotifyPropertyChanged
    {
        public enum Mode
        {
            None,
            DrawTriangle,
            DrawRect,
            DrawEllipse,
            DrawLine
        }

        public Mode CanvasMode
        {
            get { return _canvasMode; }
            set
            {
                _canvasMode = value;
                currentPoints.Clear();
                OnPropertyChanged();
            }
        }

        public List<IShape> ShapesList { get; set; } = new List<IShape>();
        public IRenderer Renderer { get; set; }
        public Color SelectedFillColor = Color.FromRgb(255, 255, 255);
        public Color SelectedBorderColor = Color.FromRgb(0, 0, 0);
        public double Scale = 1;
        public double BorderWidth = 1;

        public void AddShape(string name, IEnumerable<Point> points)
        {
            var newShape = ShapeFabric.CreateShape(name, points);
            var newShapeDrawable = newShape as IDrawable2DShape;

            if (newShapeDrawable != null)
            {
                newShapeDrawable.BorderWidth = BorderWidth;
                newShapeDrawable.BorderColor = SelectedBorderColor;
                newShapeDrawable.FillColor = SelectedFillColor;

                var rendererOpenGl = Renderer as RendererOpenGl;
                if (rendererOpenGl != null)
                    rendererOpenGl.OnDraw += (sender, args) => newShapeDrawable.Draw(Renderer);
            }

            ShapesList.Add(newShape);
        }

        public void Render()
        {
            foreach (var shape in ShapesList)
                (shape as IDrawable)?.Draw(Renderer);
        }

        List<Point> currentPoints = new List<Point>();
        private Mode _canvasMode;

        public void CanvasMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Random r = new Random();
            currentPoints.Add(new Point(r.NextDouble() - 0.5, r.NextDouble() - 0.5));
            switch (CanvasMode)
            {
                case Mode.DrawTriangle:
                    if (currentPoints.Count == 3)
                    {
                        AddShape("Triangle", currentPoints);
                        CanvasMode = Mode.None;
                    }
                    else
                    {
                    }
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}