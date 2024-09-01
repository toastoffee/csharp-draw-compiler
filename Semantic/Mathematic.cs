using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.Semantic
{
    public class Matrix_2x2
    {
        private double val_11;
        private double val_12;
        private double val_21;
        private double val_22;

        public double Val_11 { get => val_11; set => val_11 = value; }
        public double Val_12 { get => val_12; set => val_12 = value; }
        public double Val_21 { get => val_21; set => val_21 = value; }
        public double Val_22 { get => val_22; set => val_22 = value; }

        public Matrix_2x2(double v11,double v12,double v21,double v22)
        {
            Val_11 = v11;
            Val_12 = v12;
            Val_21 = v21;
            Val_22 = v22;
        }

        public static Matrix_2x2 CreateScaleOffsetMatrix(double scaleX,double scaleY)
        {
            return new Matrix_2x2(scaleX,0f,0f,scaleY);
        }

        public static Matrix_2x2 CreateRotationOffsetMatrix(double angleRadian)
        {
            return new Matrix_2x2(Math.Cos(angleRadian),
                                  Math.Sin(angleRadian),
                                  -Math.Sin(angleRadian),
                                  Math.Cos(angleRadian));
        }

        public static Matrix_2x1 operator *(Matrix_2x2 left, Matrix_2x1 right)
        {
            return new Matrix_2x1(left.Val_11 * right.Val_11 + left.Val_12 * right.Val_21,
                                  left.Val_21 * right.Val_11 + left.Val_22 * right.Val_21
                );
        }
    }

    public class Matrix_2x1
    {
        private double val_11;
        private double val_21;

        public double Val_11 { get => val_11; set => val_11 = value; }
        public double Val_21 { get => val_21; set => val_21 = value; }

        public Matrix_2x1(double v11,double v21)
        {
            Val_11 = v11;
            val_21 = v21;
        }

        public static Matrix_2x1 operator +(Matrix_2x1 left, Matrix_2x1 right)
        {
            return new Matrix_2x1(left.Val_11 + right.Val_11, left.Val_21 + right.Val_21);
        }

    }
}
