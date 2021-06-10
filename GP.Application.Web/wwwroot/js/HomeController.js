var TipojugadaMenuSelect = '';
var TipojugadaCardSelect = '';
var btnCardPrev = null;
var ejemplarNum = '';
var ejemplarName = '';
var resumenCol = 0;
var filtro = 'ALL'; 
var filtroHip = '';
var timeLeft = 60;
var seconds = 60;
var time = 60;
var interval;
//var socket = io();

$(document).ready(function () {

    var executingTimer = false;

    LoadLocacion();

    LoadHipodromosFilter();
    //LoadTipoJugadas();

    $('#panelFilterHip').attr('disabled', true);
    $('#ddlHipFilter').attr('disabled', true);
    $('#btnFilterHip').attr('disabled', true);

    var numberMask = IMask(
        document.getElementById('TMontoApuesta'),
        {
            mask: Number,
            min: 0,
            max: 1000000000000,
            thousandsSeparator: '.'
            
        });

    

    executingTimer = true;
    CargarTodasJugadas();
    executingTimer = false;    

    //socket.on('apuestamsg', function (msg) {
    //    var html = msg.html;
    //    if (msg.tipo == 'jugada') {

    //        if (resumenCol == 0) {
    //            $('#resumenCol1').prepend(html);
    //            resumenCol = 1;
    //        }
    //        else {
    //            $('#resumenCol2').prepend(html);
    //            resumenCol = 0;
    //        }
            
    //    }
    //    else if (msg.tipo == 'aceptacion') {
    //        $('#TPJUG_' + msg.id).replaceWith(html);
    //    }

    //    var id = msg.id;
    //    var saldo = $(data).find('#HSaldo').val();
    //    $('.value-balance').html(saldo + ' ' + currency);

    //    try {
    //        var numberMask2 = IMask(
    //            document.getElementById('btnMontoApuesta_' + id),
    //            {
    //                mask: Number,
    //                min: 0,
    //                max: 1000000000000,
    //                thousandsSeparator: '.'
    //            });
    //    } catch (e) {
    //        console.log(e);
    //        console.log('CajaNro:' + id);
    //    }

    //    $('#btnAceptarApuesta_' + id).click();
    //    $('#btnHeaderCard_' + id).click();
    //});

    timer(timeLeft);

    function RefreshPanel() {
        if (!executingTimer) {
            executingTimer = true;

            //if (filtro == 'ALL')
            //    onFiltroTodas();
            //else if (filtro == 'ME')
            //    onFiltroMisJugadas();
            //else if (filtro == 'ACT')
            //    onFiltroJugadasActivas();
            //else if (filtro == 'CON')
            //    onFiltroJugadasConcretadas();
            //else if (filtro == 'HIP')
            //    onFiltroJugadasHipodromos();
            if (filtro === 'ALL')
                onRefreshTodas(usrid, 'ENCURSO', filtro);
            else if (filtro === 'ME')
                onRefreshTodas(usrid, 'ENCURSO,COMPLETA', filtro);
            else if (filtro === 'ACT')
                onRefreshTodas(usrid, 'ENCURSO', filtro);
            else if (filtro === 'CON')
                onRefreshTodas(usrid, 'COMPLETA', filtro);
            else if (filtro === 'HIP')
                onRefreshTodas(usrid, 'ENCURSO', filtro);
            executingTimer = false;
        }
    }

    function timer(time) {
        interval = setInterval(countDown, 1000);
        function countDown() {
            time--;
            $('#btnRefresh').text('Refresh (' + time + ')');

            if (time === 0) {
                clearInterval(interval);
                RefreshPanel();
                timer(timeLeft);
            }

        }

       
    }

    $('#btnRefresh').on('click', function () {

        //time = parseInt(time + timeLeft);
        $('#btnRefresh').text('Refresh (' + timeLeft + ')');
        //timer(timeLeft);
        time = 0;
        if (time === 0) {
            clearInterval(interval);
            RefreshPanel();
            timer(timeLeft);
        }       
    });

    function LoadHipodromosFilter() {
        var urlAction = $('#HCargarHipodromosFilter').val();

        $("#ddlHipFilter").html('');

        $.ajax({
            method: 'GET',
            url: urlAction,
            dataType: "json",
            beforeSend: function () {
                $("#ddlHipFilter").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
            },
            success: function (data) {
                $("#ddlHipFilter").unblock();
                if (data !== undefined) {
                    $.each(data, function (key, value) {
                        $('#ddlHipFilter').append('<button class="dropdown-item" data-value=' + value.hipodromoID + ' type="button" >' + value.nombre + '</button>');
                    });
                }
            },
            error: function (err) {
                $("#ddlHipFilter").unblock();
                console.log(err);
            }
        });
    }

    function LoadLocacion() {
        var urlAction = $('#HLocacion').val();
        var actionHip = $('#HCargarHipodromos').val();

        $('#dlLocacion').html('');

        $.ajax({
            method: 'GET',
            url: urlAction,
            dataType: "json",
            beforeSend: function () {
                $("#dlLocacion").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
            },
            success: function (data) {
                $("#dlLocacion").unblock();
                if (data !== undefined) {
                    $.each(data, function (key, value) {
                        $('#dlLocacion').append('<a class="dropdown-item" href="#american" data-value="' + value.locacion + '" data-url="' + actionHip + '" onclick="ddlLocation_onclick(this,event)">' + value.locacion + '</a>');
                    });
                }
            },
            error: function (err) {
                $("#dlLocacion").unblock();
                console.log(err);
            }
        });
    }

    //function LoadTipoJugadas(locacion) {

    //    var urlAction = $('#HTipoJugadas').val();

    //    $('#tpjMenuList').html('');

    //    $.ajax({
    //        method: 'GET',
    //        url: urlAction + '?locacion=' + locacion,
    //        dataType: "json",
    //        //beforeSend: function () {
    //        //    $("#hipodomoPanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
    //        //},
    //        success: function (data) {

    //            if (data != undefined) {
    //                $.each(data, function (key, value) {
    //                    $('#tpjMenuList').append('<div id="btnMenuTP_' + value.codigo + '" class="little-box btn btn-dark" onclick="btnMenuTP_click(this,event)">' + value.codigo + '</div>');
    //                });
    //            }
    //        },
    //        error: function (err) {
    //            console.log(err);
    //        }
    //    });

    //}

    function InvocarJugadas(urlAction, urlActionJugadaCard, usuario) {

        var urlGet = urlAction;

        if (usuario !== null)
            urlGet = urlAction + '?usuario=' + usuario;

        $.ajax({
            method: 'GET',
            url: urlGet,
            dataType: "json",
            beforeSend: function () {
                $("#table-info").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
            },
            success: function (data) {

                if (data !== undefined) {
                    $.each(data, function (key, value) {

                        var m_aceptaciones = [];

                        $.each(value.aceptaciones, function (key, vale) {
                            m_aceptaciones.push({
                                'AceptacionID': vale.aceptacionID,
                                'JugadaID': vale.jugadaID,
                                'BanqueadaID': vale.banqueadaID,
                                'Usuario': vale.usuario,
                                'Monto': vale.monto,
                                'MondoJugada': vale.montoJugada,
                                'Status': vale.status,
                                'Fecha': vale.fecha,
                                'CajaNro': vale.cajaNro,
                                'Activo': vale.activo
                            });
                        });

                        var carrera = {
                            'CarreraID': value.carrera.carreraID,
                            'HipodromoID': value.carrera.hipodromoID,
                            'NumeroCarrera': value.carrera.numeroCarrera,
                            'NumeroEjemplar': value.carrera.numeroEjemplar,
                            'NombreEjemplar': value.carrera.nombreEjemplar,
                            'FechaCarrera': value.carrera.fechaCarrera,
                            'HoraCarrera': value.carrera.horaCarrera
                        };

                        var dataModel = {
                            'JugadaID': value.jugadaID,
                            'CarreraID': value.carreraID,
                            'NumeroEjemplar': value.numeroEjemplar,
                            'TipoJugada': value.tipoJugada,
                            'Monto': value.monto,
                            'FechaJugada': value.fechaJugada,
                            'Usuario': value.usuario,
                            'Agente': value.agente,
                            'IdAgente': value.idAgente,
                            'Moneda': value.moneda,
                            'CajaNro': value.cajaNro,
                            'Carrera': carrera,
                            'Status': value.status,                            
                            'Aceptaciones': m_aceptaciones
                        };

                        $.ajax({
                            method: 'POST',
                            url: urlActionJugadaCard,
                            dataType: "html",
                            data: dataModel,
                            //beforeSend: function () {
                            //    $("#hipodomoPanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
                            //},
                            success: function (result) {
                                if (result !== undefined) {
                                    //if (resumenCol == 0) {
                                    //    $('#resumenCol1').append(result);
                                    //    resumenCol = 1;
                                    //}
                                    //else {
                                    //    $('#resumenCol2').append(result);
                                    //    resumenCol = 0;
                                    //}

                                    $('#table-info').append(result);

                                    var id = $(result).find('#cardId').val();
                                    //CargarTipoJugadas($('#' + id));
                                    try {
                                        var numberMask2 = IMask(
                                            document.getElementById('btnMontoApuesta_' + id),
                                            {
                                                mask: Number,
                                                min: 0,
                                                max: 1000000000000,
                                                thousandsSeparator: '.'

                                            });
                                    } catch (e) {
                                        console.log(e);
                                        console.log('CajaNro:' + id);
                                    }
                                    

                                    $('#btnAceptarApuesta_' + id).click();
                                    $('#btnHeaderCard_' + id).click();
                                }
                            },
                            error: function (err) {
                                $("#table-info").unblock();
                                console.log(err);
                            }
                        });





                    });
                }
                $("#table-info").unblock();
            },
            error: function (err) {
                $("#table-info").unblock();
                console.log(err);
            }
        });

    }

    function CargarTodasJugadasConcretadas() {
        var urlAction = $('#HTodasJugadasConcretadas').val();
        var urlActionJugadaCard = $('#HJugadaCardView').val();

        InvocarJugadas(urlAction, urlActionJugadaCard, null);
    }

    function CargarTodasJugadasHipodromos() {
        var urlAction = $('#HTodasJugadasHipodromos').val() + '?hipodromoId=' + filtroHip;
        var urlActionJugadaCard = $('#HJugadaCardView').val();

        InvocarJugadas(urlAction, urlActionJugadaCard, null);
    }

    function CargarTodasJugadas() {
        var urlAction = $('#HTodasJugadas').val();
        var urlActionJugadaCard = $('#HJugadaCardView').val();

        InvocarJugadas(urlAction, urlActionJugadaCard, null);
    }

    function CargarMisJugadas() {

        var urlAction = $('#HMisJugadas').val();
        var urlActionJugadaCard = $('#HJugadaCardView').val();

        InvocarJugadas(urlAction, urlActionJugadaCard, usrid);
    }

    function CargarOtrasJugadas() {
        var urlAction = $('#HOtrosJugadas').val();
        var urlActionJugadaCard = $('#HJugadaCardView').val();

        InvocarJugadas(urlAction, urlActionJugadaCard, usrid);
    }

    function CargarTipoJugadas(panel) {

        var urlAction = $('#HTipoJugadas').val();

        panel.html('');

        $.ajax({
            method: 'GET',
            url: urlAction,
            dataType: "json",
            //beforeSend: function () {
            //    $("#hipodomoPanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
            //},
            success: function (data) {

                if (data !== undefined) {
                    $.each(data, function (key, value) {
                        panel.append('<div id="btnCardTP_' + value.codigo + '" class="little-box btn btn-dark" onclick="btnCardTP_click(this,event)">' + value.codigo + '</div>');
                    });
                }
            },
            error: function (err) {
                console.log(err);
            }
        });

    }

    function validarJugar() {

        var hipodromoId = $('#HHipodromoId').val();
        var carreraNum = parseInt($('#numCarr').html());
        var monto = parseFloat($('#TMontoApuesta').val());
        var tpj = TipojugadaMenuSelect;


        if (hipodromoId === undefined || hipodromoId === "") {
            return false;
        }

        if (carreraNum === undefined || isNaN(carreraNum) || carreraNum === "" || carreraNum === "CARRERA") {
            return false;
        }

        if (ejemplarNum === undefined || ejemplarNum === "") {
            return false;
        }

        if (tpj === undefined || tpj === "") {
            return false;
        }

        if (monto === undefined || isNaN(monto) || monto <= 0) {
            return false;
        }

        return true;
    }



    $('#btnMenuJugar').on('click', function (sender) {
        console.log(sender);
        executingTimer = true;
        if (!validarJugar())
            return;

        var urlAction = $('#btnMenuJugar').attr('data-url');
        var hipodromoId = $('#HHipodromoId').val();
        var hipodromoName = $('#hipName').html();
        var carreraNum = $('#numCarr').html();
        var carreraid = $('#HCarreraId').val();
        var monto = $('#TMontoApuesta').val();
        var info = $('#HCarreraInfo').val();

        monto = monto.replace(/\./g, '');
        monto = monto.replace(',', '.');

        var tpj = TipojugadaMenuSelect;
        var userId = usrid;

       // var nume = Math.floor(Math.random() * 1000000) + 1;



        var id = UUID.generate();// 'TPJJUG_' + padLeft(nume, 7);

        var dataModel = {
            'id': id,
            'UserId': userId,
            'HipodromoId': hipodromoId,
            'HipodromoName': hipodromoName,
            'CarreraId': carreraid,
            'JugadaId': 0,
            'BanquedaId': 0,
            'CarreraNum': carreraNum,
            'EjemplarNum': ejemplarNum,
            'EjemplarName': ejemplarName,
            'TipoJugada': tpj,
            'Monto': monto,
            'AceptacionesMonto': 0,
            'IdAgente': idagente,
            'Agente': agente,
            'Moneda': currency,
            'CajaNro': id,
            'Status': 'ENCURSO',
            'FechaCarreraInfo': info ,
            'Aceptaciones': {}
        };

        $('#btnMenuJugar').prop('disabled', true);

        $.ajax({
            method: 'POST',
            url: urlAction,
            dataType: "html",
            async: true,
            data: dataModel,
            beforeSend: function () {
                $("#table-info").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Procesando...</h1>' });
                executingTimer = true;
            },
            success: function (data) {
                $("#table-info").unblock();
                $('#btnMenuJugar').prop('disabled', false);
                if (data !== undefined) {
                    //if (resumenCol == 0) {
                    //    $('#resumenCol1').prepend(data);
                    //    resumenCol = 1;
                    //}
                    //else {
                    //    $('#resumenCol2').prepend(data);
                    //    resumenCol = 0;
                    //}

                    $('#table-info').prepend(data);
                    var msg = {
                        'id': id,
                        'tipo': 'jugada',
                        'html': data
                    };

                    //socket.emit('apuestamsg', msg);

                    var saldo = $(data).find('#HSaldo').val();
                    $('.value-balance').html(saldo + ' ' + currency);
                    //CargarTipoJugadas($('#' + id));
                    try {
                        var numberMask2 = IMask(
                            document.getElementById('btnMontoApuesta_' + id),
                            {
                                mask: Number,
                                min: 0,
                                max: 1000000000000,
                                thousandsSeparator: '.'
                            });
                    } catch (e) {
                        console.log(e);
                        console.log('CajaNro:' + id);
                    }
                    
                    $('#btnAceptarApuesta_' + id).click();
                    $('#btnHeaderCard_' + id).click();
                }

                $('#TMontoApuesta').val('');
                $('#lstEjemplares').html('');                

                var m_info = info.split(' ');

                ///TODO: completar la informacion de los tickets que se vayan jugando
                var infoTicket = '<li>' + hipodromoName + '</li>';
                infoTicket += '<li>Carrera #' + carreraNum + '</li>';
                infoTicket += '<li>Ejemplar #' + ejemplarNum + '</li>';
                infoTicket += '<li>Tipo Apuesta: Jugada</li>';
                infoTicket += '<li>Fecha Carrera: ' + m_info[0] + '</li>';
                infoTicket += '<li>Hora Carrera: ' + m_info[1] + ' ' + m_info[2] + '</li>';
                infoTicket += '-----------------------------------------';

                
                $('#ticketsList').prepend(infoTicket);

                TipojugadaMenuSelect = '';
                ejemplarNum = '';

                if (btnCardPrev !== undefined && btnCardPrev !== null) {
                    btnCardPrev.style = null;
                }

                executingTimer = false;
            },
            error: function (err) {
                console.log(err);
                $('#btnMenuJugar').prop('disabled', false);
                $("#table-info").unblock();
                var msg = $(err.responseText).find('.stackerror').html();

                Swal.fire({
                    type: 'error',
                    title: 'Grupo Pitazo',
                    text: msg
                });
                executingTimer = false;
            }
        });

    });

    $('#btnMenuBanquear').on('click', function (sender) {
        console.log(sender);

        var urlAction = $('#btnMenuBanquear').attr('data-url');
        var hipodromoId = $('#HHipodromoId').val();
        var hipodromoName = $('#hipName').html();
        var carreraNum = $('#numCarr').html();
        var carreraid = $('#HCarreraId').val();
        var monto = $('#TMontoApuesta').val();
        var tpj = TipojugadaMenuSelect;
        var userId = usrid;

        var nume = Math.floor(Math.random() * 1000000) + 1;

        var id = 'TPJBAN_' + padLeft(nume, 7);

        var dataModel = {
            'id': id,
            'UserId': userId,
            'HipodromoId': hipodromoId,
            'HipodromoName': hipodromoName,
            'CarreraId': carreraid,
            'JugadaId': 0,
            'BanquedaId': 0,
            'CarreraNum': carreraNum,
            'EjemplarNum': ejemplarNum,
            'EjemplarName': ejemplarName,
            'TipoJugada': tpj,
            'Monto': monto,
            'AceptacionesMonto': 0,
            'IdAgente': idagente,
            'Agente': agente,
            'Moneda': currency,
            'CajaNro': id,
            'Aceptaciones': {}
        };

        $.ajax({
            method: 'POST',
            url: urlAction,
            dataType: "html",
            data: dataModel,
            success: function (data) {
                if (data !== undefined) {
                    //if (resumenCol == 0) {
                    //    $('#resumenCol1').append(data);
                    //    resumenCol = 1;
                    //}
                    //else {
                    //    $('#resumenCol2').append(data);
                    //    resumenCol = 0;
                    //}
                    $('#table-info').append(data);
                    CargarTipoJugadas($('#' + id));
                    $('#btnOferta_' + id).click();
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    });

    $('#btnMenuEditEventos').on('click', function (sender) {
        console.log(sender);
    });

    $('#btnMenuReporte').on('click', function (sender) {
        console.log(sender);

        var urlAction = $('#btnMenuReporte').attr('data-url');
        $('#reporte').html('');


        $.ajax({
            method: 'POST',
            url: urlAction + '?usuario=' + usrid,
            dataType: "html",
            success: function (data) {
                if (data !== undefined) {

                    $('#reporte').html(data);
                    $('#report-board').modal('show');
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    });

    $('#btnMenuMsj').on('click', function (sender) {
        console.log(sender);
        $('#modalMsj').html('');
        executingTimer = true;    
        var urlAction = $('#btnMenuMsj').attr('data-url');
        $.ajax({
            method: 'POST',
            url: urlAction + '?usuario=' + usrid,
            dataType: "html",
            success: function (data) {
                if (data !== undefined) {

                    $('#modalMsj').html(data);
                    $('#msg-board').modal('show');
                }
                executingTimer = false;
            },
            error: function (err) {
                console.log(err);
                executingTimer = false;
            }
        });

    });

    $('#rbFiltroTotos').on('click', function (e) {
        executingTimer = true;  
        $('#panelFilterHip').attr('disabled', true);
        $('#ddlHipFilter').attr('disabled', true);
        $('#btnFilterHip').attr('disabled', true);
        onFiltroTodas();
        executingTimer = false;
    });

    $('#rbFiltroMis').on('click', function (e) {
        executingTimer = true;
        $('#panelFilterHip').attr('disabled', true);
        $('#ddlHipFilter').attr('disabled', true);
        $('#btnFilterHip').attr('disabled', true);
        onFiltroMisJugadas();
        executingTimer = false;
    });

    $('#rbFiltroActivas').on('click', function (e) {
        executingTimer = true;
        $('#panelFilterHip').attr('disabled', true);
        $('#ddlHipFilter').attr('disabled', true);
        $('#btnFilterHip').attr('disabled', true);
        onFiltroJugadasActivas();
        executingTimer = false;
    });

    $('#rbFiltroConcretadas').on('click', function (e) {
        executingTimer = true;
        $('#panelFilterHip').attr('disabled', true);
        $('#ddlHipFilter').attr('disabled', true);
        $('#btnFilterHip').attr('disabled', true);
        onFiltroJugadasConcretadas();
        executingTimer = false;
    });

    $('#rbFiltroHipodromos').on('click', function (e) {
        
        $('#panelFilterHip').removeAttr('disabled');
        $('#ddlHipFilter').removeAttr('disabled');
        $('#btnFilterHip').removeAttr('disabled');
        
    });

    $('#ddlHipFilter').on('click', '.dropdown-item', function (e) {
        filtroHip = $(this).attr('data-value');

        var hipName = $(this).html();
        $('#btnFilterHip').html(hipName);
        executingTimer = true;
        onFiltroJugadasHipodromos();
        executingTimer = false;
    });

    function onRefreshTodas(usuario, status, pfiltro)
    {
        var urlAction = $('#HNewJugadas').val();
        var urlActionJugadaCard = $('#HJugadaCardView').val();
        filtro = pfiltro;

        var urlGet = urlAction;
        var urlCad = '';

        if (filtroHip !== '')
            urlCad ='?hipodromoId=' + filtroHip;

        if (usuario !== null) {
            if (urlCad === '')
                urlCad = '?usuario=' + usuario;
            else
                urlCad += '&usuario=' + usuario;
        }

        if (status !== null) {
            if (urlCad === '')
                urlCad = '?status=' + status;
            else
                urlCad += '&status=' + status;
        }

        if (pfiltro !== null) {
            if (urlCad === '')
                urlCad = '?filtro=' + pfiltro;
            else
                urlCad += '&filtro=' + pfiltro;
        }

        urlGet += urlCad;

        $.ajax({
            method: 'GET',
            url: urlGet,
            dataType: "json",
            beforeSend: function () {
                $("#table-info").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
            },
            success: function (data) {

                if (data !== undefined) {
                    $.each(data, function (key, value) {

                        var m_aceptaciones = [];

                        $.each(value.aceptaciones, function (key, vale) {
                            m_aceptaciones.push({
                                'AceptacionID': vale.aceptacionID,
                                'JugadaID': vale.jugadaID,
                                'BanqueadaID': vale.banqueadaID,
                                'Usuario': vale.usuario,
                                'Monto': vale.monto,
                                'MondoJugada': vale.montoJugada,
                                'Status': vale.status,
                                'Fecha': vale.fecha,
                                'CajaNro': vale.cajaNro,
                                'Activo': vale.activo
                            });
                        });

                        var carrera = {
                            'CarreraID': value.carrera.carreraID,
                            'HipodromoID': value.carrera.hipodromoID,
                            'NumeroCarrera': value.carrera.numeroCarrera,
                            'NumeroEjemplar': value.carrera.numeroEjemplar,
                            'NombreEjemplar': value.carrera.nombreEjemplar
                        };

                        var dataModel = {
                            'JugadaID': value.jugadaID,
                            'CarreraID': value.carreraID,
                            'NumeroEjemplar': value.numeroEjemplar,
                            'TipoJugada': value.tipoJugada,
                            'Monto': value.monto,
                            'FechaJugada': value.fechaJugada,
                            'Usuario': value.usuario,
                            'Agente': value.agente,
                            'IdAgente': value.idAgente,
                            'Moneda': value.moneda,
                            'CajaNro': value.cajaNro,
                            'Carrera': carrera,
                            'Status': value.status,
                            'Aceptaciones': m_aceptaciones
                        };

                        $.ajax({
                            method: 'POST',
                            url: urlActionJugadaCard,
                            dataType: "html",
                            data: dataModel,
                            //beforeSend: function () {
                            //    $("#hipodomoPanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
                            //},
                            success: function (result) {
                                if (result !== undefined) {
                                    //if (resumenCol == 0) {
                                    //    $('#resumenCol1').prepend(result);
                                    //    resumenCol = 1;
                                    //}
                                    //else {
                                    //    $('#resumenCol2').prepend(result);
                                    //    resumenCol = 0;
                                    //}
                                    $('#table-info').prepend(result);
                                    var id = $(result).find('#cardId').val();
                                    //CargarTipoJugadas($('#' + id));
                                    try {
                                        var numberMask2 = IMask(
                                            document.getElementById('btnMontoApuesta_' + id),
                                            {
                                                mask: Number,
                                                min: 0,
                                                max: 1000000000000,
                                                thousandsSeparator: '.'

                                            });
                                    } catch (e) {
                                        console.log(e);
                                        console.log('CajaNro:' + id);
                                    }
                                    

                                    $('#btnAceptarApuesta_' + id).click();
                                    $('#btnHeaderCard_' + id).click();
                                }
                            },
                            error: function (err) {
                                $("#table-info").unblock();
                                console.log(err);
                            }
                        });                       


                    });

                    //refresh de las cajas con aceptaciones
                    RefreshJugadasAceptaciones();

                    CleanJugadasVencidas(usuario, status, pfiltro);
                }
                $("#table-info").unblock();
            },
            error: function (err) {
                $("#table-info").unblock();
                console.log(err);
            }
        });

    }

    function CleanJugadasVencidas(usuario, status, pfiltro) {
        var urlAction = $('#HClearJugadas').val();

        filtro = pfiltro;

        var urlGet = urlAction;
        var urlCad = '';

        if (filtroHip !== '')
            urlCad = '?hipodromoId=' + filtroHip;

        if (usuario !== null) {
            if (urlCad === '')
                urlCad = '?usuario=' + usuario;
            else
                urlCad += '&usuario=' + usuario;
        }

        if (status !== null) {
            if (urlCad === '')
                urlCad = '?status=' + status;
            else
                urlCad += '&status=' + status;
        }

        if (pfiltro !== null) {
            if (urlCad === '')
                urlCad = '?filtro=' + pfiltro;
            else
                urlCad += '&filtro=' + pfiltro;
        }

        urlGet += urlCad;

        $.ajax({
            method: 'GET',
            url: urlGet,
            dataType: "json",
            beforeSend: function () {
                
                executingTimer = true;
            },
            success: function (data) {

                if (data !== undefined) {
                    $.each(data, function (key, value) {                                                
                         $('#TPJUG_' + value).remove();
                    });

                }

                executingTimer = false;
                
            },
            error: function (err) {
                executingTimer = false;
                
                console.log(err);
            }
        });
    }

    function RefreshJugadasAceptaciones() {
        var urlAction = $('#HRefreshAceptaciones').val();
        var urlActionJugadaCard = $('#HJugadaCardView').val();

        $.ajax({
            method: 'GET',
            url: urlAction + '?usuario=' + usrid,
            dataType: "json",
            beforeSend: function () {
                $("#table-info").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
                executingTimer = true;
            },
            success: function (data) {

                if (data !== undefined) {
                    $.each(data, function (key, value) {

                        var m_aceptaciones = [];

                        $.each(value.aceptaciones, function (key, vale) {
                            m_aceptaciones.push({
                                'AceptacionID': vale.aceptacionID,
                                'JugadaID': vale.jugadaID,
                                'BanqueadaID': vale.banqueadaID,
                                'Usuario': vale.usuario,
                                'Monto': vale.monto,
                                'MondoJugada': vale.montoJugada,
                                'Status': vale.status,
                                'Fecha': vale.fecha,
                                'CajaNro': vale.cajaNro,
                                'Activo': vale.activo
                            });
                        });

                        var carrera = {
                            'CarreraID': value.carrera.carreraID,
                            'HipodromoID': value.carrera.hipodromoID,
                            'NumeroCarrera': value.carrera.numeroCarrera,
                            'NumeroEjemplar': value.carrera.numeroEjemplar,
                            'NombreEjemplar': value.carrera.nombreEjemplar
                        };

                        var dataModel = {
                            'JugadaID': value.jugadaID,
                            'CarreraID': value.carreraID,
                            'NumeroEjemplar': value.numeroEjemplar,
                            'TipoJugada': value.tipoJugada,
                            'Monto': value.monto,
                            'FechaJugada': value.fechaJugada,
                            'Usuario': value.usuario,
                            'Agente': value.agente,
                            'IdAgente': value.idAgente,
                            'Moneda': value.moneda,
                            'CajaNro': value.cajaNro,
                            'Carrera': carrera,
                            'Status': value.status,
                            'Aceptaciones': m_aceptaciones
                        };

                        var cardid = value.cajaNro;

                        $.ajax({
                            method: 'POST',
                            url: urlActionJugadaCard,
                            dataType: "html",
                            data: dataModel,
                            //beforeSend: function () {
                            //    $("#hipodomoPanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
                            //},
                            success: function (result) {
                                if (result !== undefined) {
                                    $('#TPJUG_' + cardid).replaceWith(result);

                                    var id = $(result).find('#cardId').val();
                                    //CargarTipoJugadas($('#' + id));
                                    try {
                                        var numberMask2 = IMask(
                                            document.getElementById('btnMontoApuesta_' + id),
                                            {
                                                mask: Number,
                                                min: 0,
                                                max: 1000000000000,
                                                thousandsSeparator: '.'

                                            });
                                    } catch (e) {
                                        console.log(e);
                                        console.log('CajaNro:' + id);
                                    }
                                    

                                    $('#btnAceptarApuesta_' + id).click();
                                    $('#btnHeaderCard_' + id).click();
                                }
                            },
                            error: function (err) {
                                $("#table-info").unblock();
                                console.log(err);
                            }
                        });


                    }); 

                    LoadHipodromosFilter();
                    //LoadLocacion();
                    //$('#ddlHipodromos').html('');
                    //$('#ddlCarreras').html('');
                    //$('#lstEjemplares').html('');

                }

                executingTimer = false;
                $("#table-info").unblock();
            },
            error: function (err) {
                executingTimer = false;
                $("#table-info").unblock();
                console.log(err);
            }
        });
    }

    function onFiltroJugadasHipodromos() {
        cleanTableInfo();
        filtro = 'HIP';
        CargarTodasJugadasHipodromos();
    }

    function onFiltroJugadasConcretadas() {
        cleanTableInfo();
        filtro = 'CON';
        CargarTodasJugadasConcretadas();
    }

    function onFiltroJugadasActivas() {
        cleanTableInfo();
        filtro = 'ACT'; 
        CargarOtrasJugadas();
    }

    function onFiltroMisJugadas() {
        cleanTableInfo();
        filtro = 'ME'; 
        CargarMisJugadas();
    }

    function onFiltroTodas() {
        cleanTableInfo();        
        filtro = 'ALL'; 
        CargarTodasJugadas();
    }
    
});

//function isNumberKey(evt) {
//    var charCode = (evt.which) ? evt.which : evt.keyCode;
//    if (charCode != 46 && charCode > 31
//        && (charCode < 48 || charCode > 57))
//        return false;

//    return true;
//}

var UUID = (function () {
    var self = {};
    var lut = []; for (var i = 0; i < 256; i++) { lut[i] = (i < 16 ? '0' : '') + (i).toString(16); }
    self.generate = function () {
        var d0 = Math.random() * 0xffffffff | 0;
        var d1 = Math.random() * 0xffffffff | 0;
        var d2 = Math.random() * 0xffffffff | 0;
        var d3 = Math.random() * 0xffffffff | 0;
        return lut[d0 & 0xff] + lut[d0 >> 8 & 0xff] + lut[d0 >> 16 & 0xff] + lut[d0 >> 24 & 0xff] + '-' +
            lut[d1 & 0xff] + lut[d1 >> 8 & 0xff] + '-' + lut[d1 >> 16 & 0x0f | 0x40] + lut[d1 >> 24 & 0xff] + '-' +
            lut[d2 & 0x3f | 0x80] + lut[d2 >> 8 & 0xff] + '-' + lut[d2 >> 16 & 0xff] + lut[d2 >> 24 & 0xff] +
            lut[d3 & 0xff] + lut[d3 >> 8 & 0xff] + lut[d3 >> 16 & 0xff] + lut[d3 >> 24 & 0xff];
    }
    return self;
})();

function LoadTipoJugadas(locacion) {

    var urlAction = $('#HTipoJugadas').val();

    $('#tpjMenuList').html('');

    $.ajax({
        method: 'GET',
        url: urlAction + '?locacion=' + locacion,
        dataType: "json",
        //beforeSend: function () {
        //    $("#hipodomoPanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
        //},
        success: function (data) {

            if (data !== undefined) {
                $.each(data, function (key, value) {
                    //$('#tpjMenuList').append('<div id="btnMenuTP_' + value.codigo + '" class="little-box btn btn-dark" onclick="btnMenuTP_click(this,event)">' + value.codigo + '</div>');
                    $('#tpjMenuList').append('<div class="little-box btn btn-dark" onclick="btnMenuTP_click(this,event)">' + value.codigo + '</div>');
                    
                });
            }
        },
        error: function (err) {
            console.log(err);
        }
    });

}

function isNumberKey(txt, evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode === 46) {
        //Check if the text already contains the . character
        if (txt.value.indexOf('.') === -1) {
            return true;
        } else {
            return false;
        }
    } else {
        if (charCode > 31 &&
            (charCode < 48 || charCode > 57))
            return false;
    }
    return true;
}

function padLeft(nr, n, str) {
    return Array(n - String(nr).length + 1).join(str || '0') + nr;
}

function btnMenuTP_click(sender, e) {
    console.log(sender);

    if (btnCardPrev !== undefined && btnCardPrev !== null) {
        btnCardPrev.style = null;
    }

    btnCardPrev = sender;

    sender.style.backgroundColor = 'black';
    TipojugadaMenuSelect = $(sender).html();
}

function btnCardTP_click(sender, e) {
    console.log(sender);
    TipojugadaCardSelect = $(sender).html();
}

function ddlLocation_onclick(sender, e) {
    console.log(sender);

    var urlAction = $(sender).attr('data-url');
    var value = $(sender).attr('data-value');
    var loc = $(sender).html();
    $('#location').html($(sender).html());
    $('#hipName').html('HIPODROMO');

    $('#ddlHipodromos').html('');
    $('#ddlHipFilter').html('');
    $('#tpjMenuList').html('');

    $.ajax({
        method: 'GET',
        url: urlAction + '?location= ' + value,
        dataType: "json",
        beforeSend: function () {
            $("#hipodomoPanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });

        },
        success: function (data) {
            $("#hipodomoPanel").unblock();
            if (data !== undefined) {

                $.each(data, function (key, value) {
                    $('#ddlHipodromos').append('<a class="dropdown-item" data-value=' + value.hipodromoID + ' onclick="ddlHipodromo_onclick(this,event)" href="#Hipodromo2">' + value.nombre + '</a>');
                    //$('#ddlHipFilter').append('<button class="dropdown-item" data-value=' + value.hipodromoID + ' type="button" >' + value.nombre + '</button>');
                });

                LoadTipoJugadas(loc);
            }
        },
        error: function (err) {
            $("#hipodomoPanel").unblock();
            console.log(err);
        }
    });
}

function ddlHipodromo_onclick(sender, e) {
    console.log(sender);

    var urlAction = $('#HCarrerasAction').val();
    var value = $(sender).attr('data-value');

    $('#hipName').html($(sender).html());

    $('#HHipodromoId').val(value);

    $('#numCarr').html('CARRERA');

    $('#ddlCarreras').html('');

    $('#lstEjemplares').html('');

    $.ajax({
        method: 'GET',
        url: urlAction + '?hipodromoId= ' + value,
        dataType: "json",
        beforeSend: function () {
            $("#carreraspanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
        },
        success: function (data) {
            $("#carreraspanel").unblock();
            if (data !== undefined) {

                $.each(data, function (key, value) {
                    $('#ddlCarreras').append('<a class="dropdown-item" data-numcarrera=' + value.numeroCarrera + ' data-carreraId=' + value.carreraID + ' data-carreraInfo="'  + value.info + '" data-value=' + value.hipodromoID + ' onclick="ddlEjemplar_onclick(this,event)" href="#Hipodromo3">' + value.numeroCarrera + ' (' + value.info + ')</a>');
                });
            }
        },
        error: function (err) {
            $("#carreraspanel").unblock();
            console.log(err);
        }
    });

}

function ddlEjemplar_onclick(sender, e) {
    console.log(sender);

    var urlAction = $('#HEjemplaresAction').val();
    var value = $(sender).attr('data-value');
    //var num = $(sender).html();
    var num = $(sender).attr('data-numcarrera');
    var hipid = $('#HHipodromoId').val();

    $('#numCarr').html(num);

    $('#HCarreraId').val($(sender).attr('data-carreraId'));
    $('#HCarreraInfo').val($(sender).attr('data-carreraInfo'));

    $('#lstEjemplares').html('');



    $.ajax({
        method: 'GET',
        url: urlAction + '?hipodromoId= ' + value + '&numCarrera=' + num,
        dataType: "json",
        beforeSend: function () {
            $("#ejemplarespanel").block({ message: '<h1><img src="../images/ajax-loader.gif" /> Cargando...</h1>' });
        },
        success: function (data) {
            $("#ejemplarespanel").unblock();
            if (data !== undefined) {

                $.each(data, function (key, value) {
                    $('#lstEjemplares').append('<li class="list-group-item"><div class= "round-number">' + value.numeroEjemplar + '</div><div class="box-bg"><div class="name-horse">' + value.nombreEjemplar + ' (' + hipid + ') </div><div class="btn-radio-horse"><input id="rbEjemplar[]" name="rbEjemplar[]" type="radio" data-num =' + value.numeroEjemplar + ' data-nombre=' + value.nombreEjemplar + ' onclick="rbEjemplar_click(this,event)"/></div></div></li>');
                });
            }
        },
        error: function (err) {
            $("#ejemplarespanel").unblock();
            console.log(err);
        }
    });
}

function rbEjemplar_click(sender, e) {
        
    ejemplarNum = $(sender).attr('data-num');
    ejemplarName = $(sender).attr('data-nombre');

    $('#numEjemplar').html(ejemplarName);
}

function validarAceptarApuesta(monto, montoJug, jugada) {

    if (monto === undefined || isNaN(monto) || monto <= 0 || monto > montoJug) {
        Swal.fire({
            type: 'error',
            title: 'Grupo Pitazo',
            text: 'monto de la apuesta supera al monto en juego'
        });
        return false;
    }
        

    if (jugada === undefined || jugada === "") {
        Swal.fire({
            type: 'error',
            title: 'Grupo Pitazo',
            text: 'La jugada no existe'
        });
        return false;
    }

    return true;
}

function btnCardJugarCollapse_onclick(sender, e) {
    //executingTimer = true;

    //var obj = $(sender).attr('data-collapse');
    
    //if ($('#' + obj).hasClass('collapse')) {
    //    executingTimer = false;
    //}   
    //else if ($('#' + obj).hasClass('show')){
    //    executingTimer = true;
    //}
}

function btnAceptarApuesta_onclick(sender, e) {
    var urlAction = $('#HGuardarAceptacion').val();
    var usr = $(sender).attr('data-usuario');
    var usrApuesta = $(sender).attr('data-usuarioApuesta');
    var jugadaid = $(sender).attr('data-jugadaid');
    var hipId = $(sender).attr('data-hipId');
    var hipname = $(sender).attr('data-hipname');
    var ejemplarNum = parseInt($(sender).attr('data-ejemplarNum'));
    var carreraNum = parseInt($(sender).attr('data-carreraNum'));
    var carreraId = parseInt($(sender).attr('data-carreraId'));
    var cardid = $(sender).attr('data-cardid');
    var jugadaTp = $(sender).attr('data-tpj');
    var montoJugada = parseFloat($(sender).attr('data-montoJugada'));
    var id = $(sender).attr('data-id');        

    if (usrid !== usr) {
        console.log();
        executingTimer = true;
        var montostring = $('#' + id).val();

        montostring = montostring.replace(/\./g, '');
        montostring = montostring.replace(',', '.');

        var monto = parseFloat(montostring);

        if (!validarAceptarApuesta(monto, montoJugada, jugadaTp))
            return;

        //STATUS : ACEPTADA, ANULADA, ELIMINADA
        var dataModel = {
            'AceptacionID': {},
            'JugadaID': jugadaid,
            'BanqueadaID': 0,
            'HipodromoId': hipId,
            'CarreraId': carreraId,
            'CarreraNum': carreraNum,
            'EjemplarNum': ejemplarNum,
            'TipoJugada': jugadaTp,
            'IdAgente': idagente,
            'Agente': agente,
            'Moneda': currency,
            'Usuario': usr,
            'UsuarioApuesta': usrApuesta,
            'Monto': monto,
            'MontoJugada': montoJugada,
            'Status': 'ACEPTADA',
            'Fecha': Date.now(),
            'CajaNro': cardid,
            'Activo': true
        };

        $.ajax({
            method: 'POST',
            url: urlAction,
            async: true,
            dataType: "html",
            data: dataModel,
            beforeSend: function () {
                $("#TPJUG_" + cardid).block({ message: '<h1><img src="../images/ajax-loader.gif" /> Procesando...</h1>' });
                executingTimer = true;
            },
            success: function (data) {
                $("#TPJUG_" + cardid).unblock();
                if (data !== undefined) {

                    $('#TPJUG_' + cardid).replaceWith(data);

                    var msg = {
                        'id': cardid,
                        'tipo': 'aceptacion',
                        'html': data
                    };
                    //socket.emit('chat message', msg);

                    //CargarTipoJugadas($('#' + id));
                    $('#btnAceptarApuesta_' + id).click();
                    var saldo = $(data).find('#HSaldo').val();
                    $('.value-balance').html(saldo + ' ' + currency);
                    try {
                        var numberMask2 = IMask(
                            document.getElementById('btnMontoApuesta_' + cardid),
                            {
                                mask: Number,
                                min: 0,
                                max: 1000000000000,
                                thousandsSeparator: '.'

                            });
                    } catch (e) {
                        console.log(e);
                        console.log('CajaNro:' + id);
                    }

                    //TODO: completar la informacion de los tickets que se vayan jugando

                    var info = $(data).find('#HinfoCarreraFecha').val();
                    var m_info = info.split(' ');
                    var infoTicket = '<li>' + hipname + '</li>';
                    infoTicket += '<li>Carrera #' + carreraNum + '</li>';
                    infoTicket += '<li>Ejemplar #' + ejemplarNum + '</li>';
                    infoTicket += '<li>Tipo Apuesta: Aceptacion Jugada</li>';
                    infoTicket += '<li>Fecha Carrera: ' + m_info[0] + '</li>';
                    infoTicket += '<li>Hora Carrera: ' + m_info[1] + ' ' + m_info[2] + '</li>';
                    infoTicket += '-----------------------------------------';

                    $('#ticketsList').prepend(infoTicket);
                    
                }
                executingTimer = false;
            },
            error: function (err) {
                console.log(err);
                $("#TPJUG_" + cardid).unblock();

                var msg = $(err.responseText).find('.stackerror').html();

                Swal.fire({
                    type: 'error',
                    title: 'Grupo Pitazo',
                    text: msg
                });

                executingTimer = false;
            }
        });

    }
}

function btnAceptarBanqueada_onclick(sender, e) {
    var usr = $(sender).attr('data-usuario');

    if (usrid !== usr) {
        console.log();

    }
}

function btnOferta_onclick(sender, e) {

}

function cleanTableInfo() {
    resumenCol = 0;
    $('#resumenCol1').html('');
    $('#resumenCol2').html('');
}