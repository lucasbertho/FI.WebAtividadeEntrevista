

function AdicionarBeneficiario(CPF, Nome) {
    var random = Number(Math.random().toString().replace('.', '')).toString();

    $('#listaBeneficiarios').find('tbody').append(
        '   <tr id=' + random + '>                                                                      ' +
        '       <th scope="row">' + CPF + '</th>                                                        ' +
        '       <td>' + Nome + '</td>                                                                   ' +
        '       <td>                                                                                    ' +
        '           <button type="button" class="btn btn-primary form-control" onclick="AlterarBeneficiario(\'' + random + '\')">Alterar</button>         ' +
        '       </td>                                                                                   ' +
        '       <td>                                                                                    ' +
        '           <button type="button" class="btn btn-primary form-control" onclick="ExcluirBeneficiario(\'' + random + '\')">Excluir</button> ' +
        '       </td>                                                                                   ' +
        '   </tr>                                                                                       '
    );
}

function AlterarBeneficiario(Id) {
    $('#formBeneficiarios #listaBeneficiarios').find('tbody').children().each(function () {
        if (Id == $(this).attr("id")) {
            var Beneficiario = {};

            Beneficiario["Id"] = $(this).attr("id");
            Beneficiario["CPF"] = $(this).find("th:nth-child(1)").text().trim();
            Beneficiario["Nome"] = $(this).find("td:nth-child(2)").text().trim();
            Beneficiario["IdCliente"] = "0";

            $('#formBeneficiarios #CPF').val(Beneficiario["CPF"]);
            $('#formBeneficiarios #Nome').val(Beneficiario["Nome"]);
            $('#idBeneficiario').val(Beneficiario["Id"]);

            return;
        }
    });
}

function ExcluirBeneficiario(Id) {
    var row = document.getElementById(Id);
    row.parentNode.removeChild(row);
}

$('#formBeneficiarios').submit(function (e) {
    e.preventDefault();

    let cpf = $(this).find("#beneficiariosModal #CPF").val();
    let id = "0";
    let idCliente = $('#idCliente').val();
    let nome = $(this).find("#beneficiariosModal #Nome").val();

    var totalCPF = 0;

    $('#formBeneficiarios #listaBeneficiarios').find('tbody').children().each(function () {
        if (cpf === $(this).find("th:nth-child(1)").text().trim()) {
            if ($('#idBeneficiario').val() != $(this).attr("id"))
                ModalDialog("Ocorreu um erro", "CPF já cadastrado para outro beneficiário");
            else {
                $(this).find("th:nth-child(1)").html(cpf);
                $(this).find("td:nth-child(2)").html(nome);
                $('#formBeneficiarios #CPF').val("");
                $('#formBeneficiarios #Nome').val("");
                $('#idBeneficiario').val("0");
            }
            totalCPF++;
        };
    });

    if (totalCPF > 0) {
        return;
    }

    $.ajax({
        url: "/Beneficiario/Validar",
        method: "POST",
        data: {
            "CPF": cpf,
            "ID": id,
            "IDCLIENTE": idCliente,
            "NOME": nome
        },
        error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
        success:
            function (r) {
                //ModalDialog("Sucesso!", r)

                if ($('#idBeneficiario').val() === "0") {
                    AdicionarBeneficiario(r.CPF, r.Nome);
                }

                //window.location.href = urlRetorno;
                $('#idBeneficiario').val("0");
                $("#formBeneficiarios")[0].reset();
            }
    })
})