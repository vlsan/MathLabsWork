$(document).ready(function () {
    //$("#requestItems-dialog").dialog({ autoOpen: false, height: 'auto', minWidth: '950', modal: true });
    //$("#RequestEdit-dialog").dialog({ autoOpen: false, height: 'auto', minWidth: '650', modal: true });
    //$("#OrederTSItems-dialog-body").dialog({ autoOpen: false, height: 'auto', minWidth: '900', modal: true });
    //$("#dialog-OrderItemHistory").dialog({ autoOpen: false, height: 'auto', minWidth: '500', modal: false });
    //// GetRequest();
    //SetParams();
    //SetDateRange($("#searchRequestPanel").find("input[name='tDatStart']"), $("#searchRequestPanel").find("input[name='tDatEnd']"));
    //GetRequest();

    $("#bInsertNewValue").on("click", function () {
        addNewData();
    });
    //$("#cancel_b").on("click", function () {
    //    $("#RequestEdit-dialog").dialog("close");
    //});

    //$("#sRequestType").on("change", function () {
    //    SetRequestType($("#sRequestType").val());
    //    //alert($("#sRequestType").val());
    //});

    //$("#bSearchRequest").on("click", function () {
    //    GetRequest();
    //});
    //setCustomersSelect();
    $("#tableTitle").text("labData");
    getlabData(1);
    getLabResult1();
    getLabResult2();
    setChart("EmpiricalFrequencies", $("#myChart"), 0.1, 0.1);
    setChart("StatisticalSeries", $("#myChart1"), 20, 1);
});



//var ctx = document.getElementById('myChart').getContext('2d');
function setChart(type, ctx, padding, min) {
    var params = {
        type: type
    };
    // ctx = document.getElementById('myChart').getContext('2d');
    $.get("/Lab/GetChartData", params, function (data) {
        if (data.status === "ok") {

            var _labels = data.labels;//["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"];// data.labels;
            var _values = data.values; //[1.25, 2.81, 15.46, 26.55, 39.95, 57.65, 72.72, 83.15, 93.27, 109.39, 122.51, 134.51];//data.values;
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: _labels,
                    datasets: [{
                        label: data.label,
                        data: _values,
                        backgroundColor: [
                            'rgba(216, 27, 96, 0.6)',
                            //    'rgba(3, 169, 244, 0.6)',
                            //    'rgba(255, 152, 0, 0.6)',
                            //    'rgba(29, 233, 182, 0.6)',
                            //    'rgba(156, 39, 176, 0.6)',
                            //    'rgba(84, 110, 122, 0.6)'
                        ],
                        borderColor: [
                            'rgba(216, 27, 96, 1)',
                            //    'rgba(3, 169, 244, 1)',
                            //    'rgba(255, 152, 0, 1)',
                            //    'rgba(29, 233, 182, 1)',
                            //    'rgba(156, 39, 176, 1)',
                            //    'rgba(84, 110, 122, 1)'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: 'Life Expectancy by Country',
                        position: 'top',
                        fontSize: 16,
                        padding: 0.1
                    },
                    scales: {
                        yAxes: [{
                            ticks: {
                                min: 0.1
                            }
                        }]
                    }
                }
            });



        }
        else {
            alert(data.status);
        }
    });


}



function getlabData(Id) {
    params = {
        Id:Id
    };
       
    $.post("/Lab/GetLabData", params, function (data) {
        $("#labTableBody").html(data);
    });
}

function getLabResult1() {
    params = {
        Id:'1'
    };

    $.post("/Lab/GetLabResult1", params, function (data) {
        getSampleMean();
        $("#labResult").html(data);
    });
}

function getLabResult2() {
    params = {
        Id: '1'
    };

    $.post("/Lab/GetLabResult2", params, function (data) {
        getSampleMean();
        $("#labResult1").html(data);
    });
}

function getSampleMean() {
    var params = {
        SValue: $("#tSvalue").val()
    };
    $.get("/Lab/GetSampleMean", params, function (data) {
        if (data.status === "Ok") {

            $("#sSampleMean").text(data.sampleMean);
            $("#sVarianceEstimation").text(data.varianceEstimation);
        }
        else {
            alert(data.status);
        }
    });
}

function addNewData() {
    var params = {
        SValue: $("#tSvalue").val()
    };
    $.post("/Lab/AddLabData", params, function (data) {
        if (data.status === "Ok") {
            getlabData(1);
            getLabResult1();
            getSampleMean();
            $("#bInsertNewValue").val('0');
        }
        else {
            alert(data.status);
        }
    });
}

function SetDataTable() {
    $('#labResult').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Russian.json"
        },
        "displayLength": 10
        , "ordering": true
        , "searching": false,
        "order": [[0, "desc"]]
    });
}
