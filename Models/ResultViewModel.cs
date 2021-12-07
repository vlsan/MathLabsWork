using System.Collections.Generic;

namespace WebMathModelLabs.Models
{
    public class ResultViewModel 
    {
        public string StringValue { get; set; }
        public string StringValue1 { get; set; }
        public double DoubleValue { get; set; }
        public double DoubleValue1 { get; set; }
        public List<StatisticViewModel>  StatisticViewModels { get; set; }
    }
}
