using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebMathModelLabs.LabCore.BLL;
using WebMathModelLabs.Models;

namespace WebMathModelLabs.Controllers
{
    public class LabController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILabService _labService;
        public LabController(ILogger<HomeController> logger, ILabService labService)
        {
            _logger = logger;
            _labService = labService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult lab2Index()
        {
            return View("Lab2Index");
        }
        public IActionResult lab3Index()
        {
            return View("Lab3Index");
        }
        public IActionResult lab4Index()
        {
            return View("Lab4Index");
        }
        public IActionResult lab5Index()
        {
            return View("Lab5Index");
        }
        public IActionResult lab6Index()
        {
            return View("Lab6Index");
        }


        [HttpPost]
        public ActionResult<List<LabDataViewModel>> GetLabData(int cnt)
        {
            var model = _labService.GetLabDataViewModels();
            return PartialView("_LabDatatable", model);
        }

        [HttpPost]
        public ActionResult<List<StatisticViewModel>> GetLabResult1()
        { 
            var model= _labService.GetLabDataViewModels();
            var st = _labService.StatisticalSeries(model);
            return PartialView("LabResultTable1", st);
        }

        [HttpPost]
        public ActionResult<List<StatisticViewModel>> GetLabResult2()
        {
            var model = _labService.GetLabDataViewModels();
            var st = _labService.EmpiricalFrequencies(model);
            return PartialView("LabResultTable1", st);
        }

        [HttpPost]
        public ActionResult<List<StatisticViewModel>> GetLab3Result()
        {
            var model = _labService.GetLabDataViewModels();
            var st= _labService.GetTheoreticalFrequencies(model);
            return PartialView("_Lab3ResultTable", st);
        }
        public ActionResult<List<StatisticViewModel>> GetLab4Result()
        {
            var model = _labService.GetLabDataViewModels();
            var st = _labService.GetEmpiricalAndTheoreticalFrequencies(model);
            return PartialView("_Lab4ResultTable", st);

        }


        [HttpGet]
        public JsonResult GetChartData(string type)
        {
           
            if (type == "EmpiricalFrequencies")
            {
                var model = _labService.GetLabDataViewModels();
                var st = _labService.EmpiricalFrequencies(model);
                var labels = st.Select(s => s.Skey).ToArray();
                var values = st.Select(s => Math.Round(s.SValue,2)).ToArray();

               
                return Json(new { status = "ok", label= "EmpiricalFrequencies", values = values, labels=labels });
            }
            else if(type == "StatisticalSeries")
            {
                var model = _labService.GetLabDataViewModels();
                var st = _labService.StatisticalSeries(model);
                var labels = st.Select(s => s.Skey).ToArray();
                var values = st.Select(s => Math.Round(s.SValue, 2)).ToArray();


                return Json(new {status ="ok",label = "StatisticalSeries", values = values, labels = labels });
            }
            else
            {
                return Json(new { status = "not" });
            }
        }


        [HttpPost]
        public JsonResult AddLabData(string SValue)
        {
            try
            {
                _labService.AddData(SValue);
                return Json(new { status = "Ok" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "Ошибка сервера " + ex.Message
                });
            }
        }

        [HttpGet]
        public JsonResult GetSampleMean(string SValue)
        {
            try
            {
                var sampleMean = _labService.GetSampleMean();
                var varianceEstimation = _labService.GetVarianceEstimation();
                return Json(new { status = "Ok", 
                    sampleMean= sampleMean.ToString(),
                    varianceEstimation = varianceEstimation.ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "Ошибка сервера " + ex.Message
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="req">"assimetry", "excess", "samplemean"</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetValueByRequest(string req)
        { 
            try
            {
                var result = _labService.GetValueByReguest(req);
                return Json( new { status = "Ok",
                resultData = result.ToString() });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    status = "Ошибка сервера " + ex.Message
                });
            }
        }
    }
}
