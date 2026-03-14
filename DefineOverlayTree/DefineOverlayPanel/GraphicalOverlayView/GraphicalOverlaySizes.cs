namespace DefineOverlayTree.DefineOverlayPanel.GraphicalOverlayView
{
    public static class GraphicalOverlaySizes
    {
        public const int CellInnerContentMaginFromOuterBorder = 1;
        public const int CellInnerContentDoubleMaginFromOuterBorder = CellInnerContentMaginFromOuterBorder * 2;

        public const int OuterLayerRowHeight = 30;
        public const int InnerLayerRowHeight = OuterLayerRowHeight - CellInnerContentDoubleMaginFromOuterBorder;
        public const int LayerVerticalMargin = 3;
        public const int OuterLayerRowWithMarginHeight = OuterLayerRowHeight + LayerVerticalMargin;

        public const int OverlayColumnWidth = 35;

        public const int LayerNameWidth = 140;

        public const int CircleDiameter = 12;
        public const int CircleYRelativeToCell = (OuterLayerRowHeight - CircleDiameter) / 2;

        public const int ArrowPointHeight = 9;
        public const int ArrowPointWidth = 7;
        public const int ArrowLineWidth = 1;

        public const int CircleX = (OverlayColumnWidth - CircleDiameter) / 2;
        public const int ArrowPointX = (OverlayColumnWidth - ArrowPointWidth) / 2;
        public const int ArrowLineX = (OverlayColumnWidth - ArrowLineWidth) / 2;

        public const int ArrowPointYRelativeToCell = CircleYRelativeToCell - ArrowPointHeight;
        public const int ArrowLineStartYRelativeToCell = (OuterLayerRowHeight + CircleDiameter) / 2;
        public const int ArrowLineEndYRelativeToCell = ArrowPointYRelativeToCell + 2;
    }
}