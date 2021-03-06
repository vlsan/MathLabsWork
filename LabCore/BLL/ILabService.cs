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
        List<StatisticViewModel> GetTheoreticalFrequencies(List<LabDataViewModel> items);
        ResultViewModel GetEmpiricalAndTheoreticalFrequencies(List<LabDataViewModel> items);
        double GetSampleMean();
        double GetVarianceEstimation();
        /// <summary>
        /// Значение Ассиметрии, Эксцесс, 
        /// </summary>
        /// <param name="requestType">"assimetry", "excess", "samplemean" </param>
        /// <returns></returns>
        double GetValueByReguest(string requestType);
        #endregion
    }
}
