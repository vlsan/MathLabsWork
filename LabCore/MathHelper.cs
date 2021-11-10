using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebMathModelLabs.LabCore
{
    public static class MathHelper
    {
        #region оценка математического ожидания случайной величины X.
        /// <summary>
        ///  Оценкой математического ожидания случайной величины X
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public double SampleMean(IEnumerable<Pair<int, double>> listNumb)
        {
            var sampleMean = 0d;
            foreach (var item in listNumb)
            {
                sampleMean += item.Value;
            }
            return sampleMean / ((double)listNumb.Count());
        }
        #endregion

        #region оценка дисперсии случайной величины X
        /// <summary>
        /// оценка дисперсии случайной величины X
        /// </summary>
        /// <param name="listNumb"></param>
        /// <param name="sampleMean"></param>
        /// <returns></returns>
        static public double VarianceEstimation(IEnumerable<Pair<int, double>> listNumb, double sampleMean)
        {
            var varianceEstimation = 0d;
            foreach (var item in listNumb)
            {
                varianceEstimation += Math.Pow((item.Value - sampleMean), 2);
            }
            return varianceEstimation / ((double)listNumb.Count() - 1d);
        }
        /// <summary>
        /// оценка дисперсии случайной величины X
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public double VarianceEstimation(IEnumerable<Pair<int, double>> listNumb)
        {
            return VarianceEstimation(listNumb, SampleMean(listNumb));
        }
        #endregion

        #region Среднее квадратичное отклонение
        /// <summary>
        /// Среднее квадратичное отклонение
        /// </summary>
        /// <param name="listNumb"></param>
        /// <param name="sampleMean"></param>
        /// <returns></returns>
        static public double StandardDeviation(IEnumerable<Pair<int, double>> listNumb, double sampleMean)
        {
            return Math.Sqrt(VarianceEstimation(listNumb, sampleMean));
        }
        /// <summary>
        /// Среднее квадратичное отклонение
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public double StandardDeviation(IEnumerable<Pair<int, double>> listNumb)
        {
            return StandardDeviation(listNumb, SampleMean(listNumb));
        }
        #endregion

        #region Формула Стерджесса
        /// <summary>
        /// Длина интервалов с использованием формулы Стерджесса
        /// </summary>
        /// <param name="listDelta"></param>
        /// <param name="listCount"></param>
        /// <returns></returns>
        static public double SturgessFormulaDelta(double listDelta, int listCount)
        {
            return (listDelta) / SturgessFormula(listCount);
        }

        /// <summary>
        /// формула Стерджесса
        /// </summary>
        /// <param name="listCount"></param>
        /// <returns></returns>
        static public double SturgessFormula(int listCount)
        {
            return Math.Floor(1d + 3.22d * Math.Log((double)listCount));
        }
        #endregion

        #region эмпирический закон и статистическа функция распределения случайной величины
        /// <summary>
        /// эмпирический закон случайной величины
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public ObservableCollection<Pair<string, double>> EmpiricalFrequencies(
            IEnumerable<Pair<int, double>> listNumb)
        {
            var min = listNumb.Min(m => m.Value);
            var max = listNumb.Max(m => m.Value);
            var dX = SturgessFormulaDelta(max - min, listNumb.Count());
            var listResult = new ObservableCollection<Pair<string, double>>();
            double i;
            var n = 0d;
            var j = 1;
            for (i = min; Math.Round(i + dX, 2, MidpointRounding.AwayFromZero) < Math.Round(max, 2, MidpointRounding.AwayFromZero); i += dX, j++)
            {
                n = 0d;
                foreach (var item in listNumb)
                {
                    if (item.Value >= i && item.Value < (i + dX))
                    {
                        n += 1d;
                    }
                }
                listResult.Add(new Pair<string, double>($"x{j}({i.ToString("F2")}) - x{j + 1}({(i + dX).ToString("F2")})",
                    (n / (double)listNumb.Count())));
            }
            n = 0d;
            foreach (var item in listNumb)
            {
                if (item.Value >= i && item.Value <= max)
                {
                    n += 1d;
                }
            }
            listResult.Add(new Pair<string, double>($"x{j}({i.ToString("F2")}) - x{j + 1}({max.ToString("F2")})",
                (n / (double)listNumb.Count())));
            return listResult;
        }
        /// <summary>
        /// статистическа функция распределения случайной величины
        /// </summary>
        /// <param name="empiricalFrequencies"></param>
        /// <returns></returns>
        static public ObservableCollection<Pair<string, double>> StatisticalSeries(
            ObservableCollection<Pair<string, double>> empiricalFrequencies)
        {
            var listResult = new ObservableCollection<Pair<string, double>>();
            listResult.Add(new Pair<string, double>(empiricalFrequencies[0].Key, empiricalFrequencies[0].Value));
            for (var i = 1; i < empiricalFrequencies.Count; i++)
            {
                listResult.Add(new Pair<string, double>(empiricalFrequencies[i].Key,
                    (empiricalFrequencies[i].Value) + listResult[i - 1].Value));
            }
            return listResult;
        }
        #endregion

        #region Коэффициент асимметрии и Эксцесс
        /// <summary>
        /// Коэффициент асимметрии
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public double AsymmetryCoefficient(IEnumerable<Pair<int, double>> listNumb)
        {
            var asymmetryCoefficient = 0d;
            var sampleMean = SampleMean(listNumb);
            var standardDeviation = StandardDeviation(listNumb, sampleMean);
            foreach (var item in listNumb)
            {
                asymmetryCoefficient += Math.Pow((item.Value - sampleMean), 3);
            }
            return asymmetryCoefficient / (((double)listNumb.Count() - 1d) * Math.Pow(standardDeviation, 3));
        }
        /// <summary>
        /// Эксцесс
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public double Excess(IEnumerable<Pair<int, double>> listNumb)
        {
            var excess = 0d;
            var sampleMean = SampleMean(listNumb);
            var standardDeviation = StandardDeviation(listNumb, sampleMean);
            foreach (var item in listNumb)
            {
                excess += Math.Pow((item.Value - sampleMean), 4);
            }
            return excess / (((double)listNumb.Count() - 1d) * Math.Pow(standardDeviation, 4)) - 3d;
        }
        #endregion

        #region Теоретические частоты попадания случайной величины в интервалы
        static public ObservableCollection<Pair<string, double>> TheoreticalFrequencies(
            IEnumerable<Pair<int, double>> listNumb)
        {
            var min = listNumb.Min(m => m.Value);
            var max = listNumb.Max(m => m.Value);
            var dX = SturgessFormulaDelta(max - min, listNumb.Count());
            var listResult = new ObservableCollection<Pair<string, double>>();
            double i;
            var j = 1;
            var sampleMean = SampleMean(listNumb);
            var standardDeviation = StandardDeviation(listNumb, sampleMean);
            var lap = new FunctionLaplas();
            for (i = min; Math.Round(i + dX, 2, MidpointRounding.AwayFromZero) < Math.Round(max, 2, MidpointRounding.AwayFromZero); i += dX, j++)
            {
                listResult.Add(new Pair<string, double>(
                    $"x{j}({i.ToString("F2")}) - x{j + 1}({(i + dX).ToString("F2")})",
                    (lap.Conculete(((i + dX) - sampleMean) / standardDeviation) - lap.Conculete(((i) - sampleMean) / standardDeviation))));
            }
            listResult.Add(new Pair<string, double>(
                $"x{j}({i.ToString("F2")}) - x{j + 1}({max.ToString("F2")})",
                    (lap.Conculete(((max) - sampleMean) / standardDeviation) - lap.Conculete(((i) - sampleMean) / standardDeviation))));
            return listResult;
        }

        #endregion

        #region Эмпирический закон и Теоретические частоты попадания случайной величины в интервалы
        /// <summary>
        /// Эмпирический закон и Теоретические частоты попадания случайной величины в интервалы
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public ObservableCollection<Triple<string, double, double>> EmpiricalAndTheoreticalFrequencies(
            IEnumerable<Pair<int, double>> listNumb)
        {
            var listResult = new ObservableCollection<Triple<string, double, double>>();
            var empiricalFrequencies = EmpiricalFrequencies(listNumb);
            var theoreticalFrequencies = TheoreticalFrequencies(listNumb);
            if (empiricalFrequencies.Count != theoreticalFrequencies.Count)
            {
                return null;
            }
            for (var i = 0; i < empiricalFrequencies.Count; i++)
            {
                if (empiricalFrequencies[i].Key != theoreticalFrequencies[i].Key)
                {
                    return null;
                }
                listResult.Add(new Triple<string, double, double>(empiricalFrequencies[i].Key,
                    theoreticalFrequencies[i].Value, empiricalFrequencies[i].Value));
            }
            return listResult;
        }

        #endregion

        #region Критерий Пирсона
        /// <summary>
        /// Критерий Пирсона
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public double PearsonTest(ObservableCollection<Triple<string, double, double>> theoreticalFrequenciesCalculate, int n)
        {
            var result = 0d;
            foreach (var item in theoreticalFrequenciesCalculate)
            {
                result += (Math.Pow((item.Value2 - item.Value1), 2) * (double)n) / item.Value1;
            }
            return result;
        }

        /// <summary>
        /// Сравнение Критерия Пирсона с таблиным
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public string СomparisonPearsonTest(double pearsonTest, int n)
        {
            var PearsonTestObj = new PearsonTest();
            return PearsonTestObj.Conculete(pearsonTest, n);
        }
        #endregion

        #region Критерий Фишера-Снедекора

        /// <summary>
        /// Сравнение Критерия Пирсона с таблиным
        /// </summary>
        /// <param name="listNumb"></param>
        /// <returns></returns>
        static public string СomparisonPhisherTest(double varianceEstimation1, double varianceEstimation2, int n1, int n2)
        {
            var phisherTest = new PhisherTest();
            return phisherTest.Conculete(varianceEstimation1, varianceEstimation2, n1, n2);
        }
        #endregion

    }

    [Serializable]
    [XmlType(TypeName = "Pair")]
    public class Pair<K, V>
    {
        public Pair()
        {
        }

        public Pair(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public K Key
        { get; set; }

        public V Value
        { get; set; }
    }
    public class Triple<K, V, Z>
    {
        public Triple()
        {
        }

        public Triple(K key, V value1, Z value2)
        {
            Key = key;
            Value1 = value1;
            Value2 = value2;
        }

        public K Key
        { get; set; }

        public V Value1
        { get; set; }

        public Z Value2
        { get; set; }

    }

    #region Класс Функции Лапласа
    public class FunctionLaplas
    {
        List<Pair<double, double>> table;

        public FunctionLaplas()
        {
            table = new List<Pair<double, double>>();
            table.Add(new Pair<double, double>(0.00d, 0.0000d));
            table.Add(new Pair<double, double>(0.50d, 0.1915d));
            table.Add(new Pair<double, double>(1.00d, 0.3413d));
            table.Add(new Pair<double, double>(1.50d, 0.4332d));
            table.Add(new Pair<double, double>(2.00d, 0.4772d));
            table.Add(new Pair<double, double>(3.00d, 0.49865d));
            table.Add(new Pair<double, double>(0.01d, 0.0040d));
            table.Add(new Pair<double, double>(0.51d, 0.1950d));
            table.Add(new Pair<double, double>(1.01d, 0.3438d));
            table.Add(new Pair<double, double>(1.51d, 0.4345d));
            table.Add(new Pair<double, double>(2.02d, 0.4783d));
            table.Add(new Pair<double, double>(3.20d, 0.49931d));
            table.Add(new Pair<double, double>(0.02d, 0.0080d));
            table.Add(new Pair<double, double>(0.52d, 0.1985d));
            table.Add(new Pair<double, double>(1.02d, 0.3461d));
            table.Add(new Pair<double, double>(1.52d, 0.4357d));
            table.Add(new Pair<double, double>(2.04d, 0.4793d));
            table.Add(new Pair<double, double>(3.40d, 0.49966d));
            table.Add(new Pair<double, double>(0.03d, 0.0120d));
            table.Add(new Pair<double, double>(0.53d, 0.2019d));
            table.Add(new Pair<double, double>(1.03d, 0.3485d));
            table.Add(new Pair<double, double>(1.53d, 0.4370d));
            table.Add(new Pair<double, double>(2.06d, 0.4803d));
            table.Add(new Pair<double, double>(3.60d, 0.499841d));
            table.Add(new Pair<double, double>(0.04d, 0.0160d));
            table.Add(new Pair<double, double>(0.54d, 0.2054d));
            table.Add(new Pair<double, double>(1.04d, 0.3508d));
            table.Add(new Pair<double, double>(1.54d, 0.4382d));
            table.Add(new Pair<double, double>(2.08d, 0.4812d));
            table.Add(new Pair<double, double>(3.80d, 0.499928d));
            table.Add(new Pair<double, double>(0.05d, 0.0199d));
            table.Add(new Pair<double, double>(0.55d, 0.2088d));
            table.Add(new Pair<double, double>(1.05d, 0.3531d));
            table.Add(new Pair<double, double>(1.55d, 0.4394d));
            table.Add(new Pair<double, double>(2.10d, 0.4821d));
            table.Add(new Pair<double, double>(4.00d, 0.499968d));
            table.Add(new Pair<double, double>(0.06d, 0.0239d));
            table.Add(new Pair<double, double>(0.56d, 0.2123d));
            table.Add(new Pair<double, double>(1.06d, 0.3554d));
            table.Add(new Pair<double, double>(1.56d, 0.4406d));
            table.Add(new Pair<double, double>(2.12d, 0.4830d));
            table.Add(new Pair<double, double>(4.50d, 0.499997d));
            table.Add(new Pair<double, double>(0.07d, 0.0279d));
            table.Add(new Pair<double, double>(0.57d, 0.2157d));
            table.Add(new Pair<double, double>(1.07d, 0.3577d));
            table.Add(new Pair<double, double>(1.57d, 0.4418d));
            table.Add(new Pair<double, double>(2.14d, 0.4838d));
            table.Add(new Pair<double, double>(5.00d, 0.499997d));
            table.Add(new Pair<double, double>(0.08d, 0.0319d));
            table.Add(new Pair<double, double>(0.58d, 0.2190d));
            table.Add(new Pair<double, double>(1.08d, 0.3599d));
            table.Add(new Pair<double, double>(1.58d, 0.4429d));
            table.Add(new Pair<double, double>(2.16d, 0.4846d));
            table.Add(new Pair<double, double>(0.09d, 0.0359d));
            table.Add(new Pair<double, double>(0.59d, 0.2224d));
            table.Add(new Pair<double, double>(1.09d, 0.3621d));
            table.Add(new Pair<double, double>(1.59d, 0.4441d));
            table.Add(new Pair<double, double>(2.18d, 0.4854d));
            table.Add(new Pair<double, double>(0.10d, 0.0398d));
            table.Add(new Pair<double, double>(0.60d, 0.2257d));
            table.Add(new Pair<double, double>(1.10d, 0.3643d));
            table.Add(new Pair<double, double>(1.60d, 0.4452d));
            table.Add(new Pair<double, double>(2.20d, 0.4861d));
            table.Add(new Pair<double, double>(0.11d, 0.0438d));
            table.Add(new Pair<double, double>(0.61d, 0.2291d));
            table.Add(new Pair<double, double>(1.11d, 0.3665d));
            table.Add(new Pair<double, double>(1.61d, 0.4463d));
            table.Add(new Pair<double, double>(2.22d, 0.4868d));
            table.Add(new Pair<double, double>(0.12d, 0.0478d));
            table.Add(new Pair<double, double>(0.62d, 0.2324d));
            table.Add(new Pair<double, double>(1.12d, 0.3686d));
            table.Add(new Pair<double, double>(1.62d, 0.4474d));
            table.Add(new Pair<double, double>(2.24d, 0.4875d));
            table.Add(new Pair<double, double>(0.13d, 0.0517d));
            table.Add(new Pair<double, double>(0.63d, 0.2357d));
            table.Add(new Pair<double, double>(1.13d, 0.3708d));
            table.Add(new Pair<double, double>(1.63d, 0.4484d));
            table.Add(new Pair<double, double>(2.26d, 0.4881d));
            table.Add(new Pair<double, double>(0.14d, 0.0557d));
            table.Add(new Pair<double, double>(0.64d, 0.2389d));
            table.Add(new Pair<double, double>(1.14d, 0.3729d));
            table.Add(new Pair<double, double>(1.64d, 0.4495d));
            table.Add(new Pair<double, double>(2.28d, 0.4887d));
            table.Add(new Pair<double, double>(0.15d, 0.0596d));
            table.Add(new Pair<double, double>(0.65d, 0.2422d));
            table.Add(new Pair<double, double>(1.15d, 0.3749d));
            table.Add(new Pair<double, double>(1.65d, 0.4505d));
            table.Add(new Pair<double, double>(2.30d, 0.4893d));
            table.Add(new Pair<double, double>(0.16d, 0.0636d));
            table.Add(new Pair<double, double>(0.66d, 0.2454d));
            table.Add(new Pair<double, double>(1.16d, 0.3770d));
            table.Add(new Pair<double, double>(1.66d, 0.4515d));
            table.Add(new Pair<double, double>(2.32d, 0.4898d));
            table.Add(new Pair<double, double>(0.17d, 0.0675d));
            table.Add(new Pair<double, double>(0.67d, 0.2486d));
            table.Add(new Pair<double, double>(1.17d, 0.3790d));
            table.Add(new Pair<double, double>(1.67d, 0.4525d));
            table.Add(new Pair<double, double>(2.34d, 0.4904d));
            table.Add(new Pair<double, double>(0.18d, 0.0714d));
            table.Add(new Pair<double, double>(0.68d, 0.2517d));
            table.Add(new Pair<double, double>(1.18d, 0.3810d));
            table.Add(new Pair<double, double>(1.68d, 0.4535d));
            table.Add(new Pair<double, double>(2.36d, 0.4909d));
            table.Add(new Pair<double, double>(0.19d, 0.0753d));
            table.Add(new Pair<double, double>(0.69d, 0.2549d));
            table.Add(new Pair<double, double>(1.19d, 0.3830d));
            table.Add(new Pair<double, double>(1.69d, 0.4545d));
            table.Add(new Pair<double, double>(2.38d, 0.4913d));
            table.Add(new Pair<double, double>(0.20d, 0.0793d));
            table.Add(new Pair<double, double>(0.70d, 0.2580d));
            table.Add(new Pair<double, double>(1.20d, 0.3849d));
            table.Add(new Pair<double, double>(1.70d, 0.4554d));
            table.Add(new Pair<double, double>(2.40d, 0.4918d));
            table.Add(new Pair<double, double>(0.21d, 0.0832d));
            table.Add(new Pair<double, double>(0.71d, 0.2611d));
            table.Add(new Pair<double, double>(1.21d, 0.3869d));
            table.Add(new Pair<double, double>(1.71d, 0.4564d));
            table.Add(new Pair<double, double>(2.42d, 0.4922d));
            table.Add(new Pair<double, double>(0.22d, 0.0871d));
            table.Add(new Pair<double, double>(0.72d, 0.2642d));
            table.Add(new Pair<double, double>(1.22d, 0.3883d));
            table.Add(new Pair<double, double>(1.72d, 0.4573d));
            table.Add(new Pair<double, double>(2.44d, 0.4927d));
            table.Add(new Pair<double, double>(0.23d, 0.0910d));
            table.Add(new Pair<double, double>(0.73d, 0.2673d));
            table.Add(new Pair<double, double>(1.23d, 0.3907d));
            table.Add(new Pair<double, double>(1.73d, 0.4582d));
            table.Add(new Pair<double, double>(2.46d, 0.4931d));
            table.Add(new Pair<double, double>(0.24d, 0.0948d));
            table.Add(new Pair<double, double>(0.74d, 0.2703d));
            table.Add(new Pair<double, double>(1.24d, 0.3925d));
            table.Add(new Pair<double, double>(1.74d, 0.4591d));
            table.Add(new Pair<double, double>(2.48d, 0.4934d));
            table.Add(new Pair<double, double>(0.25d, 0.0987d));
            table.Add(new Pair<double, double>(0.75d, 0.2734d));
            table.Add(new Pair<double, double>(1.25d, 0.3944d));
            table.Add(new Pair<double, double>(1.75d, 0.4599d));
            table.Add(new Pair<double, double>(2.50d, 0.4938d));
            table.Add(new Pair<double, double>(0.26d, 0.1026d));
            table.Add(new Pair<double, double>(0.76d, 0.2764d));
            table.Add(new Pair<double, double>(1.26d, 0.3962d));
            table.Add(new Pair<double, double>(1.76d, 0.4608d));
            table.Add(new Pair<double, double>(2.52d, 0.4941d));
            table.Add(new Pair<double, double>(0.27d, 0.1064d));
            table.Add(new Pair<double, double>(0.77d, 0.2794d));
            table.Add(new Pair<double, double>(1.27d, 0.3980d));
            table.Add(new Pair<double, double>(1.77d, 0.4616d));
            table.Add(new Pair<double, double>(2.54d, 0.4945d));
            table.Add(new Pair<double, double>(0.28d, 0.1103d));
            table.Add(new Pair<double, double>(0.78d, 0.2823d));
            table.Add(new Pair<double, double>(1.28d, 0.3997d));
            table.Add(new Pair<double, double>(1.78d, 0.4625d));
            table.Add(new Pair<double, double>(2.56d, 0.4948d));
            table.Add(new Pair<double, double>(0.29d, 0.1141d));
            table.Add(new Pair<double, double>(0.79d, 0.2852d));
            table.Add(new Pair<double, double>(1.29d, 0.4015d));
            table.Add(new Pair<double, double>(1.79d, 0.4633d));
            table.Add(new Pair<double, double>(2.58d, 0.4951d));
            table.Add(new Pair<double, double>(0.30d, 0.1179d));
            table.Add(new Pair<double, double>(0.80d, 0.2881d));
            table.Add(new Pair<double, double>(1.30d, 0.4032d));
            table.Add(new Pair<double, double>(1.80d, 0.4641d));
            table.Add(new Pair<double, double>(2.60d, 0.4953d));
            table.Add(new Pair<double, double>(0.31d, 0.1217d));
            table.Add(new Pair<double, double>(0.81d, 0.2910d));
            table.Add(new Pair<double, double>(1.31d, 0.4049d));
            table.Add(new Pair<double, double>(1.81d, 0.4649d));
            table.Add(new Pair<double, double>(2.62d, 0.4956d));
            table.Add(new Pair<double, double>(0.32d, 0.1255d));
            table.Add(new Pair<double, double>(0.82d, 0.2939d));
            table.Add(new Pair<double, double>(1.32d, 0.4066d));
            table.Add(new Pair<double, double>(1.82d, 0.4656d));
            table.Add(new Pair<double, double>(2.64d, 0.4959d));
            table.Add(new Pair<double, double>(0.33d, 0.1293d));
            table.Add(new Pair<double, double>(0.83d, 0.2967d));
            table.Add(new Pair<double, double>(1.33d, 0.4082d));
            table.Add(new Pair<double, double>(1.83d, 0.4664d));
            table.Add(new Pair<double, double>(2.66d, 0.4961d));
            table.Add(new Pair<double, double>(0.34d, 0.1331d));
            table.Add(new Pair<double, double>(0.84d, 0.2995d));
            table.Add(new Pair<double, double>(1.34d, 0.4099d));
            table.Add(new Pair<double, double>(1.84d, 0.4671d));
            table.Add(new Pair<double, double>(2.68d, 0.4963d));
            table.Add(new Pair<double, double>(0.35d, 0.1368d));
            table.Add(new Pair<double, double>(0.85d, 0.3023d));
            table.Add(new Pair<double, double>(1.35d, 0.4115d));
            table.Add(new Pair<double, double>(1.85d, 0.4678d));
            table.Add(new Pair<double, double>(2.70d, 0.4965d));
            table.Add(new Pair<double, double>(0.36d, 0.1406d));
            table.Add(new Pair<double, double>(0.86d, 0.3051d));
            table.Add(new Pair<double, double>(1.36d, 0.4131d));
            table.Add(new Pair<double, double>(1.86d, 0.4686d));
            table.Add(new Pair<double, double>(2.72d, 0.4967d));
            table.Add(new Pair<double, double>(0.37d, 0.1443d));
            table.Add(new Pair<double, double>(0.87d, 0.3078d));
            table.Add(new Pair<double, double>(1.37d, 0.4147d));
            table.Add(new Pair<double, double>(1.87d, 0.4693d));
            table.Add(new Pair<double, double>(2.74d, 0.4969d));
            table.Add(new Pair<double, double>(0.38d, 0.1480d));
            table.Add(new Pair<double, double>(0.88d, 0.3106d));
            table.Add(new Pair<double, double>(1.38d, 0.4162d));
            table.Add(new Pair<double, double>(1.88d, 0.4699d));
            table.Add(new Pair<double, double>(2.76d, 0.4971d));
            table.Add(new Pair<double, double>(0.39d, 0.1517d));
            table.Add(new Pair<double, double>(0.89d, 0.3133d));
            table.Add(new Pair<double, double>(1.39d, 0.4177d));
            table.Add(new Pair<double, double>(1.89d, 0.4706d));
            table.Add(new Pair<double, double>(2.78d, 0.4973d));
            table.Add(new Pair<double, double>(0.40d, 0.1554d));
            table.Add(new Pair<double, double>(0.90d, 0.3159d));
            table.Add(new Pair<double, double>(1.40d, 0.4192d));
            table.Add(new Pair<double, double>(1.90d, 0.4713d));
            table.Add(new Pair<double, double>(2.80d, 0.4974d));
            table.Add(new Pair<double, double>(0.41d, 0.1591d));
            table.Add(new Pair<double, double>(0.91d, 0.3186d));
            table.Add(new Pair<double, double>(1.41d, 0.4207d));
            table.Add(new Pair<double, double>(1.91d, 0.4719d));
            table.Add(new Pair<double, double>(2.82d, 0.4976d));
            table.Add(new Pair<double, double>(0.42d, 0.1628d));
            table.Add(new Pair<double, double>(0.92d, 0.3212d));
            table.Add(new Pair<double, double>(1.42d, 0.4222d));
            table.Add(new Pair<double, double>(1.92d, 0.4726d));
            table.Add(new Pair<double, double>(2.84d, 0.4977d));
            table.Add(new Pair<double, double>(0.43d, 0.1664d));
            table.Add(new Pair<double, double>(0.93d, 0.3238d));
            table.Add(new Pair<double, double>(1.43d, 0.4236d));
            table.Add(new Pair<double, double>(1.93d, 0.4732d));
            table.Add(new Pair<double, double>(2.86d, 0.4979d));
            table.Add(new Pair<double, double>(0.44d, 0.1700d));
            table.Add(new Pair<double, double>(0.94d, 0.3264d));
            table.Add(new Pair<double, double>(1.44d, 0.4251d));
            table.Add(new Pair<double, double>(1.94d, 0.4738d));
            table.Add(new Pair<double, double>(2.88d, 0.4980d));
            table.Add(new Pair<double, double>(0.45d, 0.1736d));
            table.Add(new Pair<double, double>(0.95d, 0.3289d));
            table.Add(new Pair<double, double>(1.45d, 0.4265d));
            table.Add(new Pair<double, double>(1.95d, 0.4744d));
            table.Add(new Pair<double, double>(2.90d, 0.4981d));
            table.Add(new Pair<double, double>(0.46d, 0.1772d));
            table.Add(new Pair<double, double>(0.96d, 0.3315d));
            table.Add(new Pair<double, double>(1.46d, 0.4279d));
            table.Add(new Pair<double, double>(1.96d, 0.4750d));
            table.Add(new Pair<double, double>(2.92d, 0.4982d));
            table.Add(new Pair<double, double>(0.47d, 0.1808d));
            table.Add(new Pair<double, double>(0.97d, 0.3340d));
            table.Add(new Pair<double, double>(1.47d, 0.4292d));
            table.Add(new Pair<double, double>(1.97d, 0.4756d));
            table.Add(new Pair<double, double>(2.94d, 0.4984d));
            table.Add(new Pair<double, double>(0.48d, 0.1844d));
            table.Add(new Pair<double, double>(0.98d, 0.3365d));
            table.Add(new Pair<double, double>(1.48d, 0.4306d));
            table.Add(new Pair<double, double>(1.98d, 0.4761d));
            table.Add(new Pair<double, double>(2.96d, 0.4985d));
            table.Add(new Pair<double, double>(0.49d, 0.1879d));
            table.Add(new Pair<double, double>(0.99d, 0.3389d));
            table.Add(new Pair<double, double>(1.49d, 0.4319d));
            table.Add(new Pair<double, double>(1.99d, 0.4767d));
            table.Sort((x, y) => x.Key.CompareTo(y.Key));
        }

        public double Conculete(double d)
        {
            var sign = 1d;
            if (d < 0)
            {
                d *= (-1d);
                sign = -1d;
            }
            var result = 0d;
            var last = 0d;
            foreach (var item in table)
            {
                if (item.Key > d)
                {
                    if ((item.Key + last) / 2d < d)
                    {

                        result = item.Value;
                    }
                    return result * sign;
                }
                result = item.Value;
                last = item.Key;
            }
            return result * sign;
        }
    }
    #endregion

    #region Таблица критерия Пирсона
    public class PearsonTest
    {
        List<PearsonTestOne> table;

        public PearsonTest()
        {
            table = new List<PearsonTestOne>();
            table.Add(new PearsonTestOne(1, 6.6d, 5d, 3.8d, 0.0039d, 0.00098d, 0.00016d));
            table.Add(new PearsonTestOne(2, 9.2d, 7.4d, 6d, 0.103d, 0.051d, 0.02d));
            table.Add(new PearsonTestOne(3, 11.3d, 9.4d, 7.8d, 0.352d, 0.216d, 0.115d));
            table.Add(new PearsonTestOne(4, 13.3d, 11.1d, 9.5d, 0.711d, 0.484d, 0.297));
            table.Add(new PearsonTestOne(5, 15.1d, 12.8d, 11.1d, 1.15d, 0.831d, 0.554d));
            table.Add(new PearsonTestOne(6, 16.8d, 14.4d, 12.6d, 1.64d, 1.24d, 0.872d));
            table.Add(new PearsonTestOne(7, 18.5d, 16d, 14.1d, 2.17d, 1.69d, 1.14d));
            table.Add(new PearsonTestOne(8, 20.1d, 17.5d, 15.5d, 2.73d, 2.18d, 1.65d));
            table.Add(new PearsonTestOne(9, 21.7d, 19d, 16.9d, 3.33d, 2.7d, 2.09d));
            table.Add(new PearsonTestOne(10, 23.2d, 20.5d, 18.3d, 3.94d, 3.25d, 2.56d));
            table.Add(new PearsonTestOne(11, 24.7d, 21.9d, 19.7d, 4.57d, 3.82d, 3.05d));
            table.Add(new PearsonTestOne(12, 26.2d, 23.3d, 21d, 5.23d, 4.4d, 3.57d));
            table.Add(new PearsonTestOne(13, 27.7d, 24.7d, 22.4d, 5.8d, 5.01d, 4.11d));
            table.Add(new PearsonTestOne(14, 29.1d, 26.1d, 23.7d, 6.57d, 5.63d, 4.66d));
            table.Add(new PearsonTestOne(15, 30.6d, 27.5d, 25d, 7.26d, 6.26d, 5.23d));
            table.Add(new PearsonTestOne(16, 32d, 28.8d, 26.3d, 7.96d, 6.91d, 5.81d));
            table.Add(new PearsonTestOne(17, 33.4d, 30.2d, 27.6d, 8.67d, 7.56d, 6.41d));
            table.Add(new PearsonTestOne(18, 34.8d, 31.5d, 28.9d, 9.39d, 8.23d, 7.01d));
            table.Add(new PearsonTestOne(19, 36.2d, 32.9d, 30.1d, 10.1d, 8.91d, 7.63d));
            table.Add(new PearsonTestOne(20, 37.6d, 34.2d, 31.4d, 10.9d, 9.59d, 8.26d));
            table.Add(new PearsonTestOne(21, 38.9d, 35.5d, 32.7d, 11.6d, 10.3d, 8.9d));
            table.Add(new PearsonTestOne(22, 40.3d, 36.8d, 33.9d, 12.3d, 11d, 9.54d));
            table.Add(new PearsonTestOne(23, 42.6d, 38.1d, 35.2d, 13.1d, 11.7d, 10.2d));
            table.Add(new PearsonTestOne(24, 43d, 39.4d, 36.4d, 13.8d, 12.4d, 10.9d));
            table.Add(new PearsonTestOne(25, 44.3d, 40.6d, 37.7d, 14.6d, 13.1d, 11.5d));
            table.Add(new PearsonTestOne(26, 45.6d, 41.9d, 38.9d, 15.4d, 13.8d, 12.2d));
            table.Add(new PearsonTestOne(27, 47d, 43.2d, 40.1d, 16.2d, 14.6d, 12.9d));
            table.Add(new PearsonTestOne(28, 48.3d, 44.5d, 41.3d, 16.9d, 15.3d, 13.6d));
            table.Add(new PearsonTestOne(29, 49.6d, 45.7d, 42.6d, 17.7d, 16d, 14.3d));
            table.Add(new PearsonTestOne(30, 50.9d, 47d, 43.8d, 18.5d, 16.8d, 15d));
        }

        public string Conculete(double pearsonTest, int n)
        {
            n = n - 1;
            if (n < 1 || n > 30)
            {
                return "Такого n нет в таблице со значениями распределений";
            }
            if (pearsonTest > table.First(m => m.Key == n).Value1)
            {
                return "Данное распределение не нормальное при уровнях значимости: 0.01 , 0.025 , 0.05 , 0.95 , 0.975 , 0.99";
            }

            if (pearsonTest > table.First(m => m.Key == n).Value2)
            {
                return "Данное распределение нормальное при уровнях значимости: 0.01 \n" +
                    "и Данное распределение не нормальное при уровнях значимости: 0.025 , 0.05 , 0.95 , 0.975 , 0.99";
            }
            if (pearsonTest > table.First(m => m.Key == n).Value3)
            {
                return "Данное распределение нормальное при уровнях значимости: 0.01 , 0.025\n" +
                    "и Данное распределение не нормальное при уровнях значимости: 0.05 , 0.95 , 0.975 , 0.99";
            }
            if (pearsonTest > table.First(m => m.Key == n).Value4)
            {
                return "Данное распределение нормальное при уровнях значимости: 0.01 , 0.025 , 0.05\n" +
                    "и Данное распределение не нормальное при уровнях значимости: 0.95 , 0.975 , 0.99";
            }
            if (pearsonTest > table.First(m => m.Key == n).Value5)
            {
                return "Данное распределение нормальное при уровнях значимости: 0.01 , 0.025 , 0.05, 0.95\n" +
                    "и Данное распределение не нормальное при уровнях значимости: 0.975 , 0.99";
            }
            if (pearsonTest > table.First(m => m.Key == n).Value6)
            {
                return "Данное распределение нормальное при уровнях значимости: 0.01 , 0.025 , 0.05 , 0.95 , 0.975\n" +
                    "и Данное распределение не нормальное при уровнях значимости: 0.99";
            }
            return "Данное распределение нормальное при уровнях значимости: 0.01 , 0.025 , 0.05 , 0.95 , 0.975 , 0.99";
        }
    }
    public class PearsonTestOne
    {
        public PearsonTestOne(int key, double value1, double value2, double value3, double value4, double value5, double value6)
        {
            Key = key;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
        }

        public int Key
        { get; set; }

        public double Value1
        { get; set; }

        public double Value2
        { get; set; }
        public double Value3
        { get; set; }
        public double Value4
        { get; set; }
        public double Value5
        { get; set; }
        public double Value6
        { get; set; }
    }
    #endregion

    #region Таблица критерия Фишера-Снедекора
    public class PhisherTest
    {
        List<PhisherTestOne> table;

        public PhisherTest()
        {
            table = new List<PhisherTestOne>();
            table.Add(new PhisherTestOne(1, 1, 4052d, 161d));
            table.Add(new PhisherTestOne(1, 2, 98.49d, 18.51d));
            table.Add(new PhisherTestOne(1, 3, 34.12d, 10.13d));
            table.Add(new PhisherTestOne(1, 4, 21.2d, 7.71d));
            table.Add(new PhisherTestOne(1, 5, 16.26d, 6.61d));
            table.Add(new PhisherTestOne(1, 6, 13.74d, 5.99d));
            table.Add(new PhisherTestOne(1, 7, 12.25d, 5.59d));
            table.Add(new PhisherTestOne(1, 8, 11.26d, 5.32d));
            table.Add(new PhisherTestOne(1, 9, 10.56d, 5.12d));
            table.Add(new PhisherTestOne(1, 10, 10.04d, 4.96d));
            table.Add(new PhisherTestOne(1, 11, 9.86d, 4.84d));
            table.Add(new PhisherTestOne(1, 12, 9.33d, 4.75d));
            table.Add(new PhisherTestOne(1, 13, 9.07d, 4.67d));
            table.Add(new PhisherTestOne(1, 14, 8.86d, 4.6d));
            table.Add(new PhisherTestOne(1, 15, 8.68d, 4.54d));
            table.Add(new PhisherTestOne(1, 16, 8.53d, 4.49d));
            table.Add(new PhisherTestOne(1, 17, 8.4d, 4.45d));
            table.Add(new PhisherTestOne(2, 1, 4999d, 200d));
            table.Add(new PhisherTestOne(2, 2, 99.01d, 19d));
            table.Add(new PhisherTestOne(2, 3, 30.81d, 9.55d));
            table.Add(new PhisherTestOne(2, 4, 18d, 6.94d));
            table.Add(new PhisherTestOne(2, 5, 13.27d, 5.79d));
            table.Add(new PhisherTestOne(2, 6, 10.92d, 5.14d));
            table.Add(new PhisherTestOne(2, 7, 9.55d, 4.74d));
            table.Add(new PhisherTestOne(2, 8, 8.65d, 4.26d));
            table.Add(new PhisherTestOne(2, 9, 8.02d, 4.26d));
            table.Add(new PhisherTestOne(2, 10, 7.56d, 4.1d));
            table.Add(new PhisherTestOne(2, 11, 7.2d, 3.98d));
            table.Add(new PhisherTestOne(2, 12, 6.93d, 3.88d));
            table.Add(new PhisherTestOne(2, 13, 6.7d, 3.8d));
            table.Add(new PhisherTestOne(2, 14, 6.51d, 3.74d));
            table.Add(new PhisherTestOne(2, 15, 6.36d, 3.68d));
            table.Add(new PhisherTestOne(2, 16, 6.23d, 3.63d));
            table.Add(new PhisherTestOne(2, 17, 6.11d, 3.59d));
            table.Add(new PhisherTestOne(3, 1, 5403d, 216d));
            table.Add(new PhisherTestOne(3, 2, 99.17d, 19.25d));
            table.Add(new PhisherTestOne(3, 3, 29.46d, 9.55d));
            table.Add(new PhisherTestOne(3, 4, 16.69d, 6.94d));
            table.Add(new PhisherTestOne(3, 5, 12.06d, 5.79d));
            table.Add(new PhisherTestOne(3, 6, 9.78d, 5.14d));
            table.Add(new PhisherTestOne(3, 7, 8.45d, 4.74d));
            table.Add(new PhisherTestOne(3, 8, 7.59d, 4.26d));
            table.Add(new PhisherTestOne(3, 9, 6.99d, 4.26d));
            table.Add(new PhisherTestOne(3, 10, 6.55d, 4.1d));
            table.Add(new PhisherTestOne(3, 11, 6.22d, 3.98d));
            table.Add(new PhisherTestOne(3, 12, 5.95d, 3.88d));
            table.Add(new PhisherTestOne(3, 13, 5.74d, 3.8d));
            table.Add(new PhisherTestOne(3, 14, 5.56d, 3.74d));
            table.Add(new PhisherTestOne(3, 15, 5.42d, 3.68d));
            table.Add(new PhisherTestOne(3, 16, 5.29d, 3.63d));
            table.Add(new PhisherTestOne(3, 17, 5.18d, 3.59d));
            table.Add(new PhisherTestOne(4, 1, 5625d, 225d));
            table.Add(new PhisherTestOne(4, 2, 99.25d, 19.25d));
            table.Add(new PhisherTestOne(4, 3, 28.71d, 9.12d));
            table.Add(new PhisherTestOne(4, 4, 15.98d, 6.39d));
            table.Add(new PhisherTestOne(4, 5, 11.39d, 5.19d));
            table.Add(new PhisherTestOne(4, 6, 9.15d, 4.53d));
            table.Add(new PhisherTestOne(4, 7, 7.85d, 4.12d));
            table.Add(new PhisherTestOne(4, 8, 7.01d, 3.84d));
            table.Add(new PhisherTestOne(4, 9, 6.42d, 3.63d));
            table.Add(new PhisherTestOne(4, 10, 5.99d, 3.48d));
            table.Add(new PhisherTestOne(4, 11, 5.67d, 3.36d));
            table.Add(new PhisherTestOne(4, 12, 5.41d, 3.26d));
            table.Add(new PhisherTestOne(4, 13, 5.2d, 3.18d));
            table.Add(new PhisherTestOne(4, 14, 5.03d, 3.11d));
            table.Add(new PhisherTestOne(4, 15, 4.89d, 3.06d));
            table.Add(new PhisherTestOne(4, 16, 4.77d, 3.01d));
            table.Add(new PhisherTestOne(4, 17, 4.67d, 2.96d));
            table.Add(new PhisherTestOne(5, 1, 5764d, 230d));
            table.Add(new PhisherTestOne(5, 2, 99.3d, 19.3d));
            table.Add(new PhisherTestOne(5, 3, 28.24d, 9.01d));
            table.Add(new PhisherTestOne(5, 4, 15.52d, 6.26d));
            table.Add(new PhisherTestOne(5, 5, 10.97d, 5.05d));
            table.Add(new PhisherTestOne(5, 6, 8.75d, 4.39d));
            table.Add(new PhisherTestOne(5, 7, 7.46d, 3.97d));
            table.Add(new PhisherTestOne(5, 8, 6.63d, 3.69d));
            table.Add(new PhisherTestOne(5, 9, 6.06d, 3.48d));
            table.Add(new PhisherTestOne(5, 10, 5.64d, 3.33d));
            table.Add(new PhisherTestOne(5, 11, 5.32d, 3.2d));
            table.Add(new PhisherTestOne(5, 12, 5.06d, 3.11d));
            table.Add(new PhisherTestOne(5, 13, 4.86d, 3.02d));
            table.Add(new PhisherTestOne(5, 14, 4.69d, 2.96d));
            table.Add(new PhisherTestOne(5, 15, 4.56d, 2.9d));
            table.Add(new PhisherTestOne(5, 16, 4.44d, 2.85d));
            table.Add(new PhisherTestOne(5, 17, 4.34d, 2.81d));
            table.Add(new PhisherTestOne(6, 1, 5889d, 234d));
            table.Add(new PhisherTestOne(6, 2, 99.33d, 19.33d));
            table.Add(new PhisherTestOne(6, 3, 27.91d, 8.94d));
            table.Add(new PhisherTestOne(6, 4, 15.21d, 6.16d));
            table.Add(new PhisherTestOne(6, 5, 10.67d, 4.95d));
            table.Add(new PhisherTestOne(6, 6, 8.47d, 4.28d));
            table.Add(new PhisherTestOne(6, 7, 7.19d, 3.87d));
            table.Add(new PhisherTestOne(6, 8, 6.37d, 3.58d));
            table.Add(new PhisherTestOne(6, 9, 5.8d, 3.37d));
            table.Add(new PhisherTestOne(6, 10, 5.39d, 3.22d));
            table.Add(new PhisherTestOne(6, 11, 5.07d, 3.09d));
            table.Add(new PhisherTestOne(6, 12, 4.82d, 3d));
            table.Add(new PhisherTestOne(6, 13, 4.62d, 2.92d));
            table.Add(new PhisherTestOne(6, 14, 4.46d, 2.85d));
            table.Add(new PhisherTestOne(6, 15, 4.32d, 2.79d));
            table.Add(new PhisherTestOne(6, 16, 4.2d, 2.74d));
            table.Add(new PhisherTestOne(6, 17, 4.1d, 2.7d));
            table.Add(new PhisherTestOne(7, 1, 5928d, 237d));
            table.Add(new PhisherTestOne(7, 2, 99.34d, 19.36d));
            table.Add(new PhisherTestOne(7, 3, 27.67d, 8.88d));
            table.Add(new PhisherTestOne(7, 4, 14.98d, 6.09d));
            table.Add(new PhisherTestOne(7, 5, 10.45d, 4.88d));
            table.Add(new PhisherTestOne(7, 6, 8.26d, 4.21d));
            table.Add(new PhisherTestOne(7, 7, 7d, 3.79d));
            table.Add(new PhisherTestOne(7, 8, 6.19d, 3.5d));
            table.Add(new PhisherTestOne(7, 9, 5.62d, 3.29d));
            table.Add(new PhisherTestOne(7, 10, 5.21d, 3.14d));
            table.Add(new PhisherTestOne(7, 11, 4.88d, 3.01d));
            table.Add(new PhisherTestOne(7, 12, 4.65d, 2.92d));
            table.Add(new PhisherTestOne(7, 13, 4.44d, 2.84d));
            table.Add(new PhisherTestOne(7, 14, 4.28d, 2.77d));
            table.Add(new PhisherTestOne(7, 15, 4.14d, 2.70d));
            table.Add(new PhisherTestOne(7, 16, 4.03d, 2.66d));
            table.Add(new PhisherTestOne(7, 17, 3.93d, 2.62d));
            table.Add(new PhisherTestOne(8, 1, 5981d, 239d));
            table.Add(new PhisherTestOne(8, 2, 99.36d, 19.37d));
            table.Add(new PhisherTestOne(8, 3, 27.49d, 8.84d));
            table.Add(new PhisherTestOne(8, 4, 14.8d, 6.04d));
            table.Add(new PhisherTestOne(8, 5, 10.27d, 4.82d));
            table.Add(new PhisherTestOne(8, 6, 8.1d, 4.15d));
            table.Add(new PhisherTestOne(8, 7, 6.84d, 3.73d));
            table.Add(new PhisherTestOne(8, 8, 6.03d, 3.44d));
            table.Add(new PhisherTestOne(8, 9, 5.47d, 3.23d));
            table.Add(new PhisherTestOne(8, 10, 5.06d, 3.07d));
            table.Add(new PhisherTestOne(8, 11, 4.74d, 2.95d));
            table.Add(new PhisherTestOne(8, 12, 4.5d, 2.85d));
            table.Add(new PhisherTestOne(8, 13, 4.3d, 2.77d));
            table.Add(new PhisherTestOne(8, 14, 4.14d, 2.7d));
            table.Add(new PhisherTestOne(8, 15, 4d, 2.64d));
            table.Add(new PhisherTestOne(8, 16, 3.89d, 2.59d));
            table.Add(new PhisherTestOne(8, 17, 3.79d, 2.55d));
            table.Add(new PhisherTestOne(9, 1, 6022d, 241d));
            table.Add(new PhisherTestOne(9, 2, 99.38d, 19.38d));
            table.Add(new PhisherTestOne(9, 3, 27.34d, 8.81d));
            table.Add(new PhisherTestOne(9, 4, 14.66d, 6d));
            table.Add(new PhisherTestOne(9, 5, 10.15d, 4.78d));
            table.Add(new PhisherTestOne(9, 6, 7.98d, 4.1d));
            table.Add(new PhisherTestOne(9, 7, 6.71d, 3.68d));
            table.Add(new PhisherTestOne(9, 8, 5.91d, 3.39d));
            table.Add(new PhisherTestOne(9, 9, 5.35d, 3.18d));
            table.Add(new PhisherTestOne(9, 10, 4.95d, 3.02d));
            table.Add(new PhisherTestOne(9, 11, 4.63d, 2.9d));
            table.Add(new PhisherTestOne(9, 12, 4.39d, 2.8d));
            table.Add(new PhisherTestOne(9, 13, 4.19d, 2.72d));
            table.Add(new PhisherTestOne(9, 14, 4.03d, 2.65d));
            table.Add(new PhisherTestOne(9, 15, 3.89d, 2.59d));
            table.Add(new PhisherTestOne(9, 16, 3.78d, 2.54d));
            table.Add(new PhisherTestOne(9, 17, 3.68d, 2.5d));
            table.Add(new PhisherTestOne(10, 1, 6056d, 242d));
            table.Add(new PhisherTestOne(10, 2, 99.4d, 19.39d));
            table.Add(new PhisherTestOne(10, 3, 27.23d, 8.78d));
            table.Add(new PhisherTestOne(10, 4, 14.54d, 5.96d));
            table.Add(new PhisherTestOne(10, 5, 10.05d, 4.74d));
            table.Add(new PhisherTestOne(10, 6, 7.87d, 4.06d));
            table.Add(new PhisherTestOne(10, 7, 6.62d, 3.63d));
            table.Add(new PhisherTestOne(10, 8, 5.82d, 3.34d));
            table.Add(new PhisherTestOne(10, 9, 5.26d, 3.13d));
            table.Add(new PhisherTestOne(10, 10, 4.85d, 2.97d));
            table.Add(new PhisherTestOne(10, 11, 4.54d, 2.86d));
            table.Add(new PhisherTestOne(10, 12, 4.3d, 2.76d));
            table.Add(new PhisherTestOne(10, 13, 4.1d, 2.67d));
            table.Add(new PhisherTestOne(10, 14, 3.94d, 2.6d));
            table.Add(new PhisherTestOne(10, 15, 3.8d, 2.55d));
            table.Add(new PhisherTestOne(10, 16, 3.69d, 2.49d));
            table.Add(new PhisherTestOne(10, 17, 3.59d, 2.45d));
            table.Add(new PhisherTestOne(11, 1, 6082d, 243d));
            table.Add(new PhisherTestOne(11, 2, 99.41d, 19.4d));
            table.Add(new PhisherTestOne(11, 3, 27.13d, 8.76d));
            table.Add(new PhisherTestOne(11, 4, 14.45d, 5.93d));
            table.Add(new PhisherTestOne(11, 5, 9.96d, 4.7d));
            table.Add(new PhisherTestOne(11, 6, 7.79d, 4.03d));
            table.Add(new PhisherTestOne(11, 7, 6.54d, 3.6d));
            table.Add(new PhisherTestOne(11, 8, 5.74d, 3.31d));
            table.Add(new PhisherTestOne(11, 9, 5.18d, 3.1d));
            table.Add(new PhisherTestOne(11, 10, 4.78d, 2.94d));
            table.Add(new PhisherTestOne(11, 11, 4.46d, 2.82d));
            table.Add(new PhisherTestOne(11, 12, 4.22d, 2.72d));
            table.Add(new PhisherTestOne(11, 13, 4.02d, 2.63d));
            table.Add(new PhisherTestOne(11, 14, 3.86d, 2.56d));
            table.Add(new PhisherTestOne(11, 15, 3.73d, 2.51d));
            table.Add(new PhisherTestOne(11, 16, 3.61d, 2.45d));
            table.Add(new PhisherTestOne(11, 17, 3.52d, 2.41d));
            table.Add(new PhisherTestOne(12, 1, 6106d, 244d));
            table.Add(new PhisherTestOne(12, 2, 99.42d, 19.41d));
            table.Add(new PhisherTestOne(12, 3, 27.05d, 8.74d));
            table.Add(new PhisherTestOne(12, 4, 14.37d, 5.91d));
            table.Add(new PhisherTestOne(12, 5, 9.89d, 4.68d));
            table.Add(new PhisherTestOne(12, 6, 7.72d, 4d));
            table.Add(new PhisherTestOne(12, 7, 6.47d, 3.57d));
            table.Add(new PhisherTestOne(12, 8, 5.67d, 3.28d));
            table.Add(new PhisherTestOne(12, 9, 5.11d, 3.07d));
            table.Add(new PhisherTestOne(12, 10, 4.71d, 2.91d));
            table.Add(new PhisherTestOne(12, 11, 4.4d, 2.79d));
            table.Add(new PhisherTestOne(12, 12, 4.16d, 2.69d));
            table.Add(new PhisherTestOne(12, 13, 3.96d, 2.6d));
            table.Add(new PhisherTestOne(12, 14, 3.8d, 2.53d));
            table.Add(new PhisherTestOne(12, 15, 3.67d, 2.48d));
            table.Add(new PhisherTestOne(12, 16, 3.55d, 2.42d));
            table.Add(new PhisherTestOne(12, 17, 3.45d, 2.38d));

        }

        public string Conculete(double varianceEstimation1, double varianceEstimation2, int n1, int n2)
        {
            var f = 0d;
            PhisherTestOne f2;
            if (varianceEstimation1 > varianceEstimation2)
            {
                f = varianceEstimation1 / varianceEstimation2;
                f2 = table.First(m => m.Key1 == n1 && m.Key2 == n2);
            }
            else
            {
                f = (varianceEstimation2 / varianceEstimation1);
                f2 = table.First(m => m.Key1 == n2 && m.Key2 == n1);
            }
            if (f > f2.Value1)
            {
                return "Дисперсии данных распределений не равны при уровнях значимости: 0.01 , 0.05";
            }
            if (f > f2.Value2)
            {
                return "Дисперсии данных распределений не равны при уровнях значимости: 0.05\n" +
                    "Дисперсии данных распределений равны при уровнях значимости: 0.01";
            }
            return "Дисперсии данных распределений равны при уровнях значимости: 0.01 , 0.05";
        }
    }
    public class PhisherTestOne
    {
        public PhisherTestOne(int key1, int key2, double value1, double value2)
        {
            Key1 = key1;
            Key2 = key2;
            Value1 = value1;
            Value2 = value2;
        }

        public int Key1
        { get; set; }

        public int Key2
        { get; set; }

        public double Value1
        { get; set; }

        public double Value2
        { get; set; }
    }
    #endregion

    #region Коэффициенты парных корреляций

    public class CorrelationCoefficients
    {

        public double RXY { get; set; }
        public double RXZ { get; set; }
        public double RYZ { get; set; }

        public CorrelationCoefficients(ObservableCollection<Pair<int, double>> listNumbX,
            ObservableCollection<Pair<int, double>> listNumbY, ObservableCollection<Pair<int, double>> listNumbZ)
        {
            var x = MathHelper.SampleMean(listNumbX);
            var y = MathHelper.SampleMean(listNumbY);
            var z = MathHelper.SampleMean(listNumbZ);
            var sX = MathHelper.StandardDeviation(listNumbX, x);
            var sY = MathHelper.StandardDeviation(listNumbY, y);
            var sZ = MathHelper.StandardDeviation(listNumbZ, z);

            var n = listNumbX.Count;
            var rXY = 0d;
            var rXZ = 0d;
            var rYZ = 0d;
            for (var i = 0; i < n; i++)
            {
                rXY += (listNumbX[i].Value - x) * (listNumbY[i].Value - y);
                rXZ += (listNumbX[i].Value - x) * (listNumbZ[i].Value - z);
                rYZ += (listNumbY[i].Value - y) * (listNumbZ[i].Value - z);
            }
            RXY = rXY / (((double)n - 1d) * sX * sY);
            RXZ = rXZ / (((double)n - 1d) * sX * sZ);
            RYZ = rYZ / (((double)n - 1d) * sY * sZ);
        }

        new public string ToString()
        {
            var str = "";
            if (RXY == 0d)
            {
                str += "X и Y статистически независимые величин\n";
            }
            else if (RXY == 1d || RXY == -1d)
            {
                str += "Между X и Y существует строгая случайная функциональная связь\n";
            }
            if (RXZ == 0d)
            {

                str += "X и Z статистически независимые величин\n";
            }
            else if (RXZ == 1d || RXZ == -1d)
            {
                str += "Между X и Z существует строгая случайная функциональная связь\n";
            }
            if (RYZ == 0d)
            {

                str += "Y и Z статистически независимые величин\n";
            }
            else if (RYZ == 1d || RYZ == -1d)
            {
                str += "Между Y и Z существует строгая случайная функциональная связь\n";
            }
            return str;
        }
    }
    #endregion

}

