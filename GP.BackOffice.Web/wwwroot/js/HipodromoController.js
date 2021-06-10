counter = 0;
var table = null;
var urlHome = '';
var isValid = false;
var dtTranslate_es = {
    "sProcessing": "Procesando...",
    "sLengthMenu": "Mostrar _MENU_ registros",
    "sZeroRecords": "No se encontraron resultados",
    "sEmptyTable": "Ningún dato disponible en esta tabla",
    "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
    "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
    "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
    "sInfoPostFix": "",
    "sSearch": "Buscar:",
    "sUrl": "",
    "sInfoThousands": ",",
    "sLoadingRecords": "Cargando...",
    "oPaginate": {
        "sFirst": "Primero",
        "sLast": "Último",
        "sNext": "Siguiente",
        "sPrevious": "Anterior"
    },
    "oAria": {
        "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
        "sSortDescending": ": Activar para ordenar la columna de manera descendente"
    }
};



$(document).ready(function () {
           
    var tipoForm = $('#TipoForm').val();

    onLoad();
    
    function onLoad() {

        switch (tipoForm) {
            case 'CARGA':
                cargarFormVacio();
                break;
            case 'MODIFICAR':                
            case 'RESULTADO':
                cargarFormFiltro(tipoForm);
                break;
            default:
                cargarFormVacio();
                break;
        }

    }
    
    function cargarFormVacio() {        
        $("#divisor").remove();

        var urlAction = $('#HActionVacio').val();

        counter = 0;

        $.ajax({
            method: 'GET',
            url: urlAction ,
            dataType: "html",
            success: function (data) {
                if (data != undefined) {
                    $('#dataView').html(data);

                    var hora = $("#dpHoraCarrera").val();

                    var horaAct = $("#HToday").val();

                    $('.timepicker').timepicker({
                        timeFormat: 'hh:mm p',
                        interval: 1,
                        minTime: horaAct,
                        maxTime: '23:59',
                        defaultTime: horaAct,
                        startTime: hora,
                        dynamic: false,
                        dropdown: true,
                        scrollbar: true
                    });

                    var urlAjax = $("#HHipodromoHandler").val();

                    $.fn.dataTable.ext.errMode = 'none';
                    table = $('#tblEjemplares').DataTable({
                        "sAjaxSource": urlAjax,
                        "bAutoWidth": true,
                        //"responsive": true,
                        "bServerSide": true,
                        "language": dtTranslate_es,
                        "paging": false,
                        "info": false,
                        "select": true,
                        "stateSave": true,
                        "bFilter": false,
                        "deferRender": true,
                        //"scrollY": 300,
                        //"scrollX": true,
                        //"scrollCollapse": true,
                        //"scroller": true,
                        "fnServerParams": function (aoData) {
                            aoData.push({ "name": "hipodromoId", "value": "" });
                            aoData.push({ "name": "numCarreram", "value": 0 });
                            aoData.push({ "name": "numeroEjemplar", "value": counter });
                        },
                        "aoColumns": [
                            { "mDataProp": "numeroEjemplar" },
                            {
                                "mDataProp": "nombreEjemplar", "render": function (oObj) {
                                    console.log(oObj);
                                    return '<input id="tejemplar[]" name="tejemplar[]" type="text" class="form-control" value="' + oObj + '" required /><span id="validator_tejemplar[]" asp-validation-for="tejemplar[]"></span>';
                                }
                            },
                            {
                                "mDataProp": "numeroEjemplar", "render": function (oObj) {

                                    return '<button id="bDel" name="bDel" data-numEjemplar="' + oObj + '" class="btn" title="Eliminar Ejemplar" alt="Eliminar Ejemplar" onclick="bDel_onclick(this, event)"><i class="fa fa-close"></i></button>';
                                }
                            }
                        ]
                    });


                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    function cargarFormFiltro(tipoForm) {
        var urlAction = $("#HAction").val();
        counter = 0;
        $.ajax({
            method: 'GET',
            url: urlAction + '?tipoForm=' + tipoForm,
            dataType: "html",
            success: function (data) {
                if (data != undefined) {
                    $('#findView').html(data);   

                    var urlAction = $("#dpFecha").attr('data-url');
                    cargarCarrerasFiltro(urlAction);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    $(document).on("click", "#btnGuardar", function () {
        var form = $("#frmData");
        isValid = checkForm(form[0]);

        if (isValid)
        Guardar();
    });

    $(document).on("keypress", "#dpHoraCarrera", function (event) {
        event.preventDefault();
        return;
    });

    $(document).on("keydown", "#dpHoraCarrera", function (event) {
        event.preventDefault();
        return;
    });

    $(document).on("click", "#btnBuscar",function () {
        Buscar();        
    });

    function Guardar() {
        var tipoForm = $('#TipoForm').val();
        var urlAction = $("#HActionSave").val();
        var ejemplares = [];

        var loc = $("#ddlLocacion option:selected").val();
        var clasif = $("#ddlClasificacion option:selected").val();
        var hip = $("#ddlDataHipodromo option:selected").val();
        var feccar = $("#dpFechaCarrera").val();
        var horcar = $("#dpHoraCarrera").val();
        var numcar = $("#txtNumCarrera").val();
        var cantejemp = $("#txtCantEjemplares").text();

        //var dataTbl = $('#tblEjemplares tr');

        var dataTbl = {};
        var ind = 0;

        //if (tipoForm == 'MODIFICAR') {            
        //    if ($('#tblEjemplares tr').filter('[role="row"]').length == 0) {
        //        dataTbl = $('#tblEjemplares tr');
        //    }
        //    else {
        //        dataTbl = $('#tblEjemplares tr').filter('[role="row"]');
        //    }
        //}
        //else if (tipoForm == 'RESULTADO') {            
        //    if ($('#tblEjemplares tr').length == 0) {
        //        dataTbl = $('#tblEjemplares tr');
        //    }
        //    else {
        //        dataTbl = $('#tblEjemplares tr .child');
        //        ind = -1;
        //        if (dataTbl.length == 0) {
        //            ind = 0;
        //            console.log('');
        //            dataTbl = $('#tblEjemplares tr').filter('[role="row"]');
        //        }
        //    }
        //}
        //else {            
        //    if ($('#tblEjemplares tr').filter('[role="row"]').length == 0) {
        //        dataTbl = $('#tblEjemplares tr');
        //    }
        //    else {
        //        dataTbl = $('#tblEjemplares tr').filter('[role="row"]');
        //    }
        //}

        if ($('#tblEjemplares tr').filter('[role="row"]').length == 0) {
            dataTbl = $('#tblEjemplares tr');
        }
        else {
            dataTbl = $('#tblEjemplares tr').filter('[role="row"]');
        }

        $.each(dataTbl, function (key, value) {
            console.log(key);
            console.log(value);
            var status = false;
            var resultado = null;

            //if (key > ind) {
            var nro = value.childNodes[0].firstChild.nodeValue;
            if ($.isNumeric(nro)) {
                var nombre = $($('input[name="tejemplar[]"]')[key-1]).val();// $(value.childNodes[1].innerHTML).val();

                if (value.childNodes[2] != undefined) {

                    status = $($('input[name="chkejemplar[]"]')[key-1]).prop('checked'); //$(value.childNodes[2].innerHTML).filter('.fa-toggle-on').length > 0;
                }

                if (value.childNodes[3] != undefined && $(value.childNodes[3]).find('#bDel').length == 0) {
                    resultado = $($('input[name="rejemplar[]"]')[key-1]).val();//$(value.childNodes[3].innerHTML).val();
                }

                ejemplares.push({
                    "NumeroEjemplar": nro,
                    "NombreEjemplar": nombre,
                    "Estatusejemplar": status,
                    "LlegadaEjemplar": resultado
                });
            }
            //}
        });

        if (cantejemp == 0) {
            cantejemp = ejemplares.length;
        }

        var obj = {
            "Operacion": 'EDICION',
            "TipoForm": tipoForm,
            "Locacion": loc,
            "Clasificacion": clasif,
            "HipodromoID": hip,
            "FechaCarrera": feccar,
            "HoraCarrera": horcar,
            "NumeroCarrera": numcar,
            "CantidadEjemplar": cantejemp,
            "NumeroNuevoEjemplar": cantejemp,
            "Ejemplares": ejemplares
        };

        $(document).ajaxStop($.unblockUI); 
        $.ajax({
            method: 'POST',
            url: urlAction,
            data: obj,
            dataType: "json",
            beforeSend: function () {
                $.blockUI({ message: '<h1><img src="../images/ajax-loader.gif" /> Procesando...</h1>' });
            },
            success: function (result) {
                if (result != undefined) {

                    if (result.status == 'OK') {

                        //var msgBox = confirm(result.msg + ', Desea continuar?');
                        //$.unblockUI();

                        Swal.fire({
                            title: 'Desea continuar?',
                            text: result.msg,
                            type: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#3085d6',
                            cancelButtonColor: '#d33',
                            cancelButtonText: 'No',
                            confirmButtonText: 'Si'
                        }).then((result) => {
                            if (result.value) {
                                if (tipoForm == 'CARGA') {
                                    cargarFormVacio();
                                }
                                else {
                                    cargarFormFiltro(tipoForm);
                                    $("#dataView").html('');
                                }
                            }
                            else {
                                urlHome = $("#HActionHome").val();

                                window.location.href = urlHome;
                            }
                        });

                        //if (msgBox) {
                        //    if (tipoForm == 'CARGA') {
                        //        cargarFormVacio();                                
                        //        }
                        //    else {
                        //        cargarFormFiltro();
                        //        $("#dataView").html('');
                        //    }
                        //}
                        //else {
                        //    urlHome = $("#HActionHome").val();

                        //    window.location.href = urlHome;
                        //}
                    }
                    else if (result.status == 'ERROR') {
                        //alert('Error:' + data.msg);
                        //$.unblockUI();

                        Swal.fire({
                            type: 'error',
                            title: 'Grupo Pitazo',
                            text: result.msg
                        });
                    }
                    else {
                        //$.unblockUI();
                        console.log(result.codigo);
                    }
                }
            },
            error: function (err) {
                //$.unblockUI();
                console.log(err);
            }
        });
    }

    function Buscar() {
        var tipoForm = $('#TipoForm').val();
        var urlAction = $('#btnBuscar').attr('data-url');
        var hipodromoId = $('#ddlHipodromo option:selected').val();
        var numcarrera = $('#ddlCarrera option:selected').val();

        var fecha = $("#dpFecha").val();    
        var fechaCompleta = $('#ddlCarrera option:selected').text().split('-')[1].trim();


        var hora = am_pm_to_hours(fechaCompleta.split(' ')[1] + ' ' + fechaCompleta.split(' ')[2]);
        var bCierre = false;

        var columnas = [
            { "mDataProp": "numeroEjemplar" },
            {
                "mDataProp": "nombreEjemplar", "render": function (oObj) {
                    console.log(oObj);
                    return '<input id="tejemplar[]" name="tejemplar[]" type="text" class="form-control" value="' + oObj + '" ' + (tipoForm == 'RESULTADO' ? 'readonly':'') + '  required /><span id="validator_tejemplar[]" asp-validation-for="tejemplar[]"></span>';
                }
            }
        ];


        if (tipoForm == 'MODIFICAR') {
            columnas.push(
                {
                    "mDataProp": "estatusejemplar", "render": function (oObj) {
                        console.log(oObj);
                        return oObj ? '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" checked />' : '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" />';
                    }
                }
            );

        }
        else if (tipoForm == 'RESULTADO') {
            columnas.push(
                {
                    "mDataProp": "estatusejemplar", "render": function (oObj) {
                        console.log(oObj);
                        return oObj ? '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" readonly checked />' : '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" />';
                    }
                }
            );

            columnas.push(
                {
                    "mDataProp": "llegadaEjemplar", "render": function (oObj) {
                        console.log(oObj);
                        return '<input id="rejemplar[]" name="rejemplar[]" type="number" class="form-control"  value="' + oObj + '" required /><span id="validator_rejemplar[]" asp-validation-for="rejemplar[]"></span>';
                    }
                }
            );

        }

        columnas.push(
            {
                "mDataProp": "numeroEjemplar", "render": function (oObj) {

                    return '<button id="bDel" name="bDel" title="Eliminar Ejemplar" alt="Eliminar Ejemplar" ' + (tipoForm == 'RESULTADO' ? 'disabled' : '') + ' data-numEjemplar="' + oObj + '" class="btn" onclick="bDel_onclick(this, event)"><i class="fa fa-close"></i></button>';
                }
            }
        );

        $('#dataView').html('');

        $(document).ajaxStop($.unblockUI); 

        $.ajax({
            method: 'GET',
            url: urlAction + '?tipoForm=' + tipoForm + '&hipodromoId=' + hipodromoId + '&numCarrera=' + numcarrera + '&fecha=' + fecha + '&hora=' + hora,
            dataType: "html",
            beforeSend: function () {
                $.blockUI({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' }); 
            },
            success: function (data) {
                if (data != undefined) {
                    $('#dataView').html(data);

                    var hora = $("#dpHoraCarrera").val();
                    var fecha = $("#dpFechaCarrera").val();

                    hora = hora.replace('m', ':');
                    hora = tConv24(hora);

                    h = hora.split(':')[0];
                    min = hora.split(':')[1];
                    p = min.split(' ')[1];
                    min = min.split(' ')[0];
                    min = PadLeft(min, 2);

                    hora = h + ':' + min + ' ' + p;

                    $("#dpHoraCarrera").val(hora);

                    var horaAct = $("#HToday").val();

                    if (tipoForm != 'RESULTADO') {
                        $('.timepicker').timepicker({
                            timeFormat: 'hh:mm p',
                            interval: 1,
                            minTime: horaAct,
                            maxTime: '23:55',
                            defaultTime: hora,
                            startTime: hora,
                            dynamic: false,
                            dropdown: true,
                            scrollbar: true
                        });
                    }

                    var urlActionHip = $("#HActionHip").val();

                    var txtloc = $('#ddlLocacion').attr('data-selectdValue');

                    $("#ddlLocacion option[value=" + txtloc + "]").prop("selected", true);

                    var txtclasif = $('#ddlClasificacion').attr('data-selectdValue');
                    $("#ddlClasificacion option[value=" + txtclasif + "]").prop("selected", true);


                    var hipId = $('#ddlDataHipodromo').attr('data-selectdValue');

                    $('#ddlDataHipodromo').html('');

                    $.ajax({
                        method: 'GET',
                        url: urlActionHip + '?locacion=' + txtloc + '&clasif=' + txtclasif,
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            if (data != undefined) {
                                $.each(data, function (key, value) {
                                    $('#ddlDataHipodromo').append('<option value="' + value.hipodromoID + '">' + value.nombre + '</option>');
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                        }
                    });

                    $("#ddlDataHipodromo option[value=" + hipId + "]").prop("selected", true);


                    var num = $('#txtNumCarrera').val();

                    var urlAjax = $("#btnBuscar").attr('data-ajaxHandler');

                    $.fn.dataTable.ext.errMode = 'none';
                    table = $('#tblEjemplares').DataTable({
                        "sAjaxSource": urlAjax,
                        "bAutoWidth": true,
                        //"responsive": true,
                        "bServerSide": true,
                        "language": dtTranslate_es,
                        "bFilter": false,
                        "paging": false,
                        "info": false,
                        "select": true,
                        "stateSave": true,
                        "deferRender": true,
                        //"scrollY": 300,
                        //"scrollX": true,
                        //"scrollCollapse": true,
                        //"scroller": true,
                        "fnServerParams": function (aoData) {
                            aoData.push({ "name": "hipodromoId", "value": hipId });
                            aoData.push({ "name": "numCarreram", "value": num });
                            aoData.push({ "name": "fechaCarrera", "value": fecha });
                            aoData.push({ "name": "horaCarrera", "value": hora });
                        },
                        "aoColumns": columnas
                    });

                    var fecCierre = $('#HfechaCierreCarrera').val();
                    var horaCierre = $('#HhoraCierreCarrera').val();
                    $('#tblEjemplares tr input').prop('disabled', false);

                    if (fecCierre != "" && horaCierre != "") {
                        bCierre = true;
                        $('#dpFechaCarrera').prop('readonly', true);
                        $('#dpHoraCarrera').prop('readonly', true);
                        $('#txtNumCarrera').prop('readonly', true);
                        $('#ddlLocacion').prop('disabled', true);
                        $('#ddlClasificacion').prop('disabled', true);
                        $('#ddlDataHipodromo').prop('disabled', true);
                        $('#btnGuardar').prop('disabled', true);
                    }

                }
                else {
                    Swal.fire({
                        type: 'alert',
                        title: 'Grupo Pitazo',
                        text: 'No se cargo la informacion!'
                    });
                }
            },
            complete: function () {
                // Handle the complete event
                if (bCierre) {
                    $('#tblEjemplares tr input').prop('disabled', true);
                    $('#tblEjemplares button').prop('disabled', true);
                }


            },
            error: function (err) {
                console.log(err);
            }
        });

        
    }
});

function onchangeHandler(sender, e) {


    var validatorName = "#validator_" + $(sender)[0].currentTarget.id;

    $(validatorName).text('');

    $(sender).off();

    //isValid = true;
}

function checkForm(form) {

    var valid = true;       

    if (form.ddlLocacion.value == "") {
        $("#validator_ddlLocacion").text("* Ingrese Locacion.");        
        valid = false;
    }

    if (form.dpFechaCarrera.value == "") {
        $("#validator_dpFechaCarrera").text("* Ingrese una Fecha.");
        //$(document).on('change', "#dpFechaCarrera", onchangeHandler);
        valid = false;
    }   

    if (form.ddlClasificacion.value == "") {
        $("#validator_ddlClasificacion").text("* Ingrese la clasificacion.");
        valid = false;
    }

    if (form.ddlDataHipodromo.value == "") {
        $("#validator_ddlDataHipodromo").text("* Ingrese el hipodromo.");
        valid = false;
    }

    if (form.dpHoraCarrera.value == "") {
        $("#validator_dpHoraCarrera").text("* Ingrese ingrese la hora de la carrera.");
        $(document).on('change', "#dpHoraCarrera", onchangeHandler);
        valid = false;
    }



    if (form.txtNumCarrera.value == "" || form.txtNumCarrera.value == 0) {
        $("#validator_txtNumCarrera").text("* Ingrese un numero de carrera.");
        $(document).on('change', "#txtNumCarrera", onchangeHandler);
        valid = false;
    }

    //if (form.txtCantEjemplares.value == "" || form.txtCantEjemplares.value == 0) {
    //    $("#validator_txtCantEjemplares").text("* Ingrese al menos un ejemplar.");
    //    $(document).on('change', "#txtCantEjemplares", onchangeHandler);
    //    valid = false;
    //}    

    var dataTbl = {};

    if ($('#tblEjemplares tr').filter('[role="row"]').length == 0) {
        dataTbl = $('#tblEjemplares tr');
    }
    else {
        dataTbl = $('#tblEjemplares tr').filter('[role="row"]');
    }

    
    

    $.each(dataTbl, function (key, value) {
        var nro = value.childNodes[0].firstChild.nodeValue;
        if (key > 0) {
            var tejemplarObj = $($('input[name="tejemplar[]"]')[key - 1]);
            var rejemplarObj = $($('input[name="rejemplar[]"]')[key - 1]);

            var nombre = $($('input[name="tejemplar[]"]')[key - 1]).val();
            var resultado = $($('input[name="rejemplar[]"]')[key - 1]).val();
            
            if (nombre == "") {
                var validatorObj = $($('span[id="validator_tejemplar[]"]')[key - 1]);
                validatorObj.text("* Ingrese un nombre de ejemplar.");                
                $(document).on('change', tejemplarObj, function (sender,e) {
                    
                    $($('span[id="validator_tejemplar[]"]')[key - 1]).text('');
                });
                valid = false;
            }

            if (resultado == "") {
                var validatorResObj = $($('span[id="validator_rejemplar[]"]')[key - 1]);
                validatorResObj.text("* Ingrese un resultado.");
                $(document).on('change', rejemplarObj, function (sender, e) {                    

                    $($('span[id="validator_rejemplar[]"]')[key - 1]).text('');
                });
                valid = false;
            }
        }
    });

    //Guardar();


    return valid;
}

function addRow_onclick(sender, e) {

    var form = $("#frmData");
    isValid = checkForm(form[0]);

    if (!isValid)
        return;

    var urlAjax = $("#btnGuardar").attr('data-ajaxHandler');
    var tipoForm = $('#TipoForm').val();
    var urlAction = $("#HActionSave").val();
    var ejemplares = [];

    //if (counter == 0)
    counter = parseInt($('#txtCantEjemplares').text()) + 1;

    var feccar = $("#dpFechaCarrera").val();
    var horcar = $("#dpHoraCarrera").val();

    var loc = $("#ddlLocacion option:selected").val();
    var clasif = $("#ddlClasificacion option:selected").val();
    var hip = $("#ddlDataHipodromo option:selected").val();

    var numcar = $("#txtNumCarrera").val();

    //$("#tblEjemplares").dataTable().fnDestroy();
    var num = $('#txtNumCarrera').val();

    var hipId = $('#ddlDataHipodromo option:selected').val();

    var columnas = [
        { "mDataProp": "numeroEjemplar" },
        {
            "mDataProp": "nombreEjemplar", "render": function (oObj) {
                console.log(oObj);
                return '<input id="tejemplar[]" name="tejemplar[]" type="text" class="form-control" value="' + oObj + '" ' + (tipoForm == 'RESULTADO' ? 'readonly' : '') + ' required /><span id="validator_tejemplar[]" asp-validation-for="tejemplar[]"></span>';
            }
        }
    ];


    if (tipoForm == 'MODIFICAR') {
        columnas.push(
            {
                "mDataProp": "estatusejemplar", "render": function (oObj) {
                    console.log(oObj);
                    return oObj ? '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" checked />' : '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" />';
                }
            }
        );

    }
    else if (tipoForm == 'RESULTADO') {
        columnas.push(
            {
                "mDataProp": "estatusejemplar", "render": function (oObj) {
                    console.log(oObj);
                    return oObj ? '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" readonly checked />' : '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" />';
                }
            }
        );

        columnas.push(
            {
                "mDataProp": "llegadaEjemplar", "render": function (oObj) {
                    console.log(oObj);
                    return '<input id="rejemplar[]" name="rejemplar[]" type="number" class="form-control"  value="' + oObj + '" required /><span id="validator_rejemplar[]" asp-validation-for="rejemplar[]"></span>';
                }
            }
        );

    }

    columnas.push(
        {
            "mDataProp": "numeroEjemplar", "render": function (oObj) {

                return '<button id="bDel" name="bDel" title="Eliminar Ejemplar" alt="Eliminar Ejemplar" ' + (tipoForm == 'RESULTADO' ? 'disabled' : '') + ' data-numEjemplar="' + oObj + '" class="btn" onclick="bDel_onclick(this, event)"><i class="fa fa-close"></i></button>';
            }
        }
    );


    $("#txtCantEjemplares").text(counter);


    var cantejemp = $("#txtCantEjemplares").text();
    
    var dataTbl = {};

    if ($('#tblEjemplares tr').filter('[role="row"]').length == 0) {
        dataTbl = $('#tblEjemplares tr');
    }
    else {
        dataTbl = $('#tblEjemplares tr').filter('[role="row"]');
    }

    //dataTbl = $('#tblEjemplares tr');

    $.each(dataTbl, function (key, value) {
        console.log(key);
        console.log(value);
        var status = false;
        var resultado = null;

        //if (key > 0) {
        var nro = value.childNodes[0].firstChild.nodeValue;

        if ($.isNumeric(nro)) {
            var nombre = $($('input[name="tejemplar[]"]')[key-1]).val();//$(value.childNodes[1].innerHTML).val();

            if (value.childNodes[2] != undefined) {

                status = $($('input[name="chkejemplar[]"]')[key-1]).prop('checked'); //$(value.childNodes[2].innerHTML).filter('.fa-toggle-on').length > 0;
            }

            if (value.childNodes[3] != undefined && $(value.childNodes[3]).find('#bDel').length == 0) {
                resultado = $($('input[name="rejemplar[]"]')[Key-1]).val();//$(value.childNodes[3].innerHTML).val();
            }

            ejemplares.push({
                "NumeroEjemplar": !$.isNumeric(nro) ? 1 : nro,
                "NombreEjemplar": nombre == undefined ? '_' : nombre,
                "Estatusejemplar": status,
                "LlegadaEjemplar": resultado
            });
        }
        //}
    });
       
    

    var obj = {
        "Operacion":'NUEVOEJEMPLAR',
        "TipoForm": tipoForm,
        "Locacion": loc,
        "Clasificacion": clasif,
        "HipodromoID": hip,
        "FechaCarrera": feccar,
        "HoraCarrera": horcar,
        "NumeroCarrera": numcar,
        "CantidadEjemplar": cantejemp,
        "NumeroNuevoEjemplar": counter,
        "Ejemplares": ejemplares
    };

    $(document).ajaxStop($.unblockUI); 

    $.ajax({
        method: 'POST',
        url: urlAction,
        async: false,
        data: obj,
        dataType: "json",
        beforeSend: function () {
            $.blockUI({ message: '<h1><img src="../images/ajax-loader.gif" /> Agregando...</h1>' });
        },
        success: function (data) {
            if (data != undefined) {

                if (data.status != 'ERROR') {
                    console.log(data.status);


                    //redibujar la tabla
                    if (counter > 0) {
                        RedibujarTabla(columnas, urlAjax, hipId, num, counter, feccar, horcar);

                        $('#dpFechaCarrera').prop('readonly', true);
                        $('#dpHoraCarrera').prop('readonly', true);
                        $('#txtNumCarrera').prop('readonly', true);
                        $('#ddlLocacion').prop('disabled', true);
                        $('#ddlClasificacion').prop('disabled', true);
                        $('#ddlDataHipodromo').prop('disabled', true);
                    }

                    counter++;
                }
                else {                   

                    //alert('Error:' + data.msg);
                    Swal.fire({
                        type: 'error',
                        title: 'Grupo Pitazo',
                        text: data.msg
                    });
                    counter--;
                    $("#txtCantEjemplares").text(counter);
                }
            }
        },
        error: function (err) {
            counter--;
            $("#txtCantEjemplares").text(counter);
            console.log(err);
        }
    });

    
}

function RedibujarTabla(columnas, url, hipId, num, counter, fecha, hora) {
    $.fn.dataTable.ext.errMode = 'none';
    $("#tblEjemplares").dataTable().fnDestroy();
    table = $('#tblEjemplares').DataTable({
        "sAjaxSource": url,
        "async": false,
        "bServerSide": true,
        "bAutoWidth": true,
        //"responsive": true,
        "language": dtTranslate_es,
        "select": true,
        "paging": false,
        "info": false,
        "bFilter": false,
        "stateSave": true,
        "deferRender": true,
        //"scrollY": 300,
        //"scrollX": true,
        //"scrollCollapse": true,
        //"scroller": true,
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "hipodromoId", "value": hipId });
            aoData.push({ "name": "numCarreram", "value": num });
            aoData.push({ "name": "numeroEjemplar", "value": counter });
            aoData.push({ "name": "fechaCarrera", "value": fecha });
            aoData.push({ "name": "horaCarrera", "value": hora });
        },
        "aoColumns": columnas
    });
}

function ddlDataHipodromo_onchange(sender, e) {

    
        var validatorName = "#validator_" + sender.id;

        $(validatorName).text('');
    
}

function ddlClasificacion_onchange(sender,e) {
    
        var validatorName = "#validator_" + sender.id;

    $(validatorName).text('');

    var clasificacion = $(sender).val();

    var locacion = $("#ddlLocacion option:selected").text();
    var urlAction = $("#HActionHip").val();

    $('#ddlDataHipodromo').html('');

    if (locacion != undefined && locacion != '') {
        $.ajax({
            method: 'GET',
            url: urlAction + '?locacion=' + locacion + '&clasif=' + clasificacion,
            dataType: "json",
            beforeSend: function () {
                $("#dataHip").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
            },
            success: function (data) {
                $("#dataHip").unblock();
                if (data != undefined) {
                    $('#ddlDataHipodromo').append('<option value=""></option>');
                    $.each(data, function (key, value) {
                        $('#ddlDataHipodromo').append('<option value="' + value.hipodromoID + '">' + value.nombre + '</option>');
                    });
                }
            },
            error: function (err) {
                $("#dataHip").unblock();
                console.log(err);
            }
        });
    }
}

function tConv24(time24) {
    var ts = time24;
    var H = +ts.substr(0, 2);
    var h = (H % 12) || 12;
    h = (h < 10) ? ("0" + h) : h;  // leading 0 at the left for 1 digit hours
    var ampm = H < 12 ? " AM" : " PM";
    ts = h + ts.substr(2, 3) + ampm;
    return ts;
}

function am_pm_to_hours(time) {
    console.log(time);
    var hours = Number(time.match(/^(\d+)/)[1]);
    var minutes = Number(time.match(/:(\d+)/)[1]);
    var AMPM = time.match(/\s(.*)$/)[1];
    if (AMPM.toLowerCase() == "pm" && hours < 12) hours = hours + 12;
    if (AMPM.toLowerCase() == "am" && hours == 12) hours = hours - 12;
    var sHours = hours.toString();
    var sMinutes = minutes.toString();
    if (hours < 10) sHours = "0" + sHours;
    if (minutes < 10) sMinutes = "0" + sMinutes;
    return (sHours + ':' + sMinutes);
}

function PadLeft (cad, size) {
    var s = String(cad);
    while (s.length < (size || 2)) { s = "0" + s; }
    return s;
}

function cargarCarrerasFiltro(urlAction) {
    var fecha = $("#dpFecha").val();    
    var tipoForm = $('#TipoForm').val();
    $('#ddlHipodromo').html('');

    $.ajax({
        method: 'GET',
        url: urlAction + '?fecha=' + fecha + '&tipoForm=' + tipoForm,
        dataType: "json",
        beforeSend: function () {
            $("#findView").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
        },
        success: function (data) {
            $("#findView").unblock();
            if (data != undefined) {
                $('#ddlHipodromo').append('<option value=""></option>');
                $.each(data, function (key, value) {
                    $('#ddlHipodromo').append('<option value="' + value.hipodromoID + '">' + value.nombre + '</option>');
                });
            }
        },
        error: function (err) {
            $("#findView").unblock();
            console.log(err);
        }
    });
}

function dpFecha_onchange(sender, e) {
    var urlAction = $(sender).attr('data-url');
    cargarCarrerasFiltro(urlAction);
}

function dpFechaCarrera_onchange(sender, e) {
    var validatorName = "#validator_" + sender.id;

    $(validatorName).text('');

    var fecha = $("#dpFechaCarrera").val();
    fecha = fecha.replace('/', '-');
    var dia_f = Number(fecha.split('-')[2]);
    var mes_f = Number(fecha.split('-')[1]);
    var year_f = Number(fecha.split('-')[0]);


    var today = new Date();
    var time = PadLeft(today.getHours(), 2) + ":" + PadLeft(today.getMinutes(), 2) + ":" + PadLeft(today.getSeconds(),2);

    $("#HToday").val(time);

    var horaAct = $("#HToday").val();
    var horaAct12 = tConv24(horaAct);
    $("#dpHoraCarrera").val(horaAct12);

    if (dia_f == today.getDate() && mes_f == (today.getMonth() + 1) && year_f == today.getFullYear())
        $('.timepicker').timepicker('option', 'minTime', time);
    else {
        $('.timepicker').timepicker('option', 'minTime', '00:00');
        $('.timepicker').timepicker('option', 'startTime', '00:00');
    }
}

function ddlLocacion_onchange(sender, e) {

    
        var validatorName = "#validator_" + sender.id;

        $(validatorName).text('');
    

    var id = $(sender).val();
    var clasificiacion = $("#ddlClasificacion option:selected").text();
    var urlAction = $("#HActionHip").val();

    $('#ddlDataHipodromo').html('');

    if (clasificiacion != undefined && clasificiacion != '') {
        $.ajax({
            method: 'GET',
            url: urlAction + '?locacion=' + id + '&clasif=' + clasificiacion,
            dataType: "json",
            beforeSend: function () {
                $("#dataHip").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
            },
            success: function (data) {
                $("#dataHip").unblock();
                if (data != undefined) {
                    $('#ddlDataHipodromo').append('<option value=""></option>');
                    $.each(data, function (key, value) {
                        $('#ddlDataHipodromo').append('<option value="' + value.hipodromoID + '">' + value.nombre + '</option>');
                    });
                }
            },
            error: function (err) {
                $("#dataHip").unblock();
                console.log(err);
            }
        });
    }
}

function ddlHipodromo_onchange(sender, e) {
    console.log(sender);

    var id = $(sender).val();
    var txt = $("#ddlHipodromo option:selected").text();
    var urlAction = $(sender).attr('data-url');
    var tipoForm = $('#TipoForm').val();
    var fecha = $("#dpFecha").val();

    $('#ddlCarrera').html('');
    
    $.ajax({
        method: 'GET',
        url: urlAction + '?fecha= ' + fecha + '&hipodromoId=' + id + '&tipoForm=' + tipoForm,
        dataType: "json",
        beforeSend: function () {
            $("#findView").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
        },
        success: function (data) {
            $("#findView").unblock();
            if (data != undefined) {
                $('#ddlCarrera').append('<option value=""></option>');
                $.each(data, function (key, value) {                    
                    $('#ddlCarrera').append('<option value="' + value.numeroCarrera + '">' + value.numeroCarrera + ' - ' + value.info + '</option>');
                });
            }
        },
        error: function (err) {
            $("#findView").unblock();
            console.log(err);
        }
    });
}

function bDel_onclick(sender, e) {
    e.preventDefault();
    
    Swal.fire({
        title: 'Desea Eliminar el ejemplar?',
        text: '',
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        cancelButtonText: 'No',
        confirmButtonText: 'Si'
    }).then((result) => {
        if (result.value) {
            eliminarEjemplar(sender);
        }        
    });

   
}

function eliminarEjemplar(sender) {
    var tipoForm = $('#TipoForm').val();
    var urlDel = $("#HActionDel").val();
    var urlAjax = $("#btnGuardar").attr('data-ajaxHandler');
    var num = $('#txtNumCarrera').val();
    var hipId = $('#ddlDataHipodromo option:selected').val();

    var numEjem = $(sender).attr('data-numEjemplar');     
    var feccar = $("#dpFechaCarrera").val();
    var horcar = $("#dpHoraCarrera").val();


    counter = parseInt($('#txtCantEjemplares').text());

    var columnas = [
        { "mDataProp": "numeroEjemplar" },
        {
            "mDataProp": "nombreEjemplar", "render": function (oObj) {
                console.log(oObj);
                return '<input id="tejemplar[]" name="tejemplar[]" type="text" class="form-control" value="' + oObj + '" ' + (tipoForm == 'RESULTADO' ? 'readonly' : '') + ' required /><span id="validator_tejemplar[]" asp-validation-for="tejemplar[]"></span>';
            }
        }
    ];


    if (tipoForm == 'MODIFICAR') {
        columnas.push(
            {
                "mDataProp": "estatusejemplar", "render": function (oObj) {
                    console.log(oObj);
                    return oObj ? '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" checked />' : '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" />';
                }
            }
        );

    }
    else if (tipoForm == 'RESULTADO') {
        columnas.push(
            {
                "mDataProp": "estatusejemplar", "render": function (oObj) {
                    console.log(oObj);
                    return oObj ? '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" readonly checked />' : '<input id="chkejemplar[]" name="chkejemplar[]" type="checkbox" class="form-control" />';
                }
            }
        );

        columnas.push(
            {
                "mDataProp": "llegadaEjemplar", "render": function (oObj) {
                    console.log(oObj);
                    return '<input id="rejemplar[]" name="rejemplar[]" type="number" class="form-control"  value="' + oObj + '" required /><span id="validator_rejemplar[]" asp-validation-for="rejemplar[]"></span>';
                }
            }
        );

    }

    columnas.push(
        {
            "mDataProp": "numeroEjemplar", "render": function (oObj) {

                return '<button id="bDel" name="bDel" title="Eliminar Ejemplar" alt="Eliminar Ejemplar" data-numEjemplar="' + oObj + '" class="btn" onclick="bDel_onclick(this, event)"><i class="fa fa-close"></i></button>';
            }
        }
    );


    var obj = {
        "hipodromoID": hipId,
        "numeroCarrera": num,
        "numeroEjemplar": numEjem,
        "fecha": feccar,
        "hora":am_pm_to_hours(horcar)
    };



    $.ajax({
        method: 'POST',
        url: urlDel,
        data: obj,
        dataType: "json",        
        beforeSend: function () {
            $("#dataView").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
        },
        success: function (data) {
            if (data != undefined) {

                if (data.status != 'ERROR') {
                    console.log(data.status);
                    counter--;
                    //redibujar la tabla
                    RedibujarTabla(columnas, urlAjax, hipId, num, '', feccar, horcar);

                    //var count = 0;
                    
                    //count = $('#tblEjemplares tr').length - 2;

                    //if (count < 0)
                    //    count = 0;

                    //$('#txtCantEjemplares').text(count);
                    ContarEjemplares();
                    $("#dataView").unblock();
                }
                else {
                    $("#dataView").unblock();
                    //alert('Error:' + data.msg);

                    if (data.codigo == 100) {
                        Swal.fire({
                        type: 'error',
                        title: 'Grupo Pitazo',
                        text: data.msg
                    });
                    }

                    RedibujarTabla(columnas, urlAjax, hipId, num, '', feccar, horcar);

                    ContarEjemplares();
                }
            }
        },
        error: function (err) {
            $("#dataView").unblock();
            console.log(err);
        }
    });

    function ContarEjemplares() {

        var count = 0;

        count = $('#tblEjemplares tr').length - 2;

        if (count < 0)
            count = 0;

        $('#txtCantEjemplares').text(count);
    }
}

