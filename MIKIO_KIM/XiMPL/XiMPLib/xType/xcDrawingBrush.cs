using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

using XiMPLib.xDocument;

namespace XiMPLib.xType {
    class xcDrawingBrush {
        public Brush Brush {
            get;
            set;
        }
 
        public xcDrawingBrush(xcJObject jBrush) {
            String brushValue = jBrush.getString("brush");
            if (string.IsNullOrWhiteSpace(brushValue)){
                Brush = new SolidBrush(Color.Brown);
                return;
            }

            String[] token = brushValue.Split(',');
            switch(token[0]){
                case "solid":
                    Brush = new SolidBrush(new xcDrawingColor(token[1]).Color);
                    break;
                case "hatch":
                    HatchStyle style = getHatchStyle(token[1]);
                    Color foreColor = new xcDrawingColor(token[2]).Color;
                    Color backColor = new xcDrawingColor(token[3]).Color;
                    Brush = new HatchBrush(style, foreColor, backColor);
                    break;
                case "texture":
                    Brush = new TextureBrush(new xcImage(token[1]).Image);
                    break;
                case "gradient":
                case "linear_gradient":
                case "lineargradient":
                    Brush = new LinearGradientBrush(
                        new Point(new xcLength(token[1]).PageLength, new xcLength(token[2]).PageLength), 
                        new Point(new xcLength(token[3]).PageLength, new xcLength(token[4]).PageLength), 
                        new xcDrawingColor(token[5]).Color, 
                        new xcDrawingColor(token[6]).Color);
                    break;
                case "path_gradient":
                case "pathgradient":
                    break;
            }
        }

        public HatchStyle getHatchStyle(String style) {
            switch (style.ToLower()) {
                case "backwarddiagonal":
                case "backward_diagonal":
                    return HatchStyle.BackwardDiagonal;
                case "cross":
                    return HatchStyle.Cross;
                case "darkdownwarddiagonal":
                case "dark_downward_diagonal":
                    return HatchStyle.DarkDownwardDiagonal;
                case "darkhorizontal":
                case "dark_horizontal":
                    return HatchStyle.DarkHorizontal;
                case "darkupwarddiagonal":
                case "dark_upward_diagonal":
                    return HatchStyle.DarkUpwardDiagonal;
                case "darkvertical":
                case "dark_vertical":
                    return HatchStyle.DarkVertical;
                case "dasheddownwarddiagonal":
                case "dashed_downward_diagonal":
                    return HatchStyle.DashedDownwardDiagonal;
                case "dashedhorizontal":
                case "dashed_horizontal":
                    return HatchStyle.DashedHorizontal;
                case "dashedupwarddiagonal":
                case "dashed_upward_diagonal":
                    return HatchStyle.DashedUpwardDiagonal;
                case "dashedvertical":
                case "dashed_vertical":
                    return HatchStyle.DashedVertical;
                case "diagonalbrick":
                case "diagonal_brick":
                    return HatchStyle.DiagonalBrick;
                case "diagonalcross":
                case "diagonal_cross":
                    return HatchStyle.DiagonalCross;
                case "divot":
                    return HatchStyle.Divot;
                case "dotteddiamond":
                case "dotted_diamond":
                    return HatchStyle.DottedDiamond;
                case "dottedgrid":
                case "dotted_grid":
                    return HatchStyle.DottedGrid;
                case "forwarddiagonal":
                case "forward_diagonal":
                    return HatchStyle.ForwardDiagonal;
                case "horizontal":
                    return HatchStyle.Horizontal;
                case "horizontalbrick":
                case "horizontal_brick":
                    return HatchStyle.HorizontalBrick;
                case "largecheckerboard":
                case "large_checker_board":
                    return HatchStyle.LargeCheckerBoard;
                case "largeconfetti":
                case "large_confetti":
                    return HatchStyle.LargeConfetti;
                case "largegrid":
                case "large_grid":
                    return HatchStyle.LargeGrid;
                case "lightdownwarddiagonal":
                case "light_downward_diagonal":
                    return HatchStyle.LightDownwardDiagonal;
                case "lighthorizontal":
                case "light_horizontal":
                    return HatchStyle.LightHorizontal;
                case "lightupwarddiagonal":
                case "light_upward_diagonal":
                    return HatchStyle.LightUpwardDiagonal;
                case "lightvertical":
                case "light_vertical":
                    return HatchStyle.LightVertical;
                case "max":
                    return HatchStyle.Max;
                case "min":
                    return HatchStyle.Min;
                case "narrowhorizontal":
                case "narrow_horizontal":
                    return HatchStyle.NarrowHorizontal;
                case "narrowvertical":
                case "narrow_vertical":
                    return HatchStyle.Vertical;
                case "outlineddiamond":
                case "outlined_diamond":
                    return HatchStyle.OutlinedDiamond;
                case "percent05":
                case "percent_05":
                case "5%":
                    return HatchStyle.Percent05;
                case "percent10":
                case "percent_10":
                case "10%":
                    return HatchStyle.Percent10;
                case "percent20":
                case "percent_20":
                case "20%":
                    return HatchStyle.Percent20;
                case "percent25":
                case "percent_25":
                case "25%":
                    return HatchStyle.Percent25;
                case "percent30":
                case "percent_30":
                case "30%":
                    return HatchStyle.Percent30;
                case "percent40":
                case "percent_40":
                case "40%":
                    return HatchStyle.Percent40;
                case "percent50":
                case "percent_50":
                case "50%":
                    return HatchStyle.Percent50;
                case "percent60":
                case "percent_60":
                case "60%":
                    return HatchStyle.Percent60;
                case "percent70":
                case "percent_70":
                case "70%":
                    return HatchStyle.Percent70;
                case "percent75":
                case "percent_75":
                case "75%":
                    return HatchStyle.Percent75;
                case "percent80":
                case "percent_80":
                case "80%":
                    return HatchStyle.Percent80;
                case "percent90":
                case "percent_90":
                case "90%":
                    return HatchStyle.Percent90;
                case "plaid":
                    return HatchStyle.Plaid;
                case "shingle":
                    return HatchStyle.Shingle;
                case "smallcheckerboard":
                case "small_checker_board":
                    return HatchStyle.SmallCheckerBoard;
                case "smallconfetti":
                case "small_confetti":
                    return HatchStyle.SmallConfetti;
                case "smallgrid":
                case "small_grid":
                    return HatchStyle.SmallGrid;
                case "soliddiamond":
                case "solid_diamond":
                    return HatchStyle.SolidDiamond;
                case "sphere":
                    return HatchStyle.Sphere;
                case "trellis":
                    return HatchStyle.Trellis;
                case "vertical":
                    return HatchStyle.Vertical;
                case "wave":
                    return HatchStyle.Wave;
                case "weave":
                    return HatchStyle.Weave;
                case "widedownwarddiagonal":
                case "wide_downward_diagonal":
                    return HatchStyle.WideDownwardDiagonal;
                case "wideupwarddiagonal":
                case "wide_upward_diagonal":
                    return HatchStyle.WideUpwardDiagonal;
                case "zigzag":
                case "zig_zag":
                    return HatchStyle.ZigZag;
                default:
                    return HatchStyle.Plaid;
            }
        }
    }
}
