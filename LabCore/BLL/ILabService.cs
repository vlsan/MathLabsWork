using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMathModelLabs.Models;

namespace WebMathModelLabs.LabCore.BLL
{
    public interface ILabService
    {
        #region Edit data table
        bool AddData(string SValue);

        List<LabDataViewModel> GetLabDataViewModels();
        List<StatisticViewModel> StatisticalSeries(List<LabDataViewModel> items);
        List<StatisticViewModel> EmpiricalFrequencies(List<LabDataViewModel> items);
        double GetSampleMean();
        double GetVarianceEstimation();
        #endregion
    }
}
