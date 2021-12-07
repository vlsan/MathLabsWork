using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WebMathModelLabs.Data.Repository;
using WebMathModelLabs.Entity;
using WebMathModelLabs.Models;

namespace WebMathModelLabs.LabCore.BLL
{
    public class LabDatamanager : ILabService
    {

        private readonly IRepository<MathLabTable1> _repository;
        
        public LabDatamanager(IRepository<MathLabTable1> repository)
        {
            _repository = repository;
        }

        public bool AddData(string SValue)
        {
            try
            {
                var entityItem = new MathLabTable1 { SValue = SValue };
                _repository.Add(entityItem);
                _repository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public List<LabDataViewModel> GetLabDataViewModels()
        {
            var res = _repository.GetAll().Select(s=> new LabDataViewModel { Id= s.Id, SValue= s.SValue }).ToList();
            return res;
        }

        public double GetSampleMean()
        {
            var tableData = _repository.GetAll()
                .Select(s => new Pair<int, double> { Key= s.Id, Value= double.Parse(s.SValue.Trim()) });
            var res = MathHelper.SampleMean(tableData);


            return Math.Round(res,4);
        }
        public double GetVarianceEstimation()
        {
            var sampleMean = GetSampleMean();
            var tableData = _repository.GetAll()
                .Select(s => new Pair<int, double> { Key = s.Id, Value = double.Parse(s.SValue.Trim()) });
            var res = MathHelper.VarianceEstimation(tableData, sampleMean);
            return Math.Round(res,4);
        }
        /// <summary>
        /// Значение Ассиметрии, Эксцесс, 
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        public double GetValueByReguest(string requestType)
        {
            var tableData = _repository.GetAll()
        .Select(s => new Pair<int, double> { Key = s.Id, Value = double.Parse(s.SValue.Trim()) });
            if (requestType == "assimetry")
                return MathHelper.AsymmetryCoefficient(tableData);
            if (requestType == "excess")
                return MathHelper.Excess(tableData);
            if(requestType == "samplemean")
                return MathHelper.SampleMean(tableData);
            return 0;
        }


        public List<StatisticViewModel> StatisticalSeries(List<LabDataViewModel> items)
        {
            var dict = items.ToDictionary(k => k.Id.ToString(), v => double.Parse(v.SValue.Trim()));
           
            var collection = new List<Pair<string, double>>();
            foreach (var i in dict)
            {
                var pair = new Pair<string, double>
                {
                    Key = i.Key,
                    Value = i.Value
                };

                collection.Add(pair);
            }
           
            var result = MathHelper.StatisticalSeries(collection).Select(s=> new StatisticViewModel { Skey = s.Key, SValue = Math.Round(s.Value, 2) }).ToList();
            return result;
        }

        public List<StatisticViewModel> EmpiricalFrequencies(List<LabDataViewModel> items)
        {
            var dict = items.ToDictionary(k => k.Id, v => double.Parse(v.SValue.Trim()));

            var collection = new List<Pair<int, double>>();
            foreach (var i in dict)
            {
                var pair = new Pair<int, double>
                {
                    Key = i.Key,
                    Value = i.Value
                };

                collection.Add(pair);
            }
            var result = MathHelper.EmpiricalFrequencies(collection).Select(s => new StatisticViewModel { Skey = s.Key, SValue = Math.Round(s.Value, 2) }).ToList();
            // EmpiricalAndTheoreticalFrequencies

            return result;
        }


        public List<StatisticViewModel> GetTheoreticalFrequencies(List<LabDataViewModel> items)
        {
            var dict = items.ToDictionary(k => k.Id, v => double.Parse(v.SValue.Trim()));

            var collection = new List<Pair<int, double>>();
            foreach (var i in dict)
            {
                var pair = new Pair<int, double>
                {
                    Key = i.Key,
                    Value = i.Value
                };

                collection.Add(pair);
            }
            var result = MathHelper.TheoreticalFrequencies(collection).Select(s => new StatisticViewModel { Skey = s.Key, SValue = Math.Round(s.Value, 2) }).ToList();
            return result;
        }

        public ResultViewModel GetEmpiricalAndTheoreticalFrequencies(List<LabDataViewModel> items)
        {
            var dict = items.ToDictionary(k => k.Id, v => double.Parse(v.SValue.Trim()));

            var collection = new List<Pair<int, double>>();
            foreach (var i in dict)
            {
                var pair = new Pair<int, double>
                {
                    Key = i.Key,
                    Value = i.Value
                };

                collection.Add(pair);
            }

            var result = new ResultViewModel();
            var _resCollection = MathHelper.EmpiricalAndTheoreticalFrequencies(collection);
            var resCollection = _resCollection.Select(s => new StatisticViewModel { Skey = s.Key, SValue = Math.Round(s.Value1, 4), SValue1 = Math.Round(s.Value2, 4) }).ToList();
            var pirson1 = MathHelper.PearsonTest(_resCollection, resCollection.Count());
            var pirson2 = MathHelper.СomparisonPearsonTest(pirson1, resCollection.Count());


            result.DoubleValue = pirson1;
            result.StringValue = pirson2;
            if(result.StatisticViewModels==null)
            {
                result.StatisticViewModels = new List<StatisticViewModel>();
            }
            result.StatisticViewModels.AddRange(resCollection);
            return result;
        }
    }
}
