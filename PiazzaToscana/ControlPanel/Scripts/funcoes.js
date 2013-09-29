function float2moeda(num) {

    x = 0;

    if (num < 0) {
        num = Math.abs(num);
        x = 1;
    } if (isNaN(num)) num = "0";
    cents = Math.floor((num * 100 + 0.5) % 100);

    num = Math.floor((num * 100 + 0.5) / 100).toString();

    if (cents < 10) cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + '.'
               + num.substring(num.length - (4 * i + 3)); ret = num + ',' + cents; if (x == 1) ret = ' - ' + ret; return ret;

}

function soNumeros(caracter) {

    if (window.event) { // Internet Explorer
        var tecla = event.keyCode;
    }
    else { // Firefox
        var tecla = caracter.value.which;
    }

    if ((tecla >= 48) && (tecla <= 57)){
        return true;
    }
    else {
        return false;
    }
}

function mudaCorMenu(objeto,objetos) {
    var corSelect = "#F2F2F2";
    var corUnselect ="#FFF";
    var obj = document.getElementById(objeto);
    obj.style.backgroundColor = corSelect;
    zeraCor(objeto, objetos, corUnselect);

    }

    function zeraCor(objeto, objetos, corUnselect) {

    var vetor = objetos.split(',');

      for (indice = 0; indice < vetor.length; indice++) {

            if (vetor[indice] != objeto) {

                 var obj2 = document.getElementById(vetor[indice]);
                 obj2.style.backgroundColor = corUnselect;

            }
      }


    }


function soma(valores, resultado) {

    var somatorio = 0.0;
    var vetor = valores.split(' + ');
    var tamanhoString = 0;
    var tamanhoSoma = 0;

    for (indice = 0; indice < vetor.length; indice++) {

        if (document.getElementById(vetor[indice]).value != "") {

            var val = (document.getElementById(vetor[indice]).value).replace(".", "");
            alert(val);
            val = val.replace(",", ".");
            alert(val);
            somatorio = somatorio + parseFloat(val);
            alert(somatorio);
        }
        
     }

  
    document.getElementById(resultado).value = somatorio.toString();


}

function isset(o) {
    if (o != undefined && o != null)
        return true;

    return false;
}

function limpatudo(valores,mascaras) {

    var vetor = valores.split(',');
    var vetor2 = mascaras.split(',');

    for (indice = 0; indice < vetor.length; indice++) {

        if (vetor2[indice] == "NO-MASK") {
            $(document.getElementById(vetor[indice])).val('');
        }

        else if (vetor2[indice] == "DROP") {
            document.getElementById(vetor[indice]).selectedIndex = 0;
            document.getElementById(vetor[indice]).selected = true;
        }

        else {
            $(document.getElementById(vetor[indice])).val('');
            $(document.getElementById(vetor[indice])).unmask();
            mascara(vetor[indice], vetor2[indice]);
        }
    }

}

function foco(campo) {

    var obj = document.getElementById(campo);
    obj.focus();
    msgError();

}

function loadForm() {
}

function carregando() {
    var img;
    img = document.getElementById("carregando");
    img.style["visibility"] = "visible";
}

function msgBrowseError() {
    if (document.getElementById("wasDeleted").value == "true") {
        document.getElementById("DivBrowse").style["visibility"] = "visible";
        document.getElementById("DivBrowse").style["height"] = "auto";
        //document.getElementById("DivBrowse").style["border-style"] = "dotted";
    }
    else if (document.getElementById("isBrowseValid").value == "False") {
        document.getElementById("DivBrowseError").style["visibility"] = "visible";
        document.getElementById("DivBrowseError").style["height"] = "auto";
        //document.getElementById("DivBrowseError").style["border-style"] = "dotted";
    }
}

function msgError() {
    if (document.getElementById("isSuccess").value == "true") {
        document.getElementById("DivMessage").style["visibility"] = "visible";
        document.getElementById("DivMessage").style["height"] = "auto";
        //document.getElementById("DivMessage").style["border-style"] = "dotted";
    }
    else if (document.getElementById("isValid").value == "False") {
        document.getElementById("DivError").style["visibility"] = "visible";
        document.getElementById("DivError").style["height"] = "auto";
        //document.getElementById("DivError").style["border-style"] = "dotted";
    }
}

function Refresh(index, pagesize) {
    var img;
    img = document.getElementById("carregando");
    img.style["visibility"] = "visible";

    $.ajax({
        type: "POST",
        url: "List",
        data: {
            index: index,
            pageSize: pagesize
        },
        success: function (data) {
            $('#div-list').html(data);
            $('#msgs').html("");
            $('#carregando').css("visibility", "hidden");
        }
    });
}

function validaDrp(source, mensagem) {
    var obj;
    obj = source;
    if (obj.value == "" || obj.value == "-1") {
        alert(mensagem);
        return false;
    }
    else
        return true;
}

function FormataNumero(campo) {
    vr = campo.value;
    sinal = vr.substr(0, 1);
    vr = vr.replace("-", "");
    vr = vr.replace("/", "");
    vr = vr.replace(",", "");
    vr = vr.replace(".", "");
    tam = vr.length;

    if (tam == 0)
        campo.value = vr + "0,00";

    if (tam <= 2 && tam > 0) {
        campo.value = vr + ",00";
    }
    if ((tam > 2) && (tam <= 5)) {
        campo.value = vr.substr(0, tam - 2) + ',' + vr.substr(tam - 2, tam);
    }
    if ((tam >= 6) && (tam <= 8)) {
        campo.value = vr.substr(0, tam - 5) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
    }
    if ((tam >= 9) && (tam <= 11)) {
        campo.value = vr.substr(0, tam - 8) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
    }
    if ((tam >= 12) && (tam <= 14)) {
        campo.value = vr.substr(0, tam - 11) + '.' + vr.substr(tam - 11, 3) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
    }
    if ((tam >= 15) && (tam <= 17)) {
        campo.value = vr.substr(0, tam - 14) + '.' + vr.substr(tam - 14, 3) + '.' + vr.substr(tam - 11, 3) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
    }

    if (sinal == "-") {
        campo.value = "-" + campo.value
    }
}



function isNumeric(source) {
    
    var count;
    var e;
    var i;
    var chars;
    var flag;
    flag = true;
    count = source.value.length;
    i = 0;
    chars = "0123456789-,.";

    while (i <= count - 1) {
        e = source.value.substr(i, 1);
        if (chars.indexOf(e) == -1) {
            flag = false;
            i = count + 2;
        }
        i = i + 1;

    }

    return flag;

}


function fnValidaValor(source) {
    var obj;
    obj = source; // document.forms[0].item(source);

    if (!isNumeric(obj)) {
        alert("Valor deve ser preenchida com números");
        obj.value = "0,00";
        obj.focus();
        return false;
    }

    FormataNumero(obj);

    return true;
}

function fnVerData(data) {
    var meses = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
    var hoje = new Date();
    var ano = hoje.getYear();
    if (ano >= 50 && ano <= 99)
        ano = 1900 + ano
    else
        ano = 2000 + ano;
    var dt = new String(data);
    var pos1 = dt.indexOf("/", 0)
    if (pos1 < 0) {
        return false;
    }
    var dd = dt.substring(0, pos1)
    pos2 = dt.indexOf("/", pos1 + 1)
    if (pos2 < 0) {
        return false;
    }
    var mm = dt.substring(pos1 + 1, pos2)
    var aa = dt.substring(pos2 + 1, dt.length)
    var resto = parseInt(aa, 10) % 4
    if (!resto) {
        meses[1] = 29;
    }
    if (mm > 12 || mm < 1) {
        return false;
    }
    if (dd > meses[mm - 1] || dd < 1) {
        return false;
    }
    if (aa.length < 4)
        return false;
    if (aa < 1900 || aa > ano) {
        alert("Ano inválido " + aa);
        return false;
    }
    if (mm.length < 2)
        mmstr = "0" + mm
    else
        mmstr = mm;
    if (dd.length < 2)
        ddstr = "0" + dd
    else
        ddstr = dd;
    dataconv = ddstr + "/" + mmstr + "/" + aa;
    return true;
}

function fnValidaData(source, aceitaNull) {
    var obj;
    obj = source;

    if (aceitaNull == false && (source.value == "__/__/____" || source.value == undefined || source.value == ""))  {
        alert("Campo Data deve ser preenchido.");
        source.value = "";
        return false;
    }

    if (!fnVerData(source.value)) {
        alert("Data inválida. Preencha novamente com o formato: dd/mm/aaaa.");
        source.value = "";
        return false;
    }

    return true;

}

function TarjaIn(obj) {
    obj.style["background-color"] = "#000";
}

function TarjaOut(obj) {
    obj.style["background-color"] = "red !important";
}

function TarjaOutCinza(obj) {
    obj.style.backgroundColor = "#E8E8E8";
}


function validaNumero(source) {
    if (!isNumeric(source)) {
        alert("Número inválido");
        return false;
    }
    else
        return true;
}

function closeDetail(obj) {
    document.getElementById(obj).style["visibility"] = "hidden";
}
