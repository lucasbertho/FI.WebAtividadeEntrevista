using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            List<string> erros = new List<string>();

            List<Beneficiario> beneficiarios = new List<Beneficiario>();

            if (!this.ModelState.IsValid)
            {
                erros.AddRange((from item in ModelState.Values
                                from error in item.Errors
                                select error.ErrorMessage).ToList());
            }
            // Valida CPF
            if (!Validacao.ValidarCPF(model.CPF))
            {
                erros.Add("Digite um CPF válido");
            }
            // Verifica se CPF já existe na base de dados
            else if (bo.VerificarExistencia(model.CPF) != 0)
            {
                erros.Add("CPF já cadastrado");
            }
            // Valida beneficiários
            else if (model.Beneficiarios != null)
            {
                BoBeneficiario boBen = new BoBeneficiario();

                beneficiarios = model.Beneficiarios.Select(beneficiario => new Beneficiario
                {
                    Id = beneficiario.Id,
                    CPF = beneficiario.CPF,
                    Nome = beneficiario.Nome,
                    IdCliente = model.Id
                }).ToList();

                foreach (var beneficiario in beneficiarios)
                {
                    if (!Validacao.ValidarCPF(beneficiario.CPF))
                    {
                        erros.Add("Digite um CPF válido para o beneficiário");
                    }

                    long idCliente = boBen.VerificarExistencia(beneficiario.CPF);
                    if (idCliente > 0 && idCliente != beneficiario.IdCliente)
                    {
                        erros.Add("CPF do beneficiário já cadastrado para outro cliente");
                    };
                };
            
            }

            if (erros.Count() > 0)
            {
                Response.StatusCode = 400;
                return Json(string.Join("</br>", erros.Distinct()));
            }

            model.Id = bo.Incluir(new Cliente()
            {
                CEP = model.CEP,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone,
                CPF = model.CPF,
                Beneficiarios = beneficiarios
            });

            return Json("Cadastro efetuado com sucesso");
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            List<string> erros = new List<string>();

            List<Beneficiario> beneficiarios = new List<Beneficiario>();

            if (!this.ModelState.IsValid)
            {
                erros.AddRange((from item in ModelState.Values
                                from error in item.Errors
                                select error.ErrorMessage).ToList());
            }
            // Valida CPF
            if (!Validacao.ValidarCPF(model.CPF))
            {
                erros.Add("Digite um CPF válido");
            }
            // Verifica se CPF já existe na base de dados
            else if (bo.VerificarExistencia(model.CPF) != model.Id)
            {
                erros.Add("CPF já cadastrado para outro cliente");
            }
            // Valida beneficiários
            else if (model.Beneficiarios != null)
            {
                BoBeneficiario boBen = new BoBeneficiario();

                beneficiarios = model.Beneficiarios.Select(beneficiario => new Beneficiario
                {
                    Id = beneficiario.Id,
                    CPF = beneficiario.CPF,
                    Nome = beneficiario.Nome,
                    IdCliente = model.Id
                }).ToList();

                foreach (var beneficiario in beneficiarios)
                {
                    if (!Validacao.ValidarCPF(beneficiario.CPF))
                    {
                        erros.Add("Digite um CPF válido para o beneficiário");
                    }

                    long idCliente = boBen.VerificarExistencia(beneficiario.CPF);
                    if (idCliente > 0 && idCliente != beneficiario.IdCliente)
                    {
                        erros.Add("CPF do beneficiário já cadastrado para outro cliente");
                    };
                };
            }

            if (erros.Count() > 0)
            {
                Response.StatusCode = 400;
                return Json(string.Join("</br>", erros.Distinct()));
            }

            bo.Alterar(new Cliente()
            {
                Id = model.Id,
                CEP = model.CEP,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone,
                CPF = model.CPF,
                Beneficiarios = beneficiarios
            });

            return Json("Cadastro alterado com sucesso");
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;
            List<BeneficiarioModel> beneficiarios = new List<BeneficiarioModel>();

            if (cliente != null)
            {
                if (cliente.Beneficiarios != null)
                {
                    beneficiarios = cliente.Beneficiarios.Select(beneficiario => new BeneficiarioModel
                    {
                        Id = beneficiario.Id,
                        CPF = beneficiario.CPF,
                        Nome = beneficiario.Nome,
                        IdCliente = beneficiario.IdCliente
                    }).ToList();
                }

                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF,
                    Beneficiarios = beneficiarios
                };

            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}