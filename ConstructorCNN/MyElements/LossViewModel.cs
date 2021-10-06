using System;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

namespace ConstructorCNN
{
    public class LossViewModel
    {
        public LossViewModel() { }

        public string Title { get; private set; } = "Loss Function";

        public static IList<DataPoint> PointsTrain { get; set; }
        public static IList<DataPoint> PointsTest { get; set; }
    }
}
