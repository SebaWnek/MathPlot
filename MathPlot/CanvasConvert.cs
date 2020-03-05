using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace MathPlot
{
    /// <summary>
    /// Class for converting world coordinates into screen coordinates of canvas, taken and modified from my another project
    /// </summary>
    public class CanvasConvert
    {
        private double canvasHeigth;
        private double canvasWidth;
        private double fieldWidth;
        private double fieldHeight;
        private double horizontalConversionRate;
        private double verticalConversionRate;
        private double[] middlePoint = new double[2];

        public double FieldHeigth { get => fieldHeight; }
        public double FieldWidth { get => fieldWidth; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="heigth">Canvas height</param>
        /// <param name="width">Canvas width</param>
        /// <param name="horizontalScale">Horizontal axis range</param>
        /// <param name="verticalScale">Vertical axis range</param>
        /// <param name="middle">Fractions pointing to middle of plot</param>
        public CanvasConvert(double heigth, double width, double horizontalScale, double verticalScale, double[] middle)
        {
            canvasHeigth = heigth;
            canvasWidth = width;
            fieldWidth = horizontalScale;
            fieldHeight = verticalScale;
            horizontalConversionRate = canvasWidth / FieldWidth;
            verticalConversionRate = canvasHeigth / FieldHeigth;
            middlePoint[0] = middle[0] * canvasWidth;
            middlePoint[1] = middle[1] * canvasHeigth;
        }
        public System.Windows.Point ToWindowsPoint(double x, double y)
        {
            double resultX;
            double resultY;
            resultX = x * horizontalConversionRate + middlePoint[0];
            resultY = canvasHeigth - y * verticalConversionRate - middlePoint[1];
            return new System.Windows.Point(resultX, resultY);
        }
        public int[] ToInt(double x, double y)
        {
            int resultX;
            int resultY;
            resultX = (int)(x * horizontalConversionRate);
            resultY = (int)(canvasHeigth - y * verticalConversionRate);
            return new int[] { resultX, resultY };
        }
        public System.Drawing.Point ToDrawingPoint(double x, double y)
        {
            int resultX;
            int resultY;
            resultX = (int)(x * horizontalConversionRate);
            resultY = (int)(canvasHeigth - y * verticalConversionRate);
            return new System.Drawing.Point(resultX, resultY);
        }
    }
}
